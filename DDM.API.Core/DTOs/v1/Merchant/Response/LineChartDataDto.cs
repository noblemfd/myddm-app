using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Merchant.Response
{
    public class LineChartDataDto
    {
        public DateTime X { get; set; }
        public decimal Y { get; set; }
        public LineChartDataDto()
        {

        }
        public LineChartDataDto(DateTime x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
