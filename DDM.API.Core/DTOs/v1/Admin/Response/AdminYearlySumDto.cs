using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Response
{
    public class AdminYearlySumDto
    {
        public decimal ItemTotal { get; set; }
        public decimal ItemPercent { get; set; }
        public int Year { get; set; }
    }
}
