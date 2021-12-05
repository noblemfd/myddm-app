using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Helpers
{
    public class EnumList
    {
        public enum Roles
        {
            Admin = 1,
            Merchant = 2,
            Staff = 3,
        }

        public enum MerchantStatus
        {
            Inactive = 0,
            Active = 1
        }
        public enum ChargeMode
        {
            None = 0,
            Fixed = 1,
            Percentage = 2
        }
        public enum ChargeRequired
        {
            False = 0,
            True = 1
        }

        public enum NotificationRequired
        {
            False = 0,
            True = 1
        }
        public enum WhoToCharge
        {
            None = 0,
            Merchant = 1,
            Customer = 2
        }
        public enum PaymentFrequency
        {
            Monthly = 1,
            Merchant = 2,
            Customer = 3
        }
        public enum MandateStatus
        {
            Pending = 0,
            Failed = 1,
            Processed = 2
        }
    }
}
