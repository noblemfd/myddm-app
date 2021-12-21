using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class TransactionLog : AuditableBaseEntity
    {
        [ForeignKey("MerchantId")]
        public long? MerchantId { get; set; }

        [ForeignKey("Mandate")]
        public long? MandateId { get; set; }
        public string RawData { get; set; }
        public virtual Mandate Mandate { get; set; }
        public virtual Merchant Merchant { get; set; }
    }
}
