using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Authentication
{
    public class LoginResponseDto
    {
        public string JwtToken { get; set; }
        public UserDto User { get; set; }
        public DateTime Expires { get; set; }

    }
}
