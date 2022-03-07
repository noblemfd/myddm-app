using DDM.API.Core.DTOs.v1.Admin.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Response
{
    public class AllMerchantUserListDto
    {
        public long? Id { get; set; }
        public bool? IsMerchantAdmin { get; set; }
        public AllUserListDto User { get; set; }
        //public AllMerchantListDto Merchant { get; set; }
    }
}
