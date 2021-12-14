using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.Helpers
{
    public static class ConstantHelper
    {
        public static int GetTotalMonth(DateTime startDate, DateTime endDate)
        {
            int totalMonth = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Convert.ToInt32(Math.Abs(totalMonth));
        }

        public static int GetTotalQuarter(DateTime startDate, DateTime endDate)
        {
            int firstQuarter = getQuarter(startDate);
            int secondQuarter = getQuarter(endDate);
            return 1 + Math.Abs(firstQuarter - secondQuarter);
        }

        private static int getQuarter(DateTime date)
        {
            return (date.Year * 4) + ((date.Month - 1) / 3);
        }

        public static int GetTotalYear(DateTime startDate, DateTime endDate)
        {
            int years = endDate.Year - startDate.Year;

            if (startDate.Month == endDate.Month &&// if the start month and the end month are the same
                endDate.Day < startDate.Day)// BUT the end day is less than the start day
            {
                years--;
            }
            else if (endDate.Month < startDate.Month)// if the end month is less than the start month
            {
                years--;
            }
            return Math.Abs(years);
        }
        public static int ConvertDateToInt(DateTime date)
        {
            return (date.Year * 10000) + (date.Month * 100) + (date.Day);
        }
    }
}
