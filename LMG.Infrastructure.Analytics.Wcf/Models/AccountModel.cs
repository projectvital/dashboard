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
    public class AccountModel
    {
        private long m_id = -1;
        public long Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_name = null;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private string m_sessionToken = null;
        public string SessionToken
        {
            get { return m_sessionToken; }
            set { m_sessionToken = value; }
        }

        private DateTime? m_expirationTimestamp = null;
        public DateTime? ExpirationTimestamp
        {
            get { return m_expirationTimestamp; }
            set { m_expirationTimestamp = value; }
        }

        private string m_partnerMode = null;
        public string PartnerMode
        {
            get { return m_partnerMode; }
            set { m_partnerMode = value; }
        }

        private List<CourseInstanceModel> m_courseInstances = new List<CourseInstanceModel>();
        public List<CourseInstanceModel> CourseInstances
        {
            get { return m_courseInstances; }
            set { m_courseInstances = value; }
        }
        
        public AccountModel()
        {

        }
    }
}