using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Request
{
    public class BankUserCreateDto
    {
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Required(ErrorMessage = "First Name is required")]
        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Required(ErrorMessage = "Last Name is required")]
        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "User Name is required")]
        // [RegularExpression(@"^\S*$", ErrorMessage = "Username Cannot Have Spaces")]  // Accepts Comma
        [RegularExpression(@"^[^\s\,]+$", ErrorMessage = "Username Cannot Have Spaces")]  // Retrict comma
        [JsonProperty(PropertyName = "UserName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "ConfirmPassword")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The Password and Confirmation Password do not match!")]
        public string ComparePassword { get; set; }

        [StringLength(50)]
        [JsonProperty(PropertyName = "MobileNumber")]
        public string MobileNumber { get; set; }

        [StringLength(300)]
        [JsonProperty(PropertyName = "BankBranch")]
        [Required(ErrorMessage = "Bank Branch is required")]
        public string BankBranch { get; set; }

        [StringLength(300)]
        [JsonProperty(PropertyName = "HeadOffice")]
        //[Required(ErrorMessage = "Head Office is required")]
        public string HeadOffice { get; set; }

        [JsonProperty(PropertyName = "IsAdmin")]
        public bool? IsAdmin { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }
    }
}
