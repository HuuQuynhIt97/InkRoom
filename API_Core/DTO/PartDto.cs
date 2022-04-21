using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API.DTO
{
    public class PartDto
    {
    
         public int ID { get; set; }
        public string Name { get; set; }
        public int ObjectID { get; set; }
        public int Index { get; set; }
        public bool Status { get; set; }
        public int ScheduleID { get; set; }
    }

    public class PartScheduleDto
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public bool Status { get; set; }
    }
}
