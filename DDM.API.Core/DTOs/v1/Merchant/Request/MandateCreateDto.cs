using DDM.API.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Core.DTOs.v1.Merchant.Request
{
    public class MandateCreateDto
    {
        public long MerchantId { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Reference Number")]
        [Required(ErrorMessage = "Reference Number is Required")]
        [JsonProperty(PropertyName = "ReferenceNumber")]
        public string ReferenceNumber { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Debit Account Noumber")]
        [Required(ErrorMessage = "Debit Account No. is Required")]
        [JsonProperty(PropertyName = "DrAccountNumber")]
        public string DrAccountNumber { get; set; }
        //  public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is Required")]
        [JsonProperty(PropertyName = "StartDate")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is Required")]
        [DateGreaterThanAttribute(otherPropertyName = "StartDate", ErrorMessage = "End Date must be greater than Start Date")]
        [JsonProperty(PropertyName = "EndDate")]
        public DateTime EndDate { get; set; }

        //[DataType(DataType.Date)]
        //[Display(Name = "Debit Date/Due Date")]
        //[DateGreaterThanAttribute(otherPropertyName = "StartDate", ErrorMessage = "Debit Date must be greater than Start Date")]
        //[JsonProperty(PropertyName = "DueDate")]
        //public DateTime? DueDate { get; set; }

        [Range(1,4)]
        [Display(Name = "Payment Frequency")]
        [Required(ErrorMessage = "Payment Frequency is Required")]
        [JsonProperty(PropertyName = "PaymentFrequency")]
        public PaymentFrequency? PaymentFrequency { get; set; }      //1=Monthly, 2=Quarterly, 3=BiAnnual, 4=Yearly      

        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Amount is Required")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1000, 99999999999999999999999999999999.99, ErrorMessage = "Please enter a value greater than {1000}")]
        [JsonProperty(PropertyName = "Amount")]
        public decimal Amount { get; set; }
    }
}
