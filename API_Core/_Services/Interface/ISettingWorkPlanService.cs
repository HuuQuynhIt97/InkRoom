using INK_API.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API._Services.Interface
{
    public interface ISettingWorkPlanService
    {
      //Task<bool> Deletes(int id);
      //Task<object> UpdatePart(PartInkChemicalDto obj);
      Task<object> GetLines(int id);
      Task<object> GetPoSettingWorkPlan(string line);
    }
}
