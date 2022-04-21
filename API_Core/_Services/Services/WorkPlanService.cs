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
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;

namespace INK_API._Services.Services
{
    public class WorkPlanService : IWorkPlanService
    {
        private readonly IGluesRepository _repoGlue;
        private readonly IBuildingRepository _repoBuilding;
        private readonly IInkRepository _repoInk;
        private readonly IChemicalRepository _repoChemical;
        private readonly IPartInkChemicalRepository _repoPartInkChemical;
        private readonly IPoGlueRepository _repoPoGlue;
        private readonly IScheduleUpdateRepository _repoScheduleUpdate;
        private readonly IWorkPlanRepository _repoWorkPlan;
        private readonly IProcessRepository _repoProcess;
        private readonly IGluesRepository _repoGlues;
        private readonly IPartRepository _repoPart;
        private readonly ITreatmentWayRepository _repoTreatmentWay;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public WorkPlanService(
            IGluesRepository repoGlue,
            IBuildingRepository repoBuilding,
            IInkRepository repoInk,
            IGluesRepository repoGlues,
            IPartRepository repoPart,
            ITreatmentWayRepository repoTreatmentWay,
            IPartInkChemicalRepository repoPartInkChemical,
            IChemicalRepository repoChemical,
            IScheduleUpdateRepository repoScheduleUpdate,
            IWorkPlanRepository repoWorkPlan,
            IPoGlueRepository repoPoGlue,
            IProcessRepository repoProcess,
            IMapper mapper,
            MapperConfiguration configMapper)
        {
            _repoGlue = repoGlue;
            _repoBuilding = repoBuilding;
            _repoInk = repoInk;
            _repoPartInkChemical = repoPartInkChemical;
            _repoChemical = repoChemical;
            _repoWorkPlan = repoWorkPlan;
            _repoProcess = repoProcess;
            _repoGlues = repoGlues;
            _repoPart = repoPart;
            _repoTreatmentWay = repoTreatmentWay;
            _repoPoGlue = repoPoGlue;
            _repoScheduleUpdate = repoScheduleUpdate;
            _configMapper = configMapper;
            _mapper = mapper;

        }

        public async Task<byte[]> WorkPlanFailedAdd(List<WorkPlanImportExcel>  model)
        {
            return ExportExcelConsumptionCase1(model);
            //throw new NotImplementedException();
        }
        private Byte[] ExportExcelConsumptionCase1(List<WorkPlanImportExcel> consumtionDtos)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var memoryStream = new MemoryStream();
                using (ExcelPackage p = new ExcelPackage(memoryStream))
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = "Huu Quynh";

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = "WorkPlan";
                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add("WorkPlan");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["WorkPlan"];

                    // đặt tên cho sheet
                    ws.Name = "List Failure to add";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 12;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";
                    var headers = new string[]{
                        "Line", "P0 No.",
                        "Model Name", "Model #", "Article",
                        "Qty.",  "Stitching", "Stock-fitting"
                    };

                    int headerRowIndex = 1;
                    int headerColIndex = 1;
                    foreach (var header in headers)
                    {
                        int col = headerRowIndex++;
                        ws.Cells[headerColIndex, col].Value = header;
                        ws.Cells[headerColIndex, col].Style.Font.Bold = true;
                        ws.Cells[headerColIndex, col].Style.Font.Size = 12;
                    }
                    // end Style
                    int colIndex = 1;
                    int rowIndex = 1;
                    // với mỗi item trong danh sách sẽ ghi trên 1 dòng
                    foreach (var body in consumtionDtos)
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0 #c0514d
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;


                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = body.Line;
                        ws.Cells[rowIndex, colIndex++].Value = body.PONo;
                        ws.Cells[rowIndex, colIndex++].Value = body.ModelName;
                        ws.Cells[rowIndex, colIndex++].Value = body.ModelNo;
                        ws.Cells[rowIndex, colIndex++].Value = body.ArticleNo;
                        ws.Cells[rowIndex, colIndex++].Value = body.Qty;
                        ws.Cells[rowIndex, colIndex++].Value = body.Stitching;
                        ws.Cells[rowIndex, colIndex++].Value = body.Stockfitting;
                    }

