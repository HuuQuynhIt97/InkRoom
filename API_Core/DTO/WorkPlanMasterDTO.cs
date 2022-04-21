using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INK_API.Helpers;

namespace INK_API.DTO
{
    public class WorkPlanMasterDTO
    {
        public System.Guid ID { get; set; }
        public int? ScheduleID { get; set; }
        public int? GlueID { get; set; }
        public int? WorkPlanID { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}
