using INK_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API.DTO
{
    public class WorkPlanUpdateDto
    {

        public WorkPlanUpdateDto()
        {
            this.CreatedDate = DateTime.Now;
            this.CreatedTime = DateTime.Now;
        }
        public int ID { get; set; }
        public string Line { get; set; }
        public string PONo { get; set; }
        public string ModelName { get; set; }
        public string ModelNo { get; set; }
        public string ArticleNo { get; set; }
        public string Qty { get; set; }
        public string Treatment { get; set; }
        public string Stitching { get; set; }
        public string Stockfitting { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedTime { get; set; }


    }
}
