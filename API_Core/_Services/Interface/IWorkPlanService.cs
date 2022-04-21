using INK_API.DTO;
using INK_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API._Services.Interface
{
    public interface IWorkPlanService : IECService<WorkPlanDTO>
    {
        Task<object> ImportExcel(List<WorkPlanImportExcel> scheduleDto);
        Task<object> GetAllAsync();
        Task<object> GetAllWorkPlanWithDate(DateTime time);
        Task<byte[]> WorkPlanFailedAdd(List<WorkPlanImportExcel> model);
        Task<object> GetPONumberByScheduleID(int id , string treatment);
        Task<object> GetPONumberByScheduleIDAndPart(int id , string treatment, int partId);
        Task<object> GetPONumberByScheduleIDAndPart2(int id , string treatment, int partId);
        Task<bool> UpdatePoGlue(int workPlanID);
        Task<bool> UpdatePart(int workPlanID, int partID);
        Task<object> GetGluesByScheduleId(int id);
        Task<object> GetGluesByScheduleIdWithQty(int id, int qty);
        Task<object> GetGluesByScheduleIdWithQtyWithLocale(int id, int qty, string lang);
        Task<object> GetPrintQRcodeByWorklan(int workplanID);
        Task<object> GetPrintQRcodeBySchedule(int scheduleID);
        Task<object> GetPrintQRcodeByGlueId(int scheduleID);
        Task<object> GetParticularBySchedule(int id, string treatment);

    }
}
