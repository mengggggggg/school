using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCU_GroupTen.Models
{
    public class Checkout
    {
        public int totally { get; set; }
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
        public string Store_Name { get; set; }
        public byte[] Store_Picture { get; set; }
        public int Store_Product_Sales { get; set; }

        public string Store_Introduce { get; set; }

        public string Member_Account { get; set; }

        public int Order_ID { get; set; }
        public int Order_Total_Amount { get; set; }
        public string Order_Status { get; set; }
        public System.DateTime Order_Date { get; set; }
        public System.DateTime Order_Delivery_Date { get; set; }
        public string Order_Delivery_Status { get; set; }
        public string Order_Payment_Status { get; set; }
        public string Order_Address { get; set; }
        public string Order_Delivery_Way { get; set; }

        public int Product_Count { get; set; }

        public int Order_Amount { get; set; }

        public string Order_Details_ID { get; set; }

    }
}