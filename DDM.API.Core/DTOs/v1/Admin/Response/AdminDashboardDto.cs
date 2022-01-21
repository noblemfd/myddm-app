using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Response
{
    public class AdminDashboardDto
    {
        public int? AllMandateCount { get; set; }
        public int? CompletedPaymentCount { get; set; }
        public int? ActiveMerchantCount { get; set; }
        public int? CurrentYearMandateCount { get; set; }
    }
}
