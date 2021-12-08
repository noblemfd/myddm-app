using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class AuditTrail : AuditableBaseEntity
    {
        public AuditTrail()
        {
            Timestamp = DateTime.UtcNow;
        }

        public DateTime? Timestamp { get; set; }

        public string Action { get; set; }

        public string Log { get; set; }

        public string Origin { get; set; }

        public string User { get; set; }

        public string Extra { get; set; }
        public DateTime? LoggedInAt { get; set; }
        public DateTime? LoggedOutAt { get; set; }
        public string LoginStatus { get; set; }
    }
}
