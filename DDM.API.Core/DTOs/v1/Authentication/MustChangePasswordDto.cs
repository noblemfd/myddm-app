using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Authentication
{
    public class MustChangePasswordDto
    {
 
        [Required(ErrorMessage = "The Current Password is required!")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        [JsonProperty(PropertyName = "CurrentPassword")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "The New Password is required!")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [JsonProperty(PropertyName = "NewPassword")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm New Password is required!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The New Password and Confirmation Password do not match!")]
        [JsonProperty(PropertyName = "ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
