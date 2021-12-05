using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public string CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedDate { get; set; }

        public string LastUpdatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastUpdatedDate { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; }
    }
}
