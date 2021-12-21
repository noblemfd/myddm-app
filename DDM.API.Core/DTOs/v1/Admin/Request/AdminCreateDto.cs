using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Request
{
    public class AdminCreateDto
    {
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
    }
}
