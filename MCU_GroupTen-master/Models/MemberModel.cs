using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCU_GroupTen.Models
{
    public class MemberModel
    {
			public string Member_Account { get; set; }
			public string Member_Status { get; set; }
			public string Member_Password { get; set; }
			public string Member_Mail { get; set; }
			public string Member_Phone { get; set; }
			public DateTime Member_Birth { get; set; }
			public string Member_Name { get; set; }
			public string Member_Sex { get; set; }
			public string Member_Address { get; set; }
			public string Member_identity { get; set; }
			public Nullable<int> Member_Errortimes { get; set; }

			public string Before_Password { get; set; }
			public string New_Password { get; set; }
	}
}