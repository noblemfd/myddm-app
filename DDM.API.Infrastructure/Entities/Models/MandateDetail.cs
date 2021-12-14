using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class MandateDetail : AuditableBaseEntity
    {
        public long? MerchantId { get; set; }
        public long? MandateId { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string ReferenceNumber { get; set; }
        public int? SerialNumber { get; set; }  //1,2,3 ... by MandateId
        public string DrAccountNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Narration { get; set; }
        public MandateStatus MandateStatus { get; set; }    //Pending = 0, Failed = 1, Processed = 2

        [DefaultValue(false)]
        public bool? IsNotified { get; set; }

        [Display(Name = "Payable Amount")]
        [DataType(DataType.Currency)]
        public decimal? PayableAmount { get; set; }
        public string PostingReference { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }

        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }

        [ForeignKey("MerchantId")]
        public virtual Mandate Mandate { get; set; }
    }
}
