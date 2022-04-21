using System;
namespace INK_API.Models
{
    public class PoGlue
    {
        public PoGlue()
        {
            this.CreatedTime = DateTime.Now;
        }
        public System.Guid ID { get; set; }
        public int? ScheduleID { get; set; }
        public int? PartID { get; set; }
        public bool Status { get; set; }
        public int? WorkPlanID { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
