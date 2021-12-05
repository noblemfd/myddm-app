using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class MandateDetail : BaseEntity
    {
        public long MerchantId { get; set; }
        public long MandateId { get; set; }
        public string ReferenceNumber { get; set; }
        public int SerialNumber { get; set; }
        public string DrAccountNumber { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Narration { get; set; }
        public MandateStatus Status { get; set; }
        public bool? IsNotified { get; set; }
        public decimal? Amount { get; set; }
        public string PostingReference { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
