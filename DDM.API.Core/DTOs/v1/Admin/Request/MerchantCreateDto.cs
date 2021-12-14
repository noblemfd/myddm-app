using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Core.DTOs.v1.Admin.Request
{
    public class MerchantCreateDto
    {
        [Required(ErrorMessage = "Merchant Name is required")]
        [StringLength(100)]
        public string MerchantName { get; set; }

        [Required(ErrorMessage = "Account Number is required")]
        [StringLength(50)]
        public string AccountNumber { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "User Name is required")]
       // [RegularExpression(@"^\S*$", ErrorMessage = "Username Cannot Have Spaces")]  // Accepts Comma
        [RegularExpression(@"^[^\s\,]+$", ErrorMessage = "Username Cannot Have Spaces")]  // Retrict comma
        public string UserName { get; set; }

        [StringLength(50)]
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

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal? ChargeValue { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal? MinChargeValue { get; set; }

        [DataType(DataType.Currency)]
        public decimal? MaxChargeValue { get; set; }
    }
}
