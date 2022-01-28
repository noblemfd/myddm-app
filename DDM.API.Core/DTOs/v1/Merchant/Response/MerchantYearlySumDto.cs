using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Merchant.Response
{
    public class MerchantYearlySumDto
    {
        public decimal ItemTotal { get; set; }
        public int ItemPercent { get; set; }
        public int Year { get; set; }
    }
}
