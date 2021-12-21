using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDM.API.Web.Errors
{
    public class APIValidationError : APIResponse
    {
        public APIValidationError() : base(400)
        {

        }
        public IEnumerable<string> Errors { get; set; }
    }
}
