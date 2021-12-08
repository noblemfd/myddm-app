using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class Merchant : AuditableBaseEntity
    {
        [Required]
        [Column(TypeName = "Varchar")]
        [StringLength(100)]
        public string MerchantName { get; set; }

        [Required]
        [Column(TypeName = "Varchar")]
        [StringLength(50)]
        public string AccountNumber { get; set; }
        public MerchantStatus? MerchantStatus { get; set; }
        public NotificationRequired? NotificationRequired { get; set; }
        public long? UserId { get; set; }
        public string NotificationUrl { get; set; }
        public string NotificationCredential { get; set; }  // False = 0, True = 1
        public ChargeRequired? ChargeRequired { get; set; }
        public ChargeMode? ChargeMode { get; set; }     // None = 0, Fixed = 1, Percentage = 2

        public WhoToCharge? WhoToCharge { get; set; }     // None = 0, Merchant = 1, Customer = 2
        public int? ChargePercent { get; set; }

        [DataType(DataType.Currency)]
        public decimal? ChargeValue { get; set; }

        [DataType(DataType.Currency)]
        public decimal? MinChargeValue { get; set; }

        [DataType(DataType.Currency)]
        public decimal? MaxChargeValue { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Mandate> Mandates { get; set; }
        public virtual ICollection<MandateDetail> MandateDetails { get; set; }
    }
}
