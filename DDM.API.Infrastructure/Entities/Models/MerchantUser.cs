using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class MerchantUser : AuditableBaseEntity
    {
        public long MerchantId { get; set; }
        public long UserId { get; set; }
        public bool? IsMerchantAdmin { get; set; }
        //public string UserName { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }
    }
}
