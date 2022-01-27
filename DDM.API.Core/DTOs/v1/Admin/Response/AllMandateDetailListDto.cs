using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Core.DTOs.v1.Admin.Response
{
    public class AllMandateDetailListDto
    {
        public long? Id { get; set; }
        public long? MerchantId { get; set; }
        public long? MandateId { get; set; }
        public string ReferenceNumber { get; set; }
        public int? SerialNumber { get; set; }  //1,2,3 ... by MandateId
        public string DrAccountNumber { get; set; }
        public DateTime? DueDate { get; set; }
        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        public string Narration { get; set; }
        public MandateStatus MandateStatus { get; set; }    //Pending = 0, Failed = 1, Processed = 2
        //public bool? IsNotified { get; set; }
        public decimal? PayableAmount { get; set; }
        public string PostingReference { get; set; }
        public string ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }
        public AllMerchantListDto Merchant { get; set; }
       // public AllMandateListDto Mandate { get; set; }
    }
}
