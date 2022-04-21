using INK_API.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API._Services.Interface
{
    public interface IGluesService : IECService<GluesDto>
    {
        // Task<bool> Add (InkDto entity);
        // Task<bool> AddRangeAsync(List<InkForImportExcelDto> model);
        Task<object> GetGluesByScheduleID(int id);
        Task<object> GetGluesByScheduleIDWithLocale(int id, string lang);
        Task<bool> UpdateGlueSequence(GlueSequenceDto update);
        Task<object> GetInkChemicalByGlueID(int id);
        Task<object> GetInkChemicalByGlueIDWithLocale(int id, string lang);
        Task<bool> Deletes(int id, int scheduleID);
        Task<object> SaveGlue(PartInkChemicalDto obj);


        // Task ImportExcel(List<InkForImportExcelDto> inkDto);
    }
}
