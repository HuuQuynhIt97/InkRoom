﻿using INK_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INK_API.DTO
{
    public class ScheduleUpdateEditDto
    {
        public int ID { get; set; }
        public string ModelName { get; set; }
        public string ModelNo { get; set; }
        public string ArticleNo { get; set; }
        public string Treatment { get; set; }
        public string Process { get; set; }

        public int ApprovalBy { get; set; }
        public int CreatedBy { get; set; }
        public bool ApprovalStatus { get; set; }
        public bool FinishedStatus { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? EstablishDate { get; set; }
        public string Season { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? UpdateTime { get; set; }


    }
}
