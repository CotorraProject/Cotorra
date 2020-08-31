using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System;
using System.Globalization;

namespace Cotorra.Core.Utils
{
    public class DateTimeUtil
    {
        private string DATETIME_FORMAT = "yyyy-MM-ddTHH:mm:ss";

        public static DateTime DATETIME_ORIGIN = new DateTime(1900, 1, 1);

        public DateTime FixDateTime(DateTime toFix)
        {
            return DateTime.ParseExact(toFix.ToString(DATETIME_FORMAT), DATETIME_FORMAT, CultureInfo.InvariantCulture);
        }

        public static int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = (int)(end.DayOfWeek - day);   // Number of days since last [day]
            if (sinceLastDay < 0) sinceLastDay += 7;         // Adjust for negative days since last [day]

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay) count++;

            return count;
        }

        public string GetDate(DateTime dateTime)
        {
            return dateTime.ToShortDateString();
        }
        public string GetHour(DateTime date)
        {
            var treponseHour  = date.ToString("HH:mm:ss");
            return treponseHour;
        }

        /// <summary>
        /// Get the intersection number of days between two range dates
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="e1"></param>
        /// <param name="s2"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public int InclusiveDays(DateTime s1, DateTime e1, DateTime s2, DateTime e2)
        {
            // If they don't intersect return 0.
            if (!(s1 <= e2 && e1 >= s2))
            {
                return 0;
            }

            // Take the highest start date and the lowest end date.
            DateTime start = s1 > s2 ? s1 : s2;
            DateTime end = e1 > e2 ? e2 : e1;

            // Add one to the time range since its inclusive.
            return (int)(end - start).TotalDays + 1;
        }
    }

    public class CalculateDateDifference
    {
        /// <summary>
        /// defining Number of days in month; index 0 represents january and 11 represents December
        /// february contain either 28 or 29 days, so here its value is -1
        /// which will be calculate later.
        /// </summary>
        private int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// contain from date
        /// </summary>
        private DateTime fromDate;

        /// <summary>
        /// contain To Date
        /// </summary>
        private DateTime toDate;

        /// <summary>
        /// these three variable of integer type for output representation..
        /// </summary>
        private int year;
        private int month;
        private int day;
        bool d2WasBigger;

        public CalculateDateDifference()
        {

        }

        //Public type Constructor
        public CalculateDateDifference(DateTime d1, DateTime d2)
        {
            Calculate(d1, d2);
        }

        public CalculateDateDifference Calculate(DateTime d1, DateTime d2)
        {
            int increment = 0;

            //To Date must be greater
            if (d1 > d2)
            {
                this.fromDate = d2;
                this.toDate = d1;
            }
            else
            {
                this.fromDate = d1;
                this.toDate = d2;
                d2WasBigger = true;
            }

            ///
            /// Day Calculation
            ///
            if (this.fromDate.Day > this.toDate.Day)
            {
                increment = this.monthDay[this.fromDate.Month - 1];
            }

            /// if it is february month
            /// if it's to day is less then from day
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(this.fromDate.Year))
                {
                    // leap year february contain 29 days
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }
            if (increment != 0)
            {
                day = (this.toDate.Day + increment) - this.fromDate.Day;
                increment = 1;
            }
            else
            {
                day = this.toDate.Day - this.fromDate.Day;
            }


            ///
            ///month calculation
            ///
            if ((this.fromDate.Month + increment) > this.toDate.Month)
            {
                this.month = (this.toDate.Month + 12) - (this.fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                this.month = (this.toDate.Month) - (this.fromDate.Month + increment);
                increment = 0;
            }


            ///
            /// year calculation
            ///
            this.year = this.toDate.Year - (this.fromDate.Year + increment);

            return this;
        }

        public int CalculateWeeks()
        {
            return (int)Math.Truncate((this.toDate - this.fromDate).TotalDays / 7);
        }

        public override string ToString()
        {
            return this.year + " Year(s), " + this.month + " month(s), " + this.day + " day(s)";
        }

        public string ToStringColaboratorSpanish()
        {
            string yearsRep, monthRep, daysRep;
            string message = string.Empty;
            if (d2WasBigger)
            {
                message = "Próximo ingreso";
                return message;
            }

            yearsRep = this.year > 0 ? this.year > 1 ? this.year + " Años " : this.year + " Año " : string.Empty;
            monthRep = this.month > 0 ? this.month > 1 ? this.month + " meses " : this.month + " mes " : string.Empty;
            daysRep = this.day > 0 ? this.day > 1 ? this.day + " días" : this.day + " día " : string.Empty;
            message = yearsRep + monthRep + daysRep;
            if (message == string.Empty)
            {
                message = "Recién contratado";
            }
            return message;
        }

        public int Years
        {
            get
            {
                return this.year;
            }
        }

        public int Months
        {
            get
            {
                return this.month;
            }
        }

        public int Days
        {
            get
            {
                return this.day;
            }
        }
    }
}
