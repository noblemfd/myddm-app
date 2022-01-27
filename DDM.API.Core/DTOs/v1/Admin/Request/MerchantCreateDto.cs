using Newtonsoft.Json;
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
        [JsonProperty(PropertyName = "MerchantName")]
      //  [StringLength(100)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string MerchantName { get; set; }

        [Required(ErrorMessage = "Account Number is required")]
        [JsonProperty(PropertyName = "AccountNumber")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        public string AccountNumber { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        [Required(ErrorMessage = "User Name is required")]
        [JsonProperty(PropertyName = "UserName")]
        [RegularExpression(@"^[^\s\,]+$", ErrorMessage = "Username Cannot Have Spaces")]  
        public string UserName { get; set; }

        [StringLength(15)]
        [JsonProperty(PropertyName = "MobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty(PropertyName = "MerchantStatus")]
        public MerchantStatus? MerchantStatus { get; set; }

        [JsonProperty(PropertyName = "NotificationRequired")]
        public NotificationRequired? NotificationRequired { get; set; }
        //public long? UserId { get; set; }

        [JsonProperty(PropertyName = "NotificationUrl")]
        public string NotificationUrl { get; set; }

        [JsonProperty(PropertyName = "NotificationCredential")]
        public string NotificationCredential { get; set; }  // False = 0, True = 1

        [JsonProperty(PropertyName = "ChargeRequired")]
        public ChargeRequired? ChargeRequired { get; set; }

        [JsonProperty(PropertyName = "ChargeMode")]
        public ChargeMode? ChargeMode { get; set; }     // None = 0, Fixed = 1, Percentage = 2

        [JsonProperty(PropertyName = "WhoToCharge")]
        public WhoToCharge? WhoToCharge { get; set; }     // None = 0, Merchant = 1, Customer = 2

        [JsonProperty(PropertyName = "ChargePercent")]
        public int? ChargePercent { get; set; }

        [JsonProperty(PropertyName = "ChargeValue")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        public decimal? ChargeValue { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 9999999999999999.99)]
        [JsonProperty(PropertyName = "MinChargeValue")]
        public decimal? MinChargeValue { get; set; }

        [DataType(DataType.Currency)]

        [JsonProperty(PropertyName = "MaxChargeValue")]
        public decimal? MaxChargeValue { get; set; }
    }
}
