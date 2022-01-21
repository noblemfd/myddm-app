using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Merchant.Response
{
    public class BarChartDataDto
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
        public BarChartDataDto()
        {

        }

        public BarChartDataDto(string label, decimal value)
        {
            Label = label;
            Value = Math.Round(value, 2);
        }
    }
}
