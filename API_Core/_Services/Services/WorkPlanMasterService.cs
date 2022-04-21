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
    public class WorkPlanMasterService : IWorkPlanMasterService
    {
        private readonly IProcessRepository _repoProcess;
        private readonly IChemicalRepository _repoChemical;
        private readonly IScheduleRepository _repoSchedule;
        private readonly IGluesRepository _repoGlues;
        private readonly IScheduleUpdateRepository _repoScheduleUpdate;
        private readonly ISupplierRepository _repoSup;
        private readonly IWorkPlanRepository _repoWorkPlan;
        private readonly IInkRepository _repoInk;
        private readonly IInkObjectRepository _repoObject;
        private readonly IPartRepository _repoPart;
        private readonly ICommentRepository _repoComment;
        private readonly ISchedulePartRepository _repoSchedulePart;
        private readonly ITreatmentWayRepository _repoTreatmentWay;

        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configMapper;
        public WorkPlanMasterService(
            ITreatmentWayRepository repoTreatmentWay,
            ICommentRepository repoComment,
            IGluesRepository repoGlues,
            IScheduleUpdateRepository repoScheduleUpdate,
            ISchedulePartRepository repoSchedulePart,
            IPartRepository repoPart,
            IInkObjectRepository repoObject,
            IScheduleRepository repoSchedule,
            IChemicalRepository repoChemical,
            IProcessRepository repoProcess,
            ISupplierRepository repoSup,
            IInkRepository repoInk,
            IWorkPlanRepository repoWorkPlan,
            IMapper mapper,
            MapperConfiguration configMapper)
        {
            _repoTreatmentWay = repoTreatmentWay;
            _repoGlues = repoGlues;
            _repoWorkPlan = repoWorkPlan;
            _repoScheduleUpdate = repoScheduleUpdate;
            _configMapper = configMapper;
            _mapper = mapper;
            _repoSchedulePart = repoSchedulePart;
            _repoInk = repoInk;
            _repoSup = repoSup;
            _repoProcess = repoProcess;
            _repoChemical = repoChemical;
            _repoSchedule = repoSchedule;
            _repoObject = repoObject;
            _repoPart = repoPart;
            _repoComment = repoComment;

        }

        public async Task<object> GetGluesMasterByScheduleID(int id)
        {
            var lists = await _repoGlues.FindAll(x => x.ScheduleID == id).OrderBy(x => x.Sequence).ToListAsync();
            var list = from a in lists
                       join b in _repoPart.FindAll() on a.PartID equals b.ID
                       join c in _repoTreatmentWay.FindAll() on a.TreatmentWayID equals c.ID
                       select new GluesDto
                       {
                           ID = a.ID,
                           Name = a.Name,
                           PartID = a.PartID,
                           TreatmentWayID = a.TreatmentWayID,
                           CreatedDate = a.CreatedDate,
                           ScheduleID = a.ScheduleID,
                           Sequence = a.Sequence.ToInt(),
                           Consumption = a.Consumption,
                           Part = b.Name,
                           TreatmentWay = c.Name
                       };
            return list.ToList();
        }

        public async Task<object> GetPONumberByScheduleID(int id)
        {
            var data = await _repoWorkPlan.FindAll(x => x.ScheduleID == id).Select(x => new
            {
                ID = x.ID,
                Name = x.PONo,
                Line = x.Line,
                Qty = x.Qty,
                Status = x.Status

            }).ToListAsync();
            return data;
        }
    }
}