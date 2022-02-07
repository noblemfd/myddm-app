using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Merchant.Request
{
    public class MerchantUserUpdateDto
    {
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Required(ErrorMessage = "First Name is required")]
        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Required(ErrorMessage = "Last Name is required")]
        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
        [Required(ErrorMessage = "User Name is required")]
        [JsonProperty(PropertyName = "UserName")]
        [RegularExpression(@"^[^\s\,]+$", ErrorMessage = "Username Cannot Have Spaces")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[JsonProperty(PropertyName = "Password")]
        //[Required(ErrorMessage = "Password is required")]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        //[JsonProperty(PropertyName = "ConfirmPassword")]
        //[Required(ErrorMessage = "Confirm Password is required")]
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "The Password and Confirmation Password do not match!")]
        //public string ComparePassword { get; set; }

        [StringLength(15)]
        [JsonProperty(PropertyName = "MobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty(PropertyName = "IsMerchantAdmin")]
        public bool? IsMerchantAdmin { get; set; }
    }
}
