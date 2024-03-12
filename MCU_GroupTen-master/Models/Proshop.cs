﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCU_GroupTen.Models
{
    public class Proshop
    {
        public string Member_Account { get; set; }
        public int Product_ID { get; set; }
        public string Product_Name { get; set; }
        public int Product_Stock { get; set; }
        public int Product_Price { get; set; }
        public byte[] Product_Picture { get; set; }
        public string Product_Introduce { get; set; }
        public Nullable<int> Product_Sales { get; set; }
        public string Product_Origin { get; set; }
        public string Product_Spec { get; set; }
        public string Product_Type { get; set; }
        public int Product_Seq_No { get; set; }
        public int Store_ID { get; set; }
        public int Product_Status { get; set; }
        public int Store_Status { get; set; }
        public string Store_Name { get; set; }
        public byte[] Store_Picture { get; set; }
        public int Store_Sales { get; set; }
        public DateTime Store_Startdate { get; set; }
        public DateTime Store_Enddate { get; set; }
        public string Store_Introduce { get; set; }
        public DateTime Product_StDate { get; set; }
        public string Member_identity { get; set; }
        public string Member_Status { get; set; }
    }
}