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
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Models
{
    public class CourseInstanceTimeBlockModel
    {
        private long m_CourseInstanceId = -1;
        public long CourseInstanceId
        {
            get { return m_CourseInstanceId; }
            set { m_CourseInstanceId = value; }
        }

        private long m_TimeBlock = -1;
        public long TimeBlock
        {
            get { return m_TimeBlock; }
            set { m_TimeBlock = value; }
        }

        private DateTime? m_fromDate = null;
        public DateTime? FromDate
        {
            get { return m_fromDate; }
            set { m_fromDate = value; }
        }

        private DateTime? m_untilDate = null;
        public DateTime? UntilDate
        {
            get { return m_untilDate; }
            set { m_untilDate = value; }
        }

        private List<string> m_HandledChapters = new List<string>();
        public List<string> HandledChapters
        {
            get { return m_HandledChapters; }
            set { m_HandledChapters = value; }
        }

        public CourseInstanceTimeBlockModel()
        {

        }
    }
}