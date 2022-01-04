using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Request
{
    public class StaffCreateDto
    {
        [StringLength(50)]
        [Required(ErrorMessage = "User Name is required")]
        // [RegularExpression(@"^\S*$", ErrorMessage = "Username Cannot Have Spaces")]  // Accepts Comma
        [RegularExpression(@"^[^\s\,]+$", ErrorMessage = "Username Cannot Have Spaces")]  // Retrict comma
        [JsonProperty(PropertyName = "UserName")]
        public string UserName { get; set; }

        [StringLength(50)]
        [JsonProperty(PropertyName = "MobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty(PropertyName = "IsAdmin")]
        public bool? IsAdmin { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }
    }
}
