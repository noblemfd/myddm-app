using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class StaffMember : BaseEntity
    {
        public string UserName { get; set; }
        public long? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
