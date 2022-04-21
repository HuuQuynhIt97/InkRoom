using System;
using System.Security.Claims;
using System.Threading.Tasks;
using INK_API.Helpers;
using INK_API._Services.Interface;
using INK_API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INK_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GlueController : ControllerBase
    {
        private readonly IGluesService _gluesService;
        public GlueController(IGluesService gluesService)
        {
            _gluesService = gluesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGlue(GluesDto create)
        {
            return Ok(await _gluesService.Add(create));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGlue(GluesDto update)
        {
            return Ok(await _gluesService.Update(update));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGlueSequence(GlueSequenceDto update)
        {
            return Ok(await _gluesService.UpdateGlueSequence(update));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGluesByScheduleID(int id)
        {
            var brands = await _gluesService.GetGluesByScheduleID(id);
            return Ok(brands);
        }

        [HttpGet("{id}/{lang}")]
        public async Task<IActionResult> GetGluesByScheduleIDWithLocale(int id, string lang)
        {
            var brands = await _gluesService.GetGluesByScheduleIDWithLocale(id, lang);
            return Ok(brands);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetInkChemicalByGlueID(int id)
        {
            var brands = await _gluesService.GetInkChemicalByGlueID(id);
            return Ok(brands);
        }

        [HttpGet("{id}/{lang}")]
        public async Task<IActionResult> GetInkChemicalByGlueIDWithLocale(int id, string lang)
        {
            var brands = await _gluesService.GetInkChemicalByGlueIDWithLocale(id,lang);
            return Ok(brands);
        }

        [HttpDelete("{id}/{scheduleID}")]
        public async Task<IActionResult> Delete(int id ,int scheduleID)
        {
            if (await _gluesService.Deletes(id, scheduleID))
                return NoContent();
            throw new Exception("Error deleting the Part");
        }

        [HttpGet(Name = "GetGlues")]
        public async Task<IActionResult> GetAll()
        {
            var glues = await _gluesService.GetAllAsync();
            return Ok(glues);
        }

        [HttpPut]
        public async Task<IActionResult> SaveGlue(PartInkChemicalDto update)
        {
            return Ok(await _gluesService.SaveGlue(update));
        }

        // [HttpGet]
        // public async Task<IActionResult> GetAllWorkPlan()
        // {
        //     var inks = await _workPlanService.GetAllAsync();
        //     return Ok(inks);
        // }
        // [HttpPost]
        // public async Task<ActionResult> Import([FromForm] IFormFile file2)
        // {
        //     IFormFile file = Request.Form.Files["UploadedFile"];
        //     var dataList = new List<WorkPlanImportExcel>();
        //     ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //     // if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
        //     // {
        //     //     string fileName = file.FileName;
        //     //     using (var package = new ExcelPackage(file.OpenReadStream()))
        //     //     {
        //     //         var currentSheet = package.Workbook.Worksheets;
        //     //         var workSheet = currentSheet.First();
        //     //         var noOfCol = workSheet.Dimension.End.Column;
        //     //         var noOfRow = workSheet.Dimension.End.Row;

        //     //         for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
        //     //         {
        //     //             dataList.Add(new WorkPlanImportExcel()
        //     //             {
        //     //                 Line = workSheet.Cells[rowIterator, 1].Value.ToSafetyString(),
        //     //                 PONo = workSheet.Cells[rowIterator, 2].Value.ToSafetyString(),
        //     //                 ModelName = workSheet.Cells[rowIterator, 3].Value.ToSafetyString(),
        //     //                 ModelNo = workSheet.Cells[rowIterator, 4].Value.ToSafetyString(),
        //     //                 ArticleNo = workSheet.Cells[rowIterator, 5].Value.ToSafetyString(),
        //     //                 Qty = workSheet.Cells[rowIterator, 6].Value.ToSafetyString(),
        //     //                 Treatment = workSheet.Cells[rowIterator, 7].Value.ToSafetyString(),
        //     //                 Stitching = workSheet.Cells[rowIterator, 8].Value.ToSafetyString(),
        //     //                 Stockfitting = workSheet.Cells[rowIterator, 9].Value.ToSafetyString(),
        //     //             });
        //     //         }
        //     //     }
        //         if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
        //         {
        //             string fileName = file.FileName;
        //             using (var package = new ExcelPackage(file.OpenReadStream()))
        //             {
        //                 var currentSheet = package.Workbook.Worksheets;
        //                 var workSheet = currentSheet.First();
        //                 var noOfCol = workSheet.Dimension.End.Column;
        //                 var noOfRow = workSheet.Dimension.End.Row;
        //                 var endRow = 0 ;
        //                 var startRow = 5 ;
        //                 var listObj = new List<string>() ;
        //                 var AllLine = (await _buildingService.GetAllAsync()).Where(x => x.Level == 2).OrderBy(x => x.Name).Select(c=> c.Name).ToList();
        //                 // foreach (var item in AllLine)
        //                 // {

        //                 // }
        //                 var line = string.Empty;
        //                 var info = new {
        //                             startRow = startRow,
        //                             endRow = endRow,
        //                             Line = line
        //                         };
        //                 for (int rowIterator = startRow; rowIterator <= noOfRow; rowIterator++)
        //                 {
        //                     line = workSheet.Cells[rowIterator, 1].Value.ToSafetyString().Replace("LEAN", "").Trim();
        //                     var variable = AllLine.Contains(line);
        //                     if (AllLine.Contains(line)) {
        //                         endRow = rowIterator ;
        //                         listObj.Add(JsonConvert.SerializeObject(info));
        //                         startRow = rowIterator + 3;
        //                         AllLine = AllLine.Where(line => line.Contains(line)!).ToList();
        //                     } else {

        //                     }
        //                     dataList.Add(new WorkPlanImportExcel()
        //                     {
        //                         Line = workSheet.Cells[rowIterator, 1].Value.ToSafetyString(),
        //                         ModelName = workSheet.Cells[rowIterator, 7].Value.ToSafetyString(),
        //                         ModelNo = workSheet.Cells[rowIterator, 8].Value.ToSafetyString(),
        //                         ArticleNo = workSheet.Cells[rowIterator, 9].Value.ToSafetyString(),
        //                     });
        //                 }
        //             }
        //         // await _workPlanService.ImportExcel(dataList);
        //         return Ok();
        //     }
        //     else
        //     {
        //         return StatusCode(500);
        //     }

        // }
    }
}