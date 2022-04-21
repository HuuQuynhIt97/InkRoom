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
    public class SettingController : ControllerBase
    {
        private readonly ISettingService _settingService;
        public SettingController( ISettingService settingService)
        {
            _settingService = settingService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllSetting()
        {
            var settings = await _settingService.GetAllAsync();
            return Ok(settings);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // if (await _kindService.Delete(id))
            //     return NoContent();
            throw new Exception("Error deleting the Kind");
        }
    }
}