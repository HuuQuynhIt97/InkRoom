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

namespace INK_API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WorkPlanMasterController : ControllerBase
    {

        private readonly IPartService _partService;
        private readonly IChemicalService _chemicalService;
        private readonly IWorkPlanMasterService _workPlanMasterService;
        private readonly IBuildingService _buildingService;
        public WorkPlanMasterController(IBuildingService buildingService, IWorkPlanMasterService workPlanMasterService, IPartService partService, IChemicalService chemicalService)
        {
            _partService = partService;
            _chemicalService = chemicalService;
            _workPlanMasterService = workPlanMasterService;
            _buildingService = buildingService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPONumberByScheduleID(int id)
        {
            var brands = await _workPlanMasterService.GetPONumberByScheduleID(id);
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGluesMasterByScheduleID(int id)
        {
            var brands = await _workPlanMasterService.GetGluesMasterByScheduleID(id);
            return Ok(brands);
        }


    }
}