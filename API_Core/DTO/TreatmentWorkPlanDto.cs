using INK_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API.DTO
{
    public class TreatmentWorkPlanDto
    {
        public int ID { get; set; }
        public string Treatment { get; set; }
        public bool Status { get; set; }
        public bool FinishedStatus { get; set; }
        public string Color { get; set; }
    }
}
