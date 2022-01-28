using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Merchant.Response
{
    public class MerchantMonthlySumDto
    {
        public decimal ItemSum { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
    }
}
