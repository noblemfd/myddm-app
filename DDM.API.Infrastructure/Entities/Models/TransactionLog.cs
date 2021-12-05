using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Entities.Models
{
    public class TransactionLog
    {
        public long MerchantId { get; set; }
        public long MandateId { get; set; }
        public string RawData { get; set; }
    }
}