                    //make the borders of cell F6 thick
                    ws.Cells[ws.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[ws.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[ws.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[ws.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    foreach (var item in headers.Select((x, i) => new { Value = x, Index = i }))
                    {
                        var col = item.Index + 1;
                        ws.Column(col).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Column(col).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        if (col == 7 || col == 6)
                        {
                            ws.Column(col).AutoFit(10);
                        }
                        else
                        {
                            ws.Column(col).AutoFit();
                        }
                    }
                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    return bin;
                }
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                Console.Write(mes);
                return new Byte[] { };
            }
        }

        public Task<bool> Add(WorkPlanDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetAllAsync()
        {
            var res = await _repoWorkPlan.FindAll().ToListAsync();
            var time_upload = _repoWorkPlan.FindAll().ToList().Count > 0 ? _repoWorkPlan.FindAll().ToList().LastOrDefault().CreatedTime.ToString("yyyy-MM-dd") : "";
            var items = res.GroupBy(x => new
            {
                x.ModelName,
                x.ModelNo,
                x.ArticleNo,
                //x.Treatment,
                //x.Line,
                //x.PONo
            }).Select(x => new
            {
                ID = x.FirstOrDefault().ID,
                ScheduleID = x.Select(a => a.ScheduleID),
                ModelName = x.FirstOrDefault().ModelName,
                ModelNo = x.FirstOrDefault().ModelNo,
                ArticleNo = x.FirstOrDefault().ArticleNo,
                Line = x.FirstOrDefault().Line,
                PONo = x.FirstOrDefault().PONo,
                Qty = x.Sum(a => a.Qty.ToDouble()),
                Stitching = x.FirstOrDefault().Stitching,
                Stockfitting = x.FirstOrDefault().Stockfitting,
                CreatedDate = x.FirstOrDefault().CreatedDate,
                CreatedTime = x.FirstOrDefault().CreatedTime.ToString("yyyy-MM-dd"),
                UploadDate = x.FirstOrDefault().CreatedTime.ToString("yyyy-MM"),
            }).OrderBy(x => x.ID);
            var data = items.Select(x => new
            {
                ID = x.ID,
                ModelName = x.ModelName,
                ModelNo = x.ModelNo,
                ArticleNo = x.ArticleNo,
                Line = x.Line,
                PONo = x.PONo,
                Treatment = _repoScheduleUpdate.FindAll(a => x.ScheduleID.Contains(a.ID)).Select(x => new TreatmentWorkPlanDto {
                    ID = x.ID,
                    Treatment = x.Treatment,
                    Status = _repoGlues.FindAll().Where(b => b.ScheduleID == x.ID).ToList().Count > 0 ? true : false,
                    FinishedStatus = _repoWorkPlan.FindAll().Where(b => b.ScheduleID == x.ID).All(b => b.Status == true) ? true : false,
                    Color = _repoProcess.FindAll().Where(y => y.Name == x.Treatment).FirstOrDefault().Color
                }),
                Qty = x.Qty,
                Stitching = x.Stitching,
                Stockfitting = x.Stockfitting,
                CreatedDate = x.CreatedDate,
                CreatedTime = x.CreatedTime,
                UploadDate = x.UploadDate,
            });
            var result = data.Select(x => new
            {
                ID = x.ID,
                ModelName = x.ModelName,
                ModelNo = x.ModelNo,
                ArticleNo = x.ArticleNo,
                Line = x.Line,
                PONo = x.PONo,
                Treatment = x.Treatment,
                Status = x.Treatment.ToList().All(b => b.Status == true) ? true : false,
                Qty = x.Qty,
                Stitching = x.Stitching,
                Stockfitting = x.Stockfitting,
                CreatedDate = x.CreatedDate,
                CreatedTime = x.CreatedTime,
                UploadDate = x.UploadDate
            });
            return new
            {
                result = result.ToList(),
                time_upload = time_upload
            };
        }

        public async Task<object> GetAllWorkPlanWithDate(DateTime time)
        {
            var res = await _repoWorkPlan.FindAll().ToListAsync();
            var time_upload = _repoWorkPlan.FindAll(x => x.CreatedTime.Month == time.Month && x.CreatedTime.Year == time.Year).ToList().Count > 0 ? 
                _repoWorkPlan.FindAll(x => x.CreatedTime.Month == time.Month && x.CreatedTime.Year == time.Year).ToList().LastOrDefault().CreatedTime.ToString("yyyy-MM-dd") : "";
            var items = res.GroupBy(x => new
            {
                x.ModelName,
                x.ModelNo,
                x.ArticleNo,
                //x.Treatment,
                //x.Line,
                //x.PONo
            }).Select(x => new
            {
                ID = x.FirstOrDefault().ID,
                ScheduleID = x.Select(a => a.ScheduleID),
                ModelName = x.FirstOrDefault().ModelName,
                ModelNo = x.FirstOrDefault().ModelNo,
                ArticleNo = x.FirstOrDefault().ArticleNo,
                Line = x.FirstOrDefault().Line,
                PONo = x.FirstOrDefault().PONo,
                Qty = x.Sum(a => a.Qty.ToDouble()),
                Stitching = x.FirstOrDefault().Stitching,
                Stockfitting = x.FirstOrDefault().Stockfitting,
                CreatedDate = x.FirstOrDefault().CreatedDate,
                CreatedTime = x.FirstOrDefault().CreatedTime,
            }).Where(x => x.CreatedTime.Month == time.Month && x.CreatedTime.Year == time.Year).OrderBy(x => x.ID);
            var data = items.Select(x => new
            {
                ID = x.ID,
                ModelName = x.ModelName,
                ModelNo = x.ModelNo,
                ArticleNo = x.ArticleNo,
                Line = x.Line,
                PONo = x.PONo,
                Treatment = _repoScheduleUpdate.FindAll(a => x.ScheduleID.Contains(a.ID)).Select(x => new TreatmentWorkPlanDto
                {
                    ID = x.ID,
                    Treatment = x.Treatment,
                    Status = _repoGlues.FindAll().Where(b => b.ScheduleID == x.ID).ToList().Count > 0 ? true : false,
                    FinishedStatus = _repoWorkPlan.FindAll().Where(b => b.ScheduleID == x.ID).All(b => b.Status == true) ? true : false,
                    Color = _repoProcess.FindAll().Where(y => y.Name == x.Treatment).FirstOrDefault().Color
                }),
                Qty = x.Qty,
                Stitching = x.Stitching,
                Stockfitting = x.Stockfitting,
                CreatedDate = x.CreatedDate,
                CreatedTime = x.CreatedTime.ToString("yyyy-MM-dd"),
            });
            var result = data.Select(x => new
            {
                ID = x.ID,
                ModelName = x.ModelName,
                ModelNo = x.ModelNo,
                ArticleNo = x.ArticleNo,
                Line = x.Line,
                PONo = x.PONo,
                Treatment = x.Treatment,
                Status = x.Treatment.ToList().All(b => b.Status == true) ? true : false,
                Qty = x.Qty,
                Stitching = x.Stitching,
                Stockfitting = x.Stockfitting,
                CreatedDate = x.CreatedDate,
                CreatedTime = x.CreatedTime
            });
            return new
            {
                result = result.ToList(),
                time_upload = time_upload
            };
        }

        public WorkPlanDTO GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<WorkPlanDTO>> GetWithPaginations(PaginationParams param)
        {
            throw new NotImplementedException();
        }

        private async Task<WorkPlanUpdateDto> AddSchedule(WorkPlanImportExcel scheduleDto)
        {
            var result = new WorkPlanUpdateDto() ;
            
            result.Line = scheduleDto.Line ;
            result.PONo = scheduleDto.PONo ;
            result.ModelName = scheduleDto.ModelName ;
            result.ModelNo = scheduleDto.ModelNo ;
            result.ArticleNo = scheduleDto.ArticleNo ;
            result.Qty = scheduleDto.Qty ;
            result.CreatedTime = scheduleDto.CreatedTime;
            result.Treatment = scheduleDto.Treatment ;
            result.Stitching = scheduleDto.Stitching.ToSafetyString() ;
            result.Stockfitting = scheduleDto.Stockfitting.ToSafetyString() ;
           
            return result ;
        }

        public async Task<object> ImportExcel(List<WorkPlanImportExcel> scheduleDto)
        {
            try
            {
                var list = new List<WorkPlanUpdateDto>();
                
                var result = scheduleDto.DistinctBy(x => new
                {
                    x.ModelName,
                    x.ModelNo,
                    x.ArticleNo,
                    x.Line,
                    x.PONo
                }).Where(x => x.ModelName != "").ToList();

                foreach (var item in result)
                {
                    var Schedule = await AddSchedule(item);
                    list.Add(Schedule);
                }

                var listAdd = new List<WorkPlan>();
                var listChuaAdd = new List<WorkPlanUpdateDto>();
                var listExist = new List<WorkPlanUpdateDto>();
               

                foreach (var x in list)
                {
                    var scheduleID = new List<int>();
                    scheduleID = _repoScheduleUpdate.FindAll(
                        y => y.ModelName == x.ModelName
                        && y.ModelNo == x.ModelNo
                        && y.ArticleNo == x.ArticleNo).Select(x => x.ID).ToList();


                    
                    if (scheduleID.Count > 0 )
                    {
                        foreach (var item_scheudle in scheduleID)
                        {
                            if (!await _repoWorkPlan.FindAll().AnyAsync(y => y.ModelName == x.ModelName && y.ScheduleID == item_scheudle && y.ModelNo == x.ModelNo && y.ArticleNo == x.ArticleNo && y.Line == x.Line && y.PONo == x.PONo))
                            {

                                var listAddWorkPlanMaster = new List<WorkPlanMaster>();
                                var listAddPoGlue = new List<PoGlue>();
                                var listGlue = _repoGlue.FindAll(x => x.ScheduleID == item_scheudle).Select(x => x.ID).ToList();
                                // add schedule
                                var schedules = new WorkPlan();
                                schedules.ModelName = x.ModelName;
                                schedules.ModelNo = x.ModelNo;
                                schedules.ArticleNo = x.ArticleNo;
                                schedules.Qty = x.Qty;
                                schedules.CreatedTime = x.CreatedTime;
                                schedules.Treatment = x.Treatment;
                                schedules.Line = x.Line;
                                schedules.PONo = x.PONo;
                                schedules.ScheduleID = item_scheudle;
                                schedules.Stitching = x.Stitching;
                                schedules.Stockfitting = x.Stockfitting;
                                _repoWorkPlan.Add(schedules);
                                await _repoWorkPlan.SaveAll();
                                listAdd.Add(schedules);
                                
                                var poGlue = new PoGlue();
                                poGlue.ScheduleID = item_scheudle;
                                poGlue.WorkPlanID = schedules.ID;
                                listAddPoGlue.Add(poGlue);

                                _repoPoGlue.AddRange(listAddPoGlue);
                                await _repoPoGlue.SaveAll();
                            } else
                            {
                                listExist.Add(x);
                            }
                        }
                    }
                    else
                    {

                        listChuaAdd.Add(x);
                    }
                }
                var list_added = listAdd.Where(x => x.ID > 0).ToList();
                var list_no_add = listChuaAdd.Where(x => x.ID == 0).ToList();
                return new {
                    Status = true,
                    Total = result.Count,
                    DataExist = listExist.Count,
                    Added = list_added.Count,
                    NoAdd = list_no_add.Count,
                    FailedAdd = listChuaAdd
                };
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public Task<PagedList<WorkPlanDTO>> Search(PaginationParams param, object text)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(WorkPlanDTO model)
        {
            throw new NotImplementedException();
        }

        Task<List<WorkPlanDTO>> IECService<WorkPlanDTO>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetPONumberByScheduleID(int id , string treatment)
        {
            var list = await _repoPoGlue.FindAll(x => x.ScheduleID == id).ToListAsync();
            var data = (from x in list
                        join g in _repoGlue.FindAll() on x.ScheduleID equals g.ScheduleID
                        join w in _repoWorkPlan.FindAll() on x.WorkPlanID equals w.ID
                        join p in _repoPartInkChemical.FindAll() on g.ID equals p.GlueID into P
                        let ps = P.ToList()
                        select new
                        {
                            ID = x.WorkPlanID,
                            Name = w.PONo,
                            Line = w.Line,
                            Qty = w.Qty,
                            Status = w.Status,
                            GlueName = GetNamePartInkChemical(ps, Math.Round(w.Qty.ToDouble(), 2)),
                            Treatment = w.Treatment
                        });
            var result = data.GroupBy(x => new { x.ID })
                .Select(x => new
                {
                    ID = x.First().ID,
                    Name = x.First().Name,
                    Line = x.First().Line,
                    Qty = x.First().Qty,
                    Status = x.First().Status,
                    GlueName = String.Join(" / ", x.Select(a => a.GlueName)),
                    Treatment = x.First().Treatment
                });
            return result;
        }
        public async Task<object> GetPONumberByScheduleIDAndPart(int id, string treatment, int partId)
        {
            var list = await _repoPoGlue.FindAll(x => x.ScheduleID == id).ToListAsync();
            var data = (from x in list
                        join g in _repoGlue.FindAll() on x.ScheduleID equals g.ScheduleID
                        join w in _repoWorkPlan.FindAll() on x.WorkPlanID equals w.ID
                        join p in _repoPartInkChemical.FindAll() on g.ID equals p.GlueID into P
                        let ps = P.ToList()
                        select new
                        {
                            ID = x.WorkPlanID,
                            PartID = x.PartID == partId ? x.PartID : partId,
                            Name = w.PONo,
                            Line = w.Line,
                            Qty = w.Qty,
                            Ps = ps,
                            Status = w.Status,
                            PartStatus = x.PartID == partId   ? x.Status : false,
                            GlueName = GetNamePartInkChemical(ps, Math.Round(w.Qty.ToDouble(), 2)),
                            Treatment = w.Treatment
                        });
            var result = data.DistinctBy(x => x.GlueName).GroupBy(x => new { x.ID })
                .Select(x => new
                {
                    ID = x.First().ID,
                    PartID = x.First().PartID,
                    Name = x.First().Name,
                    Line = x.First().Line,
                    Qty = x.First().Qty,
                    Ps = x.First().Ps,
                    Status = x.First().Status,
                    PartStatus = x.First().PartStatus,
                    GlueName = String.Join(" / ", x.Select(a => a.GlueName)),
                    Treatment = x.First().Treatment
                });
            return result;
        }
        public async Task<object> GetPONumberByScheduleIDAndPart2(int id, string treatment, int partId)
        {
            var list = await _repoPoGlue.FindAll(x => x.ScheduleID == id && x.PartID == partId).ToListAsync();
            var data = (from x in list
                        join g in _repoGlue.FindAll() on x.ScheduleID equals g.ScheduleID
                        join w in _repoWorkPlan.FindAll() on x.WorkPlanID equals w.ID
                        join p in _repoPartInkChemical.FindAll() on g.ID equals p.GlueID into P
                        let ps = P.ToList()
                        select new
                        {
                            ID = x.WorkPlanID,
                            PartID = x.PartID,
                            Name = w.PONo,
                            Line = w.Line,
                            Ps = ps,
                            Qty = w.Qty,
                            Status = w.Status,
                            PartStatus = x.Status,
                            GlueName = GetNamePartInkChemical(ps, Math.Round(w.Qty.ToDouble(), 2)),
                            Treatment = w.Treatment
                        });
            var result = data.DistinctBy(x => x.GlueName).GroupBy(x => new { x.ID })
                .Select(x => new
                {
                    ID = x.First().ID,
                    PartID = x.First().PartID,
                    Name = x.First().Name,
                    Line = x.First().Line,
                    Qty = x.First().Qty,
                    Ps = x.First().Ps,
                    Status = x.First().Status,
                    PartStatus = x.First().PartStatus,
                    GlueName = String.Join(" / ", x.Select(a => a.GlueName)),
                    Treatment = x.First().Treatment
                });
            return result;
        }
        public async Task<object> GetParticularBySchedule(int id, string treatment)
        {
            var list = await _repoPoGlue.FindAll(x => x.ScheduleID == id).ToListAsync();
            var data = (from x in list
                        join g in _repoGlue.FindAll() on x.ScheduleID equals g.ScheduleID
                        join w in _repoWorkPlan.FindAll() on x.WorkPlanID equals w.ID
                        join p in _repoPartInkChemical.FindAll() on g.ID equals p.GlueID into P
                        let ps = P.ToList()
                        select new
                        {
                            ID = x.WorkPlanID,
                            GlueName = GetNamePartInkChemical(ps, Math.Round(w.Qty.ToDouble(), 2)),
                            Treatment = w.Treatment
                        }).Where(x => x.Treatment == treatment);
            var result = data.GroupBy(x => new { x.ID })
                .Select(x => new
                {
                    ID = x.First().ID,
                    GlueName = String.Join(" / ", x.Select(a => a.GlueName)),
                    Treatment = x.First().Treatment
                });
            return result;
        }
       
        private string GetNamePartInkChemical(List<PartInkChemical> model, double qty)
        {
            var text = new List<string>();
            foreach (var item in model)
            {
                if (item.InkID > 0)
                {
                    var checkValue = _repoInk.FindById(item.InkID);
                    if (checkValue != null)
                    {
                        var calculator = (qty * (item.Consumption != null ? item.Consumption : 0)).ToDouble();
                        if (calculator > 0)
                        {
                            string s = Math.Round(calculator, 2) + "g " + checkValue.Code;
                            text.Add(s);
                        }
                        else
                        {
                            string s = checkValue.Code;
                            text.Add(s);
                        }
                    }
                }

                if (item.ChemicalID > 0)
                {
                    var checkValue = _repoChemical.FindById(item.ChemicalID);
                    if (checkValue != null)
                    {
                        var calculator = (qty * (item.Consumption != null ? item.Consumption : 0)).ToDouble();
                        if (calculator > 0)
                        {
                            string s = Math.Round(calculator, 2) + "g " + checkValue.Code;
                            text.Add(s);

                        }
                        else
                        {
                            string s = checkValue.Code;
                            text.Add(s);
                        }
                    }
                }
            }
            var textResult = String.Join(" + ", text);
            return textResult;
        }

        private string GetNamePartInkChemicalNoQty(List<PartInkChemical> model, double qty)
        {
            var text = new List<string>();
            foreach (var item in model)
            {
                if (item.InkID > 0)
                {
                    var checkValue = _repoInk.FindById(item.InkID);
                    if (checkValue != null)
                    {
                        var calculator = (qty * (item.Consumption != null ? item.Consumption : 0)).ToDouble();
                        if (calculator > 0)
                        {
                            string s = calculator + "g " + checkValue.Code;
                            text.Add(s);
                        }
                        else
                        {
                            string s = checkValue.Code;
                            text.Add(s);
                        }
                    }
                }

                if (item.ChemicalID > 0)
                {
                    var checkValue = _repoChemical.FindById(item.ChemicalID);
                    if (checkValue != null)
                    {
                        var calculator = (qty * (item.Consumption != null ? item.Consumption : 0)).ToDouble();
                        if (calculator > 0)
                        {
                            string s = calculator + "g " + checkValue.Code;
                            text.Add(s);

                        }
                        else
                        {
                            string s = checkValue.Code;
                            text.Add(s);
                        }
                    }
                }
            }
            var textResult = String.Join(" + ", text);
            return textResult;
        }

        private double GetTotalInkChemical(List<PartInkChemical> model, double qty)
        {
            var total = new List<double>();
            foreach (var item in model)
            {
                if (item.InkID > 0)
                {
                    var checkValue = _repoInk.FindById(item.InkID);
                    if (checkValue != null)
                    {
                        var calculator = (qty * (item.Consumption != null ? item.Consumption : 0)).ToDouble();
                        total.Add(calculator);
                    }
                }

                if (item.ChemicalID > 0)
                {
                    var checkValue = _repoChemical.FindById(item.ChemicalID);
                    if (checkValue != null)
                    {
                        var calculator = (qty * (item.Consumption != null ? item.Consumption : 0)).ToDouble();
                        total.Add(calculator);
                    }
                }
            }
            var result = total.Sum(x => x);
            return result;
        }

        private double GetTotalInk(List<PartInkChemical> model)
        {
            var total = new List<double>();
            foreach (var item in model)
            {
                if (item.InkID > 0)
                {
                    var checkValue = _repoInk.FindById(item.InkID);
                    if (checkValue != null)
                    {
                        var calculator = item.Consumption != null ? item.Consumption.ToDouble() : 0;
                        total.Add(calculator);
                    }
                }
            }
            var result = total.Sum(x => x);
            return result;
        }

        private object GetItemOfInk(int partID, int qty)
        {
            var list = _repoPartInkChemical.FindAll(x => x.PartID == partID && x.InkID > 0).ToList();
            var data = from x in list
                         join y in _repoInk.FindAll() on x.InkID equals y.ID
                         select new
                         {
                             Name = y.Code,
                             Consumption = x.Consumption,
                             Percentage = x.Percentage
                         };
            var result = data.GroupBy(x => x.Name).Select(x => new
            {
                Name = x.First().Name,
                Consumption = Math.Round(x.Sum(x => (x.Consumption.ToDouble() * qty) * (x.Percentage / 100)), 2)
            });
            return result;
        }

        private object GetItemOfChemical(int partID, int qty)
        {
            var list = _repoPartInkChemical.FindAll(x => x.PartID == partID && x.ChemicalID > 0).ToList();
            var data = from x in list
                       join y in _repoChemical.FindAll() on x.ChemicalID equals y.ID
                       select new
                       {
                           Name = y.Code,
                           Consumption = x.Consumption,
                           Percentage = x.Percentage
                       };
            var result = data.GroupBy(x => x.Name).Select(x => new
            {
                Name = x.First().Name,
                Consumption = Math.Round(x.Sum(x => (x.Consumption.ToDouble() * qty) * (x.Percentage / 100)), 2)
            });
            return result;
        }

        private double GetTotalChemical(List<PartInkChemical> model)
        {
            var total = new List<double>();
            foreach (var item in model)
            {
                if (item.ChemicalID > 0)
                {
                    var checkValue = _repoChemical.FindById(item.ChemicalID);
                    if (checkValue != null)
                    {
                        var calculator = item.Consumption != null ? item.Consumption.ToDouble() : 0;
                        total.Add(calculator);
                    }
                }
            }
            var result = total.Sum(x => x);
            return result;
        }

        private string getBuildingName(int id)
        {
            var line = _repoWorkPlan.FindAll(x => x.ScheduleID == id).FirstOrDefault().Line;
            var building_ID = _repoBuilding.FindAll(x => x.Name == line).FirstOrDefault().ParentID;
            var building_Name = _repoBuilding.FindById(building_ID).Name;
            return building_Name;
        }

        private string GetRecipeInk(List<PartInkChemical> model)
        {
            var text = new List<string>();
            foreach (var item in model)
            {
                if (item.InkID > 0)
                {
                    var checkValue = _repoInk.FindById(item.InkID);
                    if (checkValue != null)
                    {
                        
                        string s = checkValue.Code;
                        text.Add(s);
                    }
                }
            }
            var textResult = String.Join(" + ", text);
            return textResult;
        }

        public async Task<bool> UpdatePoGlue(int workPlanID)
        {
            var item = _repoWorkPlan.FindAll(x => x.ID == workPlanID).FirstOrDefault();
            item.Status = !item.Status;
            _repoWorkPlan.Update(item);
            return await _repoWorkPlan.SaveAll();
        }

        public async Task<bool> UpdatePart(int workPlanID, int partID)
        {
            var item = _repoPoGlue.FindAll(x => x.WorkPlanID == workPlanID).FirstOrDefault();
            if (item.PartID == null)
            {
                var item_update = _repoPoGlue.FindAll(x => x.WorkPlanID == workPlanID).FirstOrDefault();
                item_update.PartID = partID;
                item.Status = !item.Status;
            } else
            {
                var item_update = _repoPoGlue.FindAll(x => x.WorkPlanID == workPlanID && x.PartID == partID).FirstOrDefault();
                if (item_update != null)
                {
                    item_update.Status = !item_update.Status;
                    _repoPoGlue.Update(item_update);
                } else
                {
                    var poGlue = new PoGlue();
                    poGlue.ScheduleID = item.ScheduleID;
                    poGlue.WorkPlanID = workPlanID;
                    poGlue.Status = true;
                    poGlue.PartID = partID;
                    _repoPoGlue.Add(poGlue);
                }

            }
            
            return await _repoPoGlue.SaveAll();
        }

        public async Task<object> GetPrintQRcodeByWorklan(int workplanID)
        {
            var list = await _repoPoGlue.FindAll(x => x.WorkPlanID == workplanID).ToListAsync();
            var data = (from x in list
                        join g in _repoGlue.FindAll() on x.ScheduleID equals g.ScheduleID
                        join w in _repoWorkPlan.FindAll() on x.WorkPlanID equals w.ID
                        join p in _repoPartInkChemical.FindAll() on g.ID equals p.GlueID into P
                        let ps = P.ToList()
                        select new
                        {
                            ID = x.WorkPlanID,
                            Article = w.ArticleNo,
                            Qty = GetTotalInkChemical(ps, Math.Round(w.Qty.ToDouble(), 2)),
                            Recipe = GetRecipeInk(ps),
                        });
            var result_groupBy = data.GroupBy(x => new { x.ID })
                .Select(x => new
                {
                    ID = x.First().ID,
                    Article = x.First().Article,
                    Qty = x.Sum(x => x.Qty) / 1000,
                    Recipe = String.Join(" / ", x.Where(a => !String.IsNullOrEmpty(a.Recipe)).Select(a => a.Recipe)),
                });
            var result = new
            {
                Qty = result_groupBy.First().Qty,
                Article = result_groupBy.First().Article,
                Recipe = result_groupBy.First().Recipe
            };
            return result;
        }

        public async Task<object> GetPrintQRcodeBySchedule(int scheduleID)
        {
            var list = await _repoPoGlue.FindAll(x => x.ScheduleID == scheduleID).ToListAsync();
            var data = (from x in list
                        join g in _repoGlue.FindAll() on x.ScheduleID equals g.ScheduleID
                        join w in _repoWorkPlan.FindAll() on x.WorkPlanID equals w.ID
                        join p in _repoPartInkChemical.FindAll() on g.ID equals p.GlueID into P
                        let ps = P.ToList()
                        join q in _repoPart.FindAll() on P.First().PartID equals q.ID
                        
                        select new
                        {
                            ID = x.WorkPlanID,
                            ModelName = w.ModelName,
                            Article = w.ArticleNo,
                            Part = q.Name,
                            Qty = GetTotalInkChemical(ps, Math.Round(w.Qty.ToDouble(), 2)),
                            Recipe = GetRecipeInk(ps),
                        });
            var result_groupBy = data.GroupBy(x => new { x.ID })
                .Select(x => new
                {
                    ID = x.First().ID,
                    Article = x.First().Article,
                    Part = x.First().Part,
                    ModelName = x.First().ModelName,
                    Qty = Math.Round(x.Sum(x => x.Qty) / 1000 , 2),
                    Recipe = String.Join(" / ", x.Where(a => !String.IsNullOrEmpty(a.Recipe)).Select(a => a.Recipe)),
                });
            try
            {
                var result = new
                {
                    Qty = result_groupBy.First().Qty,
                    Part = result_groupBy.First().Part,
                    ModelName = result_groupBy.First().ModelName,
                    Article = result_groupBy.First().Article,
                    Recipe = result_groupBy.First().Recipe
                };
                return result;
            }
            catch (Exception)
            {
                var result = new
                {
                    Qty = 0,
                    Article = "N/A",
                    Recipe = "N/A"
                };
                return result;
                throw;
            }
           
        }

        public async Task<object> GetPrintQRcodeByGlueId(int glueId)
        {
            var data = await _repoGlues.FindAll(x => x.ID == glueId).ToListAsync();

            return data;            
        }

        public async Task<object> GetGluesByScheduleId(int id)
        {
            var lists = await _repoGlues.FindAll(x => x.ScheduleID == id).OrderBy(x => x.Sequence).ToListAsync();
            var list = from a in lists
                       join p in _repoPartInkChemical.FindAll() on a.ID equals p.GlueID into P
                       let ps = P.ToList()
                       join b in _repoPart.FindAll() on a.PartID equals b.ID
                       join c in _repoTreatmentWay.FindAll() on a.TreatmentWayID equals c.ID
                       select new
                       {
                           ID = a.ID,
                           Name = a.Name,
                           Recipe = GetRecipeInk(ps),
                           PartID = a.PartID,
                           TreatmentWayID = a.TreatmentWayID,
                           CreatedDate = a.CreatedDate,
                           ScheduleID = a.ScheduleID,
                           TotalInk = GetTotalInk(ps),
                           TotalChemical = GetTotalChemical(ps),
                           Sequence = a.Sequence.ToInt(),
                           Consumption = a.Consumption,
                           Part = b.Name,
                           TreatmentWay = c.Name
                       };

            var result = list.GroupBy(x => x.Part).Select(x => new
            {
                Part = x.First().Part,
                Recipe = String.Join(" / ", x.Where(a => !String.IsNullOrEmpty(a.Recipe)).Select(a => a.Recipe)),
                TreatmentWay = String.Join(" / ", x.Select(a => a.TreatmentWay)),
                Name = String.Join(" / ", x.Select(a => a.Name)),
                Consumption = Math.Round(x.Sum(x => x.Consumption.ToDouble()), 2),
                TotalInk = Math.Round(x.Sum(x => x.TotalInk.ToDouble()), 2),
                TotalChemical = Math.Round(x.Sum(x => x.TotalChemical.ToDouble()), 2)
            });
            return result.ToList();
        }

        public async Task<object> GetGluesByScheduleIdWithQty(int id, int qty)
        {
            var lists = await _repoGlues.FindAll(x => x.ScheduleID == id).OrderBy(x => x.Sequence).ToListAsync();
            var list = from a in lists
                       join p in _repoPartInkChemical.FindAll() on a.ID equals p.GlueID into P
                       let ps = P.ToList()
                       join b in _repoPart.FindAll() on a.PartID equals b.ID
                       join c in _repoTreatmentWay.FindAll() on a.TreatmentWayID equals c.ID
                       select new
                       {
                           ID = a.ID,
                           Name = a.Name,
                           Recipe = GetRecipeInk(ps),
                           PartID = a.PartID,
                           TreatmentWayID = a.TreatmentWayID,
                           CreatedDate = a.CreatedDate,
                           ScheduleID = a.ScheduleID,
                           TotalInk = GetTotalInk(ps),
                           TotalChemical = GetTotalChemical(ps),
                           ItemOfInk = GetItemOfInk(a.PartID, qty),
                           ItemOfChemical = GetItemOfChemical(a.PartID, qty),
                           Sequence = a.Sequence.ToInt(),
                           Consumption = a.Consumption,
                           Part = b.Name,
                           TreatmentWay = c.Name
                       };

            var result = list.GroupBy(x => x.Part).Select(x => new
            {
                Part = x.First().Part,
                Recipe = String.Join(" / ", x.Where(a => !String.IsNullOrEmpty(a.Recipe)).Select(a => a.Recipe)),
                TreatmentWay = String.Join(" / ", x.Select(a => a.TreatmentWay)),
                Name = String.Join(" / ", x.Select(a => a.Name)),
                Consumption = Math.Round(x.Sum(x => x.Consumption.ToDouble() ), 2) ,
                TotalInk = Math.Round(x.Sum(x => x.TotalInk.ToDouble() * qty), 2),
                TotalChemical = Math.Round(x.Sum(x => x.TotalChemical.ToDouble() * qty), 2) ,
                ItemOfInk = x.First().ItemOfInk,
                ItemOfChemical = x.First().ItemOfChemical,
            });
            return result.ToList();
        }

        public async Task<object> GetGluesByScheduleIdWithQtyWithLocale(int id, int qty, string lang)
        {
            var lists = await _repoGlues.FindAll(x => x.ScheduleID == id).OrderBy(x => x.Sequence).ToListAsync();
            var list = from a in lists
                       join p in _repoPartInkChemical.FindAll() on a.ID equals p.GlueID into P
                       let ps = P.ToList()
                       join b in _repoPart.FindAll() on a.PartID equals b.ID
                       join c in _repoTreatmentWay.FindAll() on a.TreatmentWayID equals c.ID
                       select new
                       {
                           ID = a.ID,
                           Name = a.Name,
                           Recipe = GetRecipeInk(ps),
                           PartID = a.PartID,
                           TreatmentWayID = a.TreatmentWayID,
                           CreatedDate = a.CreatedDate,
                           ScheduleID = a.ScheduleID,
                           TotalInk = GetTotalInk(ps),
                           Building = getBuildingName(id),
                           TotalChemical = GetTotalChemical(ps),
                           ItemOfInk = GetItemOfInk(a.PartID, qty),
                           ItemOfChemical = GetItemOfChemical(a.PartID, qty),
                           Sequence = a.Sequence.ToInt(),
                           Consumption = a.Consumption,
                           Part = b.Name,
                           TreatmentWay = lang == Constants.SystemLang.EN ? c.NameEn != null ? c.NameEn : c.Name : c.Name
                       };

            var result = list.GroupBy(x => x.Part).Select(x => new
            {
                Part = x.First().Part,
                Building = x.First().Building,
                Recipe = String.Join(" / ", x.Where(a => !String.IsNullOrEmpty(a.Recipe)).Select(a => a.Recipe)),
                TreatmentWay = String.Join(" / ", x.Select(a => a.TreatmentWay)),
                Name = String.Join(" / ", x.Select(a => a.Name)),
                Consumption = Math.Round(x.Sum(x => x.Consumption.ToDouble()), 2),
                TotalInk = Math.Round(x.Sum(x => (x.TotalInk.ToDouble() * qty) / 1000), 2),
                TotalChemical = Math.Round(x.Sum(x => (x.TotalChemical.ToDouble() * qty) / 1000), 2),
                ItemOfInk = x.First().ItemOfInk,
                ItemOfChemical = x.First().ItemOfChemical,
            });
            return result.ToList();
        }


    }
}