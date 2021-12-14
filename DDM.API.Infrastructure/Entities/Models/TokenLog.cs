using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class TokenLog : AuditableBaseEntity
    {
        public string JwtToken { get; set; }
        public DateTime Expires { get; set; }
        public long? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        //public string Message { get; set; }
        //public string MessageTemplate { get; set; }
        //public string Level { get; set; }

        //public DateTime? TimeStamp { get; set; }
        //public string Exception { get; set; }
        //public string Properties { get; set; } //XML properties
        //public string LogEvent { get; set; }
    }
}
