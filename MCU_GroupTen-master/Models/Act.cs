using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCU_GroupTen.Models
{
    public class Act
    {
        public int Activity_ID { get; set; }
        public int Activity_Status { get; set; }
        public string Activity_Information { get; set; }
        public string Activity_URL { get; set; }
        public DateTime Activity_StartDate { get; set; }
        public byte[] Activity_Picture { get; set; }

    }
}