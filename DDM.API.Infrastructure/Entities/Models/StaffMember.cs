using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class StaffMember : AuditableBaseEntity
    {
        public long? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [DefaultValue(false)]
        public bool? IsAdmin { get; set; }
        public string Description { get; set; }
    }
}
