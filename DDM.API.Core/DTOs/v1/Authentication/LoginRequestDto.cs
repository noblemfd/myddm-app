using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Authentication
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "The Username is required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is required!")]
        public string Password { get; set; }

    }
}
