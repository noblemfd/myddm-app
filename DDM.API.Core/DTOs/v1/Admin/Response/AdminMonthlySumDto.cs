using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Response
{
    public class AdminMonthlySumDto
    {
        public decimal ItemSum { get; set; }
        //public decimal values { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        //public string labels { get; set; }
    }
}
