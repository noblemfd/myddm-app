using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.DTOs
{
    public class PagedResponse<T> where T : class
    {
        public List<T> Result { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public long TotalPages { get; set; }
        public ErrorResponseDto Error { get; set; }
    }
}
