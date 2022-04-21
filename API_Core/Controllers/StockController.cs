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
    public class StockController : ControllerBase
    {
        private readonly IPartService _partService;
        private readonly IChemicalService _chemicalService;
        public StockController(IPartService partService, IChemicalService chemicalService)
        {
            _partService = partService;
            _chemicalService = chemicalService;
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


    }
}