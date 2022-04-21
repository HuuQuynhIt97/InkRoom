using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INK_API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using INK_API._Repositories.Interface;
using INK_API._Services.Interface;
using INK_API.DTO;
using INK_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using INK_API.Data;

namespace INK_API._Services.Services
{
    public class ChemicalService : IChemicalService
    {
        private readonly IProcessRepository _repoProcess;
        private readonly IChemicalRepository _repoChemical;
        private readonly ISupplierRepository _repoSup;
        private readonly IStockRepository _repoStock;
        private readonly IInkRepository _repoInk;
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly MapperConfiguration _configMapper;
        public ChemicalService(DataContext context , IStockRepository repoStock, IChemicalRepository repoChemical, IProcessRepository repoProcess, ISupplierRepository repoSup, IInkRepository repoInk, IMapper mapper, MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoInk = repoInk;
            _repoSup = repoSup;
            _repoProcess = repoProcess;
            _repoStock = repoStock;
            _repoChemical = repoChemical;
            _context = context;

        }

        public async Task<object> GetAllStock()
        {
            var resultStart = DateTime.Now;
            var resultEnd = DateTime.Now;
            return await _repoStock.FindAll(x => x.CreatedDate >= resultStart.Date && x.CreatedDate <= resultEnd.Date).ProjectTo<StockDTO>(_configMapper).OrderByDescending(x => x.ID).ToListAsync();
        }

        public async Task<object> ScanInput(string qrCode, string subName, string building, int userid)
        {
            try
            {
                // load tat ca supplier
                var supModel = _repoSup.GetAll();
                // lay gia tri "barcode" trong chuỗi qrcode được chuyền lên
                var Barcode = qrCode.Split('-', '-')[2];
                // tim ID của ingredient
                
                // Find ingredient theo ingredientID vừa tìm được ở trên
                // lấy giá trị "ProductionDate" trong chuỗi qrcode được chuyền lên
                var ProductionDate = qrCode.Split('-')[0];
                // lấy giá trị "Batch" trong chuỗi qrcode được chuyền lên
                var Batch = qrCode.Split('-', '-')[1];
                // sau đó convert sang kiểu date time
                var ProductionDates = Convert.ToDateTime(ProductionDate.Substring(0, 4) + "/" + ProductionDate.Substring(4, 2) + "/" + ProductionDate.Substring(6, 2));
                // var exp = ProductionDates.AddMonths(3);
                // khai báo biến start = ngày hiện tại
                var resultStart = DateTime.Now;
                // khai báo biến end = ngày hiện tại
                var resultEnd = DateTime.Now;
                var ID = 0 ;
                var NameInkOrChemical = string.Empty;
                var SupplierName = string.Empty;
                var InkOrChemicalID = 0;
                var Unit = 0;
                var Code = string.Empty;
                var exp = DateTime.Now;
                if (subName == "Ink") {
                    ID = _repoInk.FindAll(x => x.MaterialNO.Equals(Barcode)).FirstOrDefault().ID;
                    var model = _repoInk.FindById(ID);
                    NameInkOrChemical = model.Name;
                    SupplierName = supModel.FirstOrDefault(s => s.ID == model.SupplierID).Name;
                    InkOrChemicalID = model.ID;
                    Unit = model.Unit.ToInt();
                    Code = model.MaterialNO;
                    exp = ProductionDates.AddDays(model.DaysToExpiration);
                } 
                else 
                {
                    ID = _repoChemical.FindAll(x => x.MaterialNO.Equals(Barcode)).FirstOrDefault().ID;
                    var model = _repoChemical.FindById(ID);
                    NameInkOrChemical = model.Name;
                    SupplierName = supModel.FirstOrDefault(s => s.ID == model.SupplierID).Name;
                    InkOrChemicalID = model.ID;
                    Unit = model.Unit.ToInt();
                    Code = model.MaterialNO;
                    exp = ProductionDates.AddDays(model.DaysToExpiration);
                }
                // tạo ingredientInfo mới
                var data = await CreateStock(new Stock
                {
                    NameInkOrChemical = NameInkOrChemical,
                    ExpiredTime = exp.Date,
                    ManufacturingDate = ProductionDates.Date,
                    SupplierName = SupplierName,
                    Unit = Unit,
                    Batch = Batch,
                    Code = Code,
                    InkOrChemicalID = InkOrChemicalID,
                    UserID = userid,
                    SubName = subName,
                    BuildingName = building

                });
                return true;
            }
            catch (System.Exception)
            {
                return false;
                throw;
            }
            // throw new NotImplementedException();
        }

        public async Task<object> ScanOutput(string qrCode, string subName, string building, int userid)
        {
            // load tat ca supplier
            var supModel = _repoSup.GetAll();
            // lay gia tri "barcode" trong chuỗi qrcode được chuyền lên
            var Barcode = qrCode.Split('-', '-')[2];
            // tim ID của ingredient
            // lấy giá trị "Batch" trong chuỗi qrcode được chuyền lên
            var Batch = qrCode.Split('-', '-')[1];

            var currentDay = DateTime.Now;

            // check trong bang ingredientReport xem đã tồn tại code hay chưa , nếu có tồn tại 
            if (await CheckBarCodeExists(Barcode))
            {
                // check tiep trong bang ingredientReport xem co du lieu chua 
                var checkStatus = _repoStock.FindAll(x => x.Code == Barcode && x.BuildingName == building && x.Batch == Batch && x.CreatedDate == currentDay.Date && x.SubName == subName && x.Status == false).OrderBy(y => y.CreatedTime).FirstOrDefault();
                // nếu khác Null thi update lai
                if (checkStatus != null)
                {
                    checkStatus.Status = true;
                    await UpdateStock(checkStatus);
                }
                else
                {
                    return new
                    {
                        status = false,
                        message = "Đã dùng hết !"
                    };
                }
            }

            // nếu chưa tồn tại thì thêm mới
            else
            {
                return new
                {
                    status = false,
                    message = "Hãy scan QR Code hàng nhập trước :) !"
                };
            }

            return true;
        }

        public async Task<bool> CheckBarCodeExists(string code)
        {
            return await _context.Stocks.AnyAsync(x => x.Code.Equals(code));
        }

        public async Task<Stock> UpdateStock(Stock data)
        {
            try
            {
                _repoStock.Update(data);
                await _repoStock.SaveAll();
                return data;
            }
            catch (Exception)
            {

                return data;
            }
        }

