using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API.DTO
{
    public class GlueSequenceDto
    {
        public int GlueDefaultID { get; set; }
        public int GlueChangeID { get; set; }
        public int ScheduleID { get; set; }
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
    }
}
