using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Merchant.Request
{
    public class MandateCancelDto
    {
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        [Display(Name = "Cancellation Note")]
        [Required(ErrorMessage = "Cancellation Note is Required")]
        [JsonProperty(PropertyName = "CancellationNote")]
        public string CancellationNote { get; set; }
    }
}
