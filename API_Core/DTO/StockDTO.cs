using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INK_API.Helpers;

namespace INK_API.DTO
{
    public class StockDTO
    {
        public StockDTO()
        {
            this.CreatedDate = DateTime.Now;
            this.CreatedTime = DateTime.Now;
        }
        public int ID { get; set; }
        public int InkOrChemicalID { get; set; }
        public string Code { get; set; }
        public string NameInkOrChemical { get; set; }
        public string SubName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public string SupplierName { get; set; }
        public string Batch {get; set ;}
        public DateTime ExpiredTime { get; set; }
        public int Unit { get; set; }
        public int UserID { get; set; }
        public string BuildingName { get; set; }

        public bool Status { get; set; }

    }
}
