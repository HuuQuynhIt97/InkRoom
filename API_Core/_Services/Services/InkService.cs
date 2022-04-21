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

namespace INK_API._Services.Services
{
    public class InkService : IInkService
    {
        private readonly IProcessRepository _repoProcess;
        private readonly ISupplierRepository _repoSup;
        private readonly IInkRepository _repoInk;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public InkService(IProcessRepository repoProcess , ISupplierRepository repoSup ,IInkRepository repoInk , IMapper mapper, MapperConfiguration configMapper)
        {
            _configMapper = configMapper;
            _mapper = mapper;
            _repoInk = repoInk;
            _repoSup = repoSup;
            _repoProcess = repoProcess;

        }

        private async Task<InkDto> AddInk(InkForImportExcelDto inkDto)
        {
            var result = new InkDto();
          
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
             new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
            
                var supname = await _repoSup.FindAll(x => x.Name.ToUpper().Equals(inkDto.Supplier.ToUpper())).FirstOrDefaultAsync();
                if (supname != null)
                {
                    result.SupplierID = supname.ID;
                }
                
        
                var process = await _repoProcess.FindAll(x => x.Name.ToUpper().Equals(inkDto.Process.ToUpper())).FirstOrDefaultAsync();
                if (process != null)
                {
                    result.ProcessID = process.ID;
                }

                // result.CreatedBy = inkDto.CreatedBy;
                scope.Complete();
                return result;
            }
        }
        public async Task ImportExcel(List<InkForImportExcelDto> inkDtos)
        {
            try
            {
                var list = new List<InkForImportExcelDto>();
                var listChuaAdd = new List<InkForImportExcelDto>();
                var result = inkDtos.DistinctBy(x => new
                {
                    x.Name,
                    x.Supplier,
                    x.Code,
                    x.MaterialNO,
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
                        } else
                        {
                            var supAdd = new Supplier
                            {
                                Name = item.Supplier,
                                ProcessID = processId,
                                isShow = true
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
                                ProcessID = processId,
                                isShow = true
                            };

                            _repoSup.Add(supAdd);
                            await _repoSup.SaveAll();

                            supId = supAdd.ID;
                        }

                    }
                    item.SupplierID = supId;
                    item.ProcessID = processId;
                   
                    list.Add(item);
                }

                var listAdd = new List<Ink>();
                foreach (var ink in list)
                {
                    if (!await CheckExistInk(ink))
                    {
                        var inks = new Ink();
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
                        _repoInk.Add(inks);
                        await _repoInk.SaveAll();
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

        private async Task<bool> CheckExistInk(InkForImportExcelDto ink)
        {
            return await _repoInk.FindAll().AnyAsync(x =>
            x.Name == ink.Name
            && x.Code == ink.Code
            && x.ProcessID == ink.ProcessID
            && x.MaterialNO == ink.MaterialNO
            && x.SupplierID == ink.SupplierID
            );
        }
        public async Task<bool> AddRangeAsync(List<InkForImportExcelDto> model)
        {
            var ingredients = _mapper.Map<List<Ink>>(model);
            ingredients.ForEach(ingredient => { ingredient.isShow = true; });
            _repoInk.AddRange(ingredients);
            return await _repoInk.SaveAll();
        }
        public async Task<bool> Add(InkDto model)
        {
            var ink = _mapper.Map<Ink>(model);
            ink.isShow = true;
            ink.CreatedDate = DateTime.Now;
            _repoInk.Add(ink);
            return await _repoInk.SaveAll();
        }

        public async Task<bool> Delete(object id)
        {
            var ink = _repoInk.FindById(id);
            _repoInk.Remove(ink);
            return await _repoInk.SaveAll();
        }

        public async Task<List<InkDto>> GetAllAsync()
        {
            // ProjectTo<InkDto>(_configMapper)
            return await _repoInk.FindAll().Include(x => x.Supplier).Include(x => x.Processes).Select(x => new InkDto {
                ID = x.ID,
                Code = x.Code,
                Name = x.Name,
                NameEn = x.NameEn,
                CreatedDate = x.CreatedDate,
                MaterialNO = x.MaterialNO,
                VOC = x.VOC,
                Unit = x.Unit,
                Color = x.Processes.Color,
                Supplier = x.Supplier.Name,
                Process = x.Processes.Name,
                DaysToExpiration = x.DaysToExpiration,
                SupplierID = x.SupplierID,
                ProcessID = x.ProcessID,
                Percentage = x.percentage,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate
            }).OrderByDescending(x => x.ID).ToListAsync();
        }

        public async Task<List<InkDto>> GetAllWithLocale(string lang)
        {
            // ProjectTo<InkDto>(_configMapper)
            return await _repoInk.FindAll().Include(x => x.Supplier).Include(x => x.Processes).Select(x => new InkDto
            {
                ID = x.ID,
                Code = x.Code,
                Name = x.Name,
                NameEn = x.NameEn,
                CreatedDate = x.CreatedDate,
                MaterialNO = x.MaterialNO,
                VOC = x.VOC,
                Unit = x.Unit,
                Color = x.Processes.Color,
                Supplier = x.Supplier.Name,
                Process = x.Processes.Name,
                DaysToExpiration = x.DaysToExpiration,
                SupplierID = x.SupplierID,
                ProcessID = x.ProcessID,
                Percentage = x.percentage,
                CreatedBy = x.CreatedBy,
                ModifiedDate = x.ModifiedDate
            }).OrderByDescending(x => x.ID).ToListAsync();
        }

        public InkDto GetById(object id)
        {
           return _mapper.Map<Ink, InkDto>(_repoInk.FindById(id));
        }

        public Task<PagedList<InkDto>> GetWithPaginations(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<InkDto>> Search(PaginationParams param, object text)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(InkUpdateDto model)
        {
            var ink = _mapper.Map<Ink>(model);
            _repoInk.Update(ink);
            return await _repoInk.SaveAll();
        }

        public Task<bool> Update(InkDto model)
        {
            throw new NotImplementedException();
        }

      

    }
}