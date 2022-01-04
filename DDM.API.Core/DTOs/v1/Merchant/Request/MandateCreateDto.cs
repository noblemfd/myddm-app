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

        [StringLength(200, MinimumLength = 1)]
        [Display(Name = "Reference Number")]
        [Required(ErrorMessage = "Reference Number is Required")]
        [JsonProperty(PropertyName = "ReferenceNumber")]
        public string ReferenceNumber { get; set; }

        [StringLength(50, MinimumLength = 1)]
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
        [JsonProperty(PropertyName = "EndDate")]
        public DateTime EndDate { get; set; }
       // public string RawData { get; set; }

        [Range(1,3)]
        [Display(Name = "Payment Frequency")]
        [Required(ErrorMessage = "Payment Frequency is Required")]
        [JsonProperty(PropertyName = "PaymentFrequency")]
        public PaymentFrequency? PaymentFrequency { get; set; }      //1=Monthly, 2=Quarterly, 3=Yearly      
        //   public int? PaymentCount { get; set; }      //4, or 3 based on PaymentFrequency

        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Amount is Required")]
        [Column(TypeName = "decimal(18,2)")]
        [JsonProperty(PropertyName = "Amount")]
        public decimal Amount { get; set; }

        //public string RequestedBy { get; set; }
        //public string ApprovedBy { get; set; }
      //  public DateTime? ApprovedDate { get; set; }
    }
}
