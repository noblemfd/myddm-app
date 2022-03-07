using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Response
{
    public class AllBankUserListDto
    {
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public bool? IsAdmin { get; set; }
        public string Description { get; set; }
    }
}
