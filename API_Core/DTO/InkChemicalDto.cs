using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API.DTO
{
    public class InkChemicalDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Subname { get; set; }
        public string MaterialNo { get; set; }

        public double percentage { get; set; }
        public double? Consumption { get; set; }

        public bool modify { get; set; }
        public string Code { get; set; }

    }
}
