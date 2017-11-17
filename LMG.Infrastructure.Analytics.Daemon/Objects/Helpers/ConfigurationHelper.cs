/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class ConfigurationHelper
    {
        public static List<string> GetStudentGroups()
        {
            List<string> list = new List<string>();
            string val = ConfigurationManager.AppSettings["StudentGroupFilter"];
            if (!string.IsNullOrWhiteSpace(val))
            {
                list.AddRange(val.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            }
            return list;
        }

        public static bool GetSubmissionPeriod(out DateTime pushStartDate, out DateTime? pushStopDate)
        {
            return GetPeriod(out pushStartDate, out pushStopDate, "Push");
        }
        public static bool GetExtractionPeriod(out DateTime exportStartDate, out DateTime? exportStopDate)
        {
            return GetPeriod(out exportStartDate, out exportStopDate, "Pull");
        }
        public static bool GetPeriod(out DateTime exportStartDate, out DateTime? exportStopDate, string push_or_pull)
        {
            DateTime dateFrom, dateUntil;
            string str_fromDate = ConfigurationManager.AppSettings["DaemonMode" + push_or_pull + "FromDate"];
            string str_untilDate = ConfigurationManager.AppSettings["DaemonMode" + push_or_pull + "UntilDate"];
            if (str_fromDate == "yesterday")
            {
                //Get everything from yesterday to midnight
                exportStartDate = DateTime.SpecifyKind(DateTime.Today.AddDays(-1), DateTimeKind.Utc);
                exportStopDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
            }
            else if (str_fromDate == "today")
            {
                //Get everything from today (so far!)
                exportStartDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
                exportStopDate = DateTime.SpecifyKind(DateTime.Today.AddDays(1), DateTimeKind.Utc);
            }
            else
            {
                bool from_ok = DateTime.TryParseExact(str_fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateFrom);
                bool until_ok = DateTime.TryParseExact(str_untilDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateUntil);

                if (from_ok
                    &&
                    until_ok
                    )
                {
                    exportStartDate = DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc);
                    exportStopDate = DateTime.SpecifyKind(dateUntil, DateTimeKind.Utc);

                    //DateTime exportStartDate = new DateTime(2017, 01, 30, 0, 0, 0, DateTimeKind.Utc);
                    //DateTime exportStopDate = new DateTime(2017, 02, 07, 0, 0, 0, DateTimeKind.Utc);
                }
                else if (from_ok && string.IsNullOrWhiteSpace(str_untilDate))
                {
                    exportStartDate = DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc);
                    exportStopDate = null;
                }
                else
                {
                    //No valid date found. Stop!
                    exportStartDate = DateTime.MinValue;
                    exportStopDate = DateTime.MinValue;

                    return false;
                }
            }
            return true;
        }

    }
}
