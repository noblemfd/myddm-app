using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.DTOs
{
    public class ErrorResponseDto
    {
        public bool? Success { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
