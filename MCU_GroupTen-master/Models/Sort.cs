using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCU_GroupTen.Models
{
    public class Sort
    {

        public List<Act_Data> Index_Act_Data { get; set; }
        public class Act_Data
        {
            public int Activity_ID { get; set; }
            public string Activity_Information { get; set; }
            public string Activity_URL { get; set; }
            public DateTime Activity_StartDate { get; set; }
            public byte[] Activity_Picture { get; set; }
        }
        public List<Products_Data> Index_Products_Data { get; set; }
        public class Products_Data
        {
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
            public string Store_Name { get; set; }
            public int Product_Seq_No { get; set; }
            public int Store_ID { get; set; }
            public DateTime Product_StDate { get; set; }
        }
        public List<ProductsDate_Data> Date_Products_Data { get; set; }
        public class ProductsDate_Data
        {
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
            public string Store_Name { get; set; }
            public int Product_Seq_No { get; set; }
            public int Store_ID { get; set; }
            public DateTime Product_StDate { get; set; }
        }

        public List<Store_Data> Index_Store_Data { get; set; }
        public class Store_Data
        {
            public int Store_ID { get; set; }
            public string Store_Name { get; set; }
            public byte[] Store_Picture { get; set; }
            public int Store_Sales { get; set; }
            public DateTime Store_Startdate { get; set; }
            public DateTime Store_Enddate { get; set; }
            public string Store_Introduce { get; set; }
            public DateTime Product_StDate { get; set; }
        }
    }

    
}