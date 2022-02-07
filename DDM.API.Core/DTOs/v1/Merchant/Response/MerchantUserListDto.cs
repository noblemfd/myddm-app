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
        public bool? IsMerchantAdmin { get; set; }
        public UserListDto User { get; set; }
        public MerchantListDto Merchant { get; set; }
    }
}
