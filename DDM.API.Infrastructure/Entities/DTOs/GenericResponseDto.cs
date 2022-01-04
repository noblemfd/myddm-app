using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.DTOs
{
    public class GenericResponseDto<T>
    {
        public int? StatusCode { get; set; }
        public string Message { get; set; }
       // public bool? HasError { get; set; }
        public T Result { get; set; }
        public ErrorResponseDto Error { get; set; }
    }
}
