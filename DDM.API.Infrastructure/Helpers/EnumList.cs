using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Helpers
{
    public class EnumList
    {
        public enum Roles : byte
        {
            Admin = 1,
            Merchant = 2,
            Staff = 3,
        }

        public enum MerchantStatus : byte
        {
            Inactive = 0,
            Active = 1
        }
        public enum ChargeMode : byte
        {
            None = 0,
            Fixed = 1,
            Percentage = 2
        }
        public enum ChargeRequired : byte
        {
            False = 0,
            True = 1
        }

        public enum NotificationResponse : byte
        {
            None = 0,
            Rejected = 1,
            Accepted = 2
        }

        public enum NotificationType : byte
        {
            None = 0,
            Merchant = 1,
            Customer = 2
        }

        public enum NotificationRequired : byte
        {
            False = 0,
            True = 1
        }
        public enum WhoToCharge : byte
        {
            None = 0,
            Merchant = 1,
            Customer = 2
        }
        public enum PaymentFrequency : byte
        {
            Monthly = 1,
            Merchant = 2,
            Customer = 3
        }
        public enum MandateStatus : byte
        {
            Pending = 0,
            Failed = 1,
            Processed = 2
        }
    }
}