        public async Task<Stock> CreateStock(Stock data)
        {
            try
            {
                _repoStock.Add(data);
                await _repoStock.SaveAll();
                return data;
            }
            catch (Exception)
            {

                return data;
            }
        }
        private async Task<ChemicalDto> AddInk(ChemicalForImportExcelDto chemicalDto)
        {
            var result = new ChemicalDto();

            using (var scope = new TransactionScope(TransactionScopeOption.Required,
             new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {

                var supname = await _repoSup.FindAll(x => x.Name.ToUpper().Equals(chemicalDto.Supplier.ToUpper())).FirstOrDefaultAsync();
                if (supname != null)
                {
                    result.SupplierID = supname.ID;
                }
                

                var process = await _repoProcess.FindAll(x => x.Name.ToUpper().Equals(chemicalDto.Process.ToUpper())).FirstOrDefaultAsync();
                if (process != null)
                {
                    result.ProcessID = process.ID;
                }
               

                // result.CreatedBy = inkDto.CreatedBy;
                scope.Complete();
                return result;
            }
        }
        public async Task ImportExcel(List<ChemicalForImportExcelDto> chemicalDtos)
        {
            try
            {
                var list = new List<ChemicalForImportExcelDto>();
                var listChuaAdd = new List<ChemicalForImportExcelDto>();
                var result = chemicalDtos.DistinctBy(x => new
                {
                    x.Name,
                    x.Supplier,
                    x.MaterialNO,
                    x.Code,
                    x.Process
                }).Where(x => x.Name != "").ToList();

                foreach (var item in result)
                {
                    var supId = 0;
                    var processId = 0;

                    var process = await _repoProcess.FindAll(x => x.Name.ToUpper().Equals(item.Process.ToUpper())).FirstOrDefaultAsync();
                    if (process != null)
                    {
                        //item.ProcessID = process.ID;
                        processId = process.ID;

                        var supname = await _repoSup.FindAll(x => x.Name.ToUpper().Equals(item.Supplier.ToUpper()) && x.ProcessID == processId).FirstOrDefaultAsync();
                        if (supname != null)
                        {

                            //item.SupplierID = supname.ID;
                            supId = supname.ID;
                        }
                        else
                        {
                            var supAdd = new Supplier
                            {
                                Name = item.Supplier,
                                isShow = true,
                                ProcessID = processId
                            };

                            _repoSup.Add(supAdd);
                            await _repoSup.SaveAll();

                            supId = supAdd.ID;
                        }
                    }
                    else
                    {
                        var proAdd = new Process
                        {
                            Name = item.Process
                        };
                        _repoProcess.Add(proAdd);
                        await _repoProcess.SaveAll();

                        processId = proAdd.ID;

                        var supname = await _repoSup.FindAll(x => x.Name.ToUpper().Equals(item.Supplier.ToUpper()) && x.ProcessID == processId).FirstOrDefaultAsync();
                        if (supname != null)
                        {

                            //item.SupplierID = supname.ID;
                            supId = supname.ID;
                        }
                        else
                        {
                            var supAdd = new Supplier
                            {
                                Name = item.Supplier,
                                isShow = true,
                                ProcessID = processId
                            };

                            _repoSup.Add(supAdd);
                            await _repoSup.SaveAll();

                            supId = supAdd.ID;
                        }

                    }
                    item.SupplierID = supId;
                    item.ProcessID = processId;
                    // var ink = await AddInk(item);
                    list.Add(item);
                }

                var listAdd = new List<Chemical>();
                foreach (var ink in list)
                {
                    if (!await CheckExistInk(ink))
                    {
                        var inks = new Chemical();
                        inks.SupplierID = ink.SupplierID;
                        inks.Code = ink.Code;
                        inks.Name = ink.Name;
                        inks.ProcessID = ink.ProcessID;
                        inks.MaterialNO = ink.MaterialNO;
                        inks.Unit = ink.Units;
                        inks.CreatedDate = ink.CreatedDate;
                        inks.CreatedBy = ink.CreatedBy;
                        inks.DaysToExpiration = ink.DaysToExpiration;
                        inks.isShow = true;
                        inks.VOC = ink.VOC;
                        inks.Modify = ink.Modify;
                        if (ink.Modify == false)
                        {
                            inks.Percentage = 100;
                        }
                        _repoChemical.Add(inks);
                        await _repoChemical.SaveAll();
                        listAdd.Add(inks);
                    }
                    else
                    {
                        listChuaAdd.Add(ink);
                    }
                }
                var result1 = listAdd.Where(x => x.ID > 0).ToList();
                var result2 = listAdd.Where(x => x.ID == 0).ToList();
            }
            catch
            {
                throw;
            }
        }

        private async Task<bool> CheckExistInk(ChemicalForImportExcelDto ink)
        {
            return await _repoChemical.FindAll().AnyAsync(x => 
            x.Name == ink.Name 
            && x.Code == ink.Code 
            && x.ProcessID == ink.ProcessID
            && x.MaterialNO == ink.MaterialNO
            && x.SupplierID == ink.SupplierID
            );
        }

        public async Task<bool> AddRangeAsync(List<ChemicalForImportExcelDto> model)
        {
            var ingredients = _mapper.Map<List<Chemical>>(model);
            ingredients.ForEach(ingredient => { ingredient.isShow = true; });
            _repoChemical.AddRange(ingredients);
            return await _repoInk.SaveAll();
        }

        public async Task<bool> Add(ChemicalDto model)
        {
            var chemical = _mapper.Map<Chemical>(model);
            if (model.Modify == false)
            {
                chemical.Percentage = 100;
            }
            chemical.isShow = true;
            chemical.CreatedDate = DateTime.Now;
            _repoChemical.Add(chemical);
            return await _repoChemical.SaveAll();
        }

        public async Task<bool> Delete(object id)
        {
            var chemical = _repoChemical.FindById(id);
            _repoChemical.Remove(chemical);
            return await _repoChemical.SaveAll();
            // throw new NotImplementedException();
        }

        public async Task<List<ChemicalDto>> GetAllAsync()
        {
            // ProjectTo<InkDto>(_configMapper)
            return await _repoChemical.FindAll().Include(x => x.Supplier).Include(x => x.Processes).Select(x => new ChemicalDto
            {
                ID = x.ID,
                Code = x.Code,
                Name = x.Name,
                NameEn = x.NameEn,
                Color = x.Processes.Color,
                CreatedDate = x.CreatedDate,
                MaterialNO = x.MaterialNO,
                VOC = x.VOC,
                Unit = x.Unit,
                Supplier = x.Supplier.Name,
                Process = x.Processes.Name,
                DaysToExpiration = x.DaysToExpiration,
                Modify = x.Modify,
                SupplierID = x.SupplierID,
                ProcessID = x.ProcessID,

            }).OrderByDescending(x => x.ID).ToListAsync();
        }
        public async Task<object> GetChemicalBySupplier(int id)
        {
            // throw new NotImplementedException();
            var list = new List<InkChemicalDto>();
            var inkmodel = await _repoInk.FindAll(x => x.SupplierID == id).Select(x => new InkChemicalDto
            {
                ID = x.ID,
                Name = x.Name,
                Subname = "Ink",
                percentage = x.percentage,
                modify = true,
                Code = x.Code

            }).ToListAsync();
            list.AddRange(inkmodel);
           
            var chemicalmodel = await _repoChemical.FindAll(x => x.SupplierID == id).Select(x => new InkChemicalDto
            {
                ID = x.ID,
                Name = x.Name,
                Subname = "Chemical",
                percentage = x.Percentage,
                modify = x.Modify,
                Code = x.Code
            }).ToListAsync();
            list.AddRange(chemicalmodel);
          
            return list;
        }

        public async Task<object> GetAllInkChemical()
        {
            var list = new List<InkChemicalDto>();
            var inkmodel = await _repoInk.FindAll().Select(x => new InkChemicalDto
            {
                ID = x.ID,
                Name = x.Name,
                Subname = "Ink",
                percentage = x.percentage,
                modify = true,
                Code = x.Code,
                MaterialNo = x.MaterialNO

            }).ToListAsync();
            list.AddRange(inkmodel);

            var chemicalmodel = await _repoChemical.FindAll().Select(x => new InkChemicalDto
            {
                ID = x.ID,
                Name = x.Name,
                Subname = "Chemical",
                percentage = x.Percentage,
                modify = x.Modify,
                Code = x.Code,
                MaterialNo = x.MaterialNO
            }).ToListAsync();
            list.AddRange(chemicalmodel);
            return list;
        }
       

        public Task<PagedList<InkDto>> GetWithPaginations(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<InkDto>> Search(PaginationParams param, object text)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(InkDto model)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(ChemicalUpdateDto model)
        {
            var chemical = _mapper.Map<Chemical>(model);
            chemical.CreatedDate = DateTime.Now;
            _repoChemical.Update(chemical);
            return await _repoChemical.SaveAll();
        }

        Task<PagedList<ChemicalDto>> IECService<ChemicalDto>.GetWithPaginations(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        Task<PagedList<ChemicalDto>> IECService<ChemicalDto>.Search(PaginationParams param, object text)
        {
            throw new NotImplementedException();
        }

       

        public Task<bool> Update(ChemicalDto model)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Deletes(int id)
        {
            var item = _repoStock.FindById(id);
            _repoStock.Remove(item);
            return await _repoStock.SaveAll();
        }

        public ChemicalDto GetById(object id)
        {
           return _mapper.Map<Chemical, ChemicalDto>(_repoChemical.FindById(id));
            
        }
    }
}