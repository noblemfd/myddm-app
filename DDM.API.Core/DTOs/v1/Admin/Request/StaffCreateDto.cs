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
        public string UserName { get; set; }

        [StringLength(50)]
        public string MobileNumber { get; set; }
        public bool? IsAdmin { get; set; }
        public string Description { get; set; }
    }
}
