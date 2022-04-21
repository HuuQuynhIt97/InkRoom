using INK_API.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API._Services.Interface
{
    public interface IInkService : IECService<InkDto>
    {
        Task<List<InkDto>> GetAllWithLocale(string lang);
        Task<bool> Add (InkDto entity);
        Task<bool> UpdateAsync (InkUpdateDto entity);
        Task<bool> AddRangeAsync(List<InkForImportExcelDto> model);
        Task ImportExcel(List<InkForImportExcelDto> inkDto);
    }
}
