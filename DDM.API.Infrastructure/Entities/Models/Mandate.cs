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
    public class Mandate : AuditableBaseEntity
    {
        public long MerchantId { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string ReferenceNumber { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string DrAccountNumber { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RawData { get; set; }
        public PaymentFrequency? PaymentFrequncy { get; set; }      //1=Monthly, 2=Quarterly, 3=Yearly
        public int? PaymentCount { get; set; }      //4, or 3 based on PaymentFrequency

        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal? Amount { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string RequestedBy { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }
        public virtual TransactionLog TransactionLog { get; set; }
        public virtual ICollection<MandateDetail> MandateDetails { get; set; }
    }
}
