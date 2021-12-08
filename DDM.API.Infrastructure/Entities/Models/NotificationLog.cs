using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class NotificationLog : AuditableBaseEntity
    {
        public string NotificationDetail { get; set; } 
        public NotificationResponse? NotificationResponse { get; set; }     // None = 0, Rejected = 1, Accepted = 2
        public NotificationType? NotificationType { get; set; }     // None = 0, Merchant = 1, Customere = 2
        public string SentTo { get; set; }

        [DefaultValue(false)]
        public bool? IsRead { get; set; }
    }
}
