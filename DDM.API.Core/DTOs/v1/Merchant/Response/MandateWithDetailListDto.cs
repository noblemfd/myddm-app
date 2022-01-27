using DDM.API.Core.DTOs.v1.Admin.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Core.DTOs.v1.Merchant.Response
{
    public class MandateWithDetailListDto
    {
        public long? Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string DrAccountNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // public string RawData { get; set; }
        public PaymentFrequency? PaymentFrequency { get; set; }      //1=Monthly, 2=Quarterly, 3=Yearly                                                               
        //   public int? PaymentCount { get; set; }      //4, or 3 based on PaymentFrequency

        [DefaultValue(false)]
        public bool? IsApproved { get; set; }
        public decimal Amount { get; set; }
        public string RequestedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [DefaultValue(false)]
        public bool? IsCancelled { get; set; }
        public string CancellationNote { get; set; }
        public DateTime? MandateCancellationDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public MerchantListDto Merchant { get; set; }
        public ICollection<MandateDetailListDto> MandateDetails { get; set; }
    }
}
