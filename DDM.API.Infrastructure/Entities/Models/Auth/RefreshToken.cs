using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Models.Auth
{
    public class RefreshToken : AuditableBaseEntity
    {
        public long UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }

        public bool? IsRevoked { get; set; }
    //    public DateTime DateAdded { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
