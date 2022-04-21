using System;
using System.Security.Claims;
using System.Threading.Tasks;
using INK_API.Helpers;
using INK_API._Services.Interface;
using INK_API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace INK_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WorkPlanController : ControllerBase
    {

        private readonly IPartService _partService;
        private readonly IChemicalService _chemicalService;
        private readonly IWorkPlanService _workPlanService;
        private readonly IBuildingService _buildingService;
        public WorkPlanController(IBuildingService buildingService, IWorkPlanService workPlanService, IPartService partService, IChemicalService chemicalService)
        {
            _partService = partService;
            _chemicalService = chemicalService;
            _workPlanService = workPlanService;
            _buildingService = buildingService;
        }


        [HttpPost]
        public async Task<IActionResult> WorkPlanFailedAddExport(List<WorkPlanImportExcel> model)
        {

            var bin = await _workPlanService.WorkPlanFailedAdd(model);
            return File(bin, "application/octet-stream", "reportConsumption1.xlsx");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrintQRcodeByWorklan(int id)
        {
            var detail = await _workPlanService.GetPrintQRcodeByWorklan(id);
            return Ok(detail);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrintQRcodeBySchedule(int id)
        {
            var detail = await _workPlanService.GetPrintQRcodeBySchedule(id);
            return Ok(detail);
        }

        [HttpGet("{id}")]
        public IActionResult GetPrintQRcodeByGlueId(int id)
        {
            var detail = _workPlanService.GetPrintQRcodeByGlueId(id);
            return Ok(detail);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGluesByScheduleId(int id)
        {
            var brands = await _workPlanService.GetGluesByScheduleId(id);
            return Ok(brands);
        }

        [HttpGet("{id}/{qty}")]
        public async Task<IActionResult> GetGluesByScheduleIdWithQty(int id, int qty)
        {
            var brands = await _workPlanService.GetGluesByScheduleIdWithQty(id , qty);
            return Ok(brands);
        }

        [HttpGet("{id}/{qty}/{lang}")]
        public async Task<IActionResult> GetGluesByScheduleIdWithQtyWithLocale(int id, int qty, string lang)
        {
            var brands = await _workPlanService.GetGluesByScheduleIdWithQtyWithLocale(id, qty, lang);
            return Ok(brands);
        }

        [HttpPost("{workPlanID}")]
        public async Task<IActionResult> UpdatePoGlue(int workPlanID)
        {
            return Ok(await _workPlanService.UpdatePoGlue(workPlanID));

        }

        [HttpPost("{workPlanID}/{partID}")]
        public async Task<IActionResult> UpdatePart(int workPlanID, int partID)
        {
            return Ok(await _workPlanService.UpdatePart(workPlanID, partID));

        }

        [HttpGet("{id}/{treatment}")]
        public async Task<IActionResult> GetPONumberByScheduleID(int id , string treatment)
        {
            var brands = await _workPlanService.GetPONumberByScheduleID(id , treatment);
            return Ok(brands);
        }

        [HttpGet("{id}/{treatment}/{partID}")]
        public async Task<IActionResult> GetPONumberByScheduleIDAndPart(int id, string treatment, int partId)
        {
            var brands = await _workPlanService.GetPONumberByScheduleIDAndPart(id, treatment, partId);
            return Ok(brands);
        }

        [HttpGet("{id}/{treatment}/{partID}")]
        public async Task<IActionResult> GetPONumberByScheduleIDAndPart2(int id, string treatment, int partId)
        {
            var brands = await _workPlanService.GetPONumberByScheduleIDAndPart2(id, treatment, partId);
            return Ok(brands);
        }
        [HttpGet("{id}/{treatment}")]
        public async Task<IActionResult> GetParticularBySchedule(int id, string treatment)
        {
            var detail = await _workPlanService.GetParticularBySchedule(id, treatment);
            return Ok(detail);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWorkPlan()
        {
            var workplans = await _workPlanService.GetAllAsync();
            return Ok(workplans);
        }

        [HttpGet("{time}")]
        public async Task<IActionResult> GetAllWorkPlanWithDate(DateTime time)
        {
            var workplans = await _workPlanService.GetAllWorkPlanWithDate(time);
            return Ok(workplans);
        }

        [HttpPost]
        public async Task<ActionResult> Import2([FromForm] IFormFile files)
        {

            IFormFile file = Request.Form.Files["UploadedFile"];
            var time = Request.Form["Time"];
            var dataList = new List<WorkPlanImportExcel>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
            {
              string fileName = file.FileName;
              using (var package = new ExcelPackage(file.OpenReadStream()))
              {
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;
                var allLine = (await _buildingService.GetAllAsync()).Where(x => x.Level == 2).OrderBy(x => x.Name).Select(c => c.Name).ToList();
                //var line = string.Empty;
                for (int rowIterator = 5; rowIterator <= noOfRow; rowIterator++)
                {
                    var line = workSheet.Cells[rowIterator, 1].Value;
                    bool result;

                        // check merger

                        if (workSheet.Cells[rowIterator, 1].Merge)
                        {
                            var cats = workSheet.Cells[rowIterator, 1].Value.ToSafetyString().Replace(" ", "").Trim();
                            if (cats != null)
                            {
                                foreach (var item in allLine)
                                {
                                    result = cats.Contains(item);
                                    if (result)
                                    {
                                        line = item;
                                        break;
                                    }
                                    else
                                    {
                                        line = string.Empty;
                                    }
                                }
                                //neu la merger , thi tang rowIterator len 3 dong
                                rowIterator = rowIterator + 3;
                            }
                            else
                            {
                                line = string.Empty;
                            }
                        }

                    // check line
                    var check = allLine.Contains(line);

                  // get value tu file excel
                  var valueStitchingExcel = workSheet.Cells[rowIterator, 20].Value;
                  var valueobjStockfittingExcel = workSheet.Cells[rowIterator, 24].Value;

                  // format sang date
                  var StitchingTamp = DateTime.FromOADate(valueStitchingExcel.ToDouble()).Date;
                  var StockfittingTamp = DateTime.FromOADate(valueobjStockfittingExcel.ToDouble()).Date;

                  var Stitching = string.Empty;
                  if (valueStitchingExcel != null)
                  {
                    Stitching = StitchingTamp.Month.ToString() + '/' + StitchingTamp.Day.ToString();
                  }

                  var Stockfitting = string.Empty;
                  if (valueobjStockfittingExcel != null)

                  {
                    Stockfitting = StockfittingTamp.Month.ToString() + '/' + StockfittingTamp.Day.ToString();
                  }

                  // neu co line thi them vao dataList
                  if (check)
                  {
                    dataList.Add(new WorkPlanImportExcel()
                    {
                          Line = line.ToSafetyString(),
                          PONo = workSheet.Cells[rowIterator, 6].Value.ToSafetyString(),
                          ModelName = workSheet.Cells[rowIterator, 7].Value.ToSafetyString(),
                          ModelNo = workSheet.Cells[rowIterator, 8].Value.ToSafetyString(),
                          ArticleNo = workSheet.Cells[rowIterator, 9].Value.ToSafetyString(),
                          Qty = workSheet.Cells[rowIterator, 10].Value.ToSafetyString(),
                          Treatment = workSheet.Cells[rowIterator, 12].Value.ToSafetyString(),
                          Stitching = Stitching,
                          CreatedTime = Convert.ToDateTime(time),
                          Stockfitting = Stockfitting
                    });
                  }

                }
              }
              await _workPlanService.ImportExcel(dataList);
              return Ok();
            }
            else
            {
              return StatusCode(500);
            }

        }

        [HttpPost]
        public async Task<ActionResult> Import([FromForm] IFormFile files)
        {

            IFormFile file = Request.Form.Files["UploadedFile"];
            var time = Request.Form["Time"];
            var dataList = new List<WorkPlanImportExcel>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
            {
                string fileName = file.FileName;
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    var allLine = (await _buildingService.GetAllAsync()).Where(x => x.Level == 2).OrderBy(x => x.Name).Select(c => c.Name).ToList();
                    //var line = string.Empty;
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        var line = workSheet.Cells[rowIterator, 1].Value;
                        bool result;

                        // check line
                        var check = allLine.Contains(line);

                        // get value tu file excel
                        var valueStitchingExcel = workSheet.Cells[rowIterator, 7].Value.ToSafetyString();
                        var valueobjStockfittingExcel = workSheet.Cells[rowIterator, 8].Value.ToSafetyString();

                        // format sang date
                        var StitchingTamp = DateTime.FromOADate(valueStitchingExcel.ToDouble()).Date;
                        var StockfittingTamp = DateTime.FromOADate(valueobjStockfittingExcel.ToDouble()).Date;

                        //var StitchingTamp = Convert.ToDateTime(valueStitchingExcel);
                        //var StockfittingTamp = Convert.ToDateTime(valueobjStockfittingExcel);

                        var Stitching = string.Empty;
                        if (valueStitchingExcel != null)
                        {
                            Stitching = StitchingTamp.Month.ToString() + '/' + StitchingTamp.Day.ToString();
                        }

                        var Stockfitting = string.Empty;
                        if (valueobjStockfittingExcel != null)

                        {
                            Stockfitting = StockfittingTamp.Month.ToString() + '/' + StockfittingTamp.Day.ToString();
                        }

                        // neu co line thi them vao dataList
                        if (check)
                        {
                            dataList.Add(new WorkPlanImportExcel()
                            {
                                Line = line.ToSafetyString(),
                                PONo = workSheet.Cells[rowIterator, 2].Value.ToSafetyString(),
                                ModelName = workSheet.Cells[rowIterator, 3].Value.ToSafetyString(),
                                ModelNo = workSheet.Cells[rowIterator, 4].Value.ToSafetyString(),
                                ArticleNo = workSheet.Cells[rowIterator, 5].Value.ToSafetyString(),
                                Qty = workSheet.Cells[rowIterator, 6].Value.ToSafetyString(),
                                Stitching = Stitching,
                                CreatedTime = Convert.ToDateTime(time),
                                Stockfitting = Stockfitting
                            });
                        }

                    }
                }
                return Ok(await _workPlanService.ImportExcel(dataList));
            }
            else
            {
                return StatusCode(500);
            }

        }
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllStock()
        {
          var ingredientsInfo = await _chemicalService.GetAllStock();
          return Ok(ingredientsInfo);
        }

        [HttpGet("{qrCode}/{subName}/{building}/{userid}")]
        public async Task<IActionResult> ScanInput(string qrCode, string subName, string building, int userid)
        {
          var lines = await _chemicalService.ScanInput(qrCode, subName, building, userid);
          return Ok(lines);
        }

        [HttpGet("{qrCode}/{subName}/{building}/{userid}")]
        public async Task<IActionResult> ScanQRCodeOutput(string qrCode, string subName, string building, int userid)
        {
          return Ok(await _chemicalService.ScanOutput(qrCode, subName, building, userid));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _chemicalService.Deletes(id))
                return NoContent();
            throw new Exception("Error deleting the Part");
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartDto create)
        {
          return Ok(await _partService.Add(create));
        }

        [HttpPut]
        public async Task<IActionResult> Update(PartDto update)
        {
          if (await _partService.Update(update))
              return NoContent();
          return BadRequest($"Updating Part {update.ID} failed on save");
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePart(PartInkChemicalDto update)
        {
            return Ok(await _partService.UpdatePart(update));
        }

        [HttpGet]
        public async Task<IActionResult> ExcelExport()
        {
            string filename = "WorkPlanTemplate.xlsx";
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/excelTemplate", filename);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/octet-stream"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}