using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Core.DTOs.v1.Admin.Response
{
    public class AllMerchantListDto
    {
        public long? Id { get; set; }
        public string MerchantName { get; set; }
        public string AccountNumber { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public MerchantStatus? MerchantStatus { get; set; }
        public NotificationRequired? NotificationRequired { get; set; }
        //public long? UserId { get; set; }
        public string NotificationUrl { get; set; }
        public string NotificationCredential { get; set; }  // False = 0, True = 1
        public ChargeRequired? ChargeRequired { get; set; }
        public ChargeMode? ChargeMode { get; set; }     // None = 0, Fixed = 1, Percentage = 2
        public WhoToCharge? WhoToCharge { get; set; }     // None = 0, Merchant = 1, Customer = 2
        public int? ChargePercent { get; set; }
        public decimal? ChargeValue { get; set; }
        public decimal? MinChargeValue { get; set; }
        public decimal? MaxChargeValue { get; set; }
        public DateTime? CreatedDate { get; set; }
        public AllUserListDto User { get; set; }
        public ICollection<AllMerchantUserListDto> MerchantUsers { get; set; }
    }
}
