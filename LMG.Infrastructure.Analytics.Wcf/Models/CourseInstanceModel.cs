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
    public class CourseInstanceModel
    {
        private long m_Id = -1;
        public long Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        private string m_name = null;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private string m_academicYear = null;
        public string AcademicYear
        {
            get { return m_academicYear; }
            set { m_academicYear = value; }
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

        public CourseInstanceModel()
        {

        }
    }
}