using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Merchant.Response
{
    public class MerchantUserListDto
    {
        public long? Id { get; set; }
        public string FirstName { get; set; }
        public string LastNumber { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public bool? IsMerchantAdmin { get; set; }
    }
}
