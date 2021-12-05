using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class Merchant : BaseEntity
    {
        public string MerchantName { get; set; }
        public string AccountNumber { get; set; }
        public MerchantStatus? MerchantStatus { get; set; }
        public NotificationRequired? NotificationRequired { get; set; }
        public string UserName { get; set; }
        public long? UserId { get; set; }
        public string NotificationUrl { get; set; }
        public string NotificationCredential { get; set; }
        public ChargeRequired? ChargeRequired { get; set; }
        public ChargeMode? ChargeMode { get; set; }
        public decimal? ChargePercent { get; set; }
        public decimal? ChargeValue { get; set; }
        public decimal? MinChargeValue { get; set; }
        public decimal? MaxChargeValue { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
