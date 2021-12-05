using DDM.API.Infrastructure.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class Mandate : BaseEntity
    {
        public long MerchantId { get; set; }
        public string ReferenceNumber { get; set; }
        public string DrAccountNumber { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RawData { get; set; }
        public PaymentFrequency PaymentFrequncy { get; set; }
        public int? PaymentCount { get; set; }
        public decimal? Amount { get; set; }
        public string RequestedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public virtual Merchant Merchant { get; set; }
    }
}
