using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Admin.Request
{
    public class AdminCreateDto
    {
        [JsonProperty(PropertyName = "UserName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "MobileNumber")]
        public string MobileNumber { get; set; }

        //[JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }
    }
}
