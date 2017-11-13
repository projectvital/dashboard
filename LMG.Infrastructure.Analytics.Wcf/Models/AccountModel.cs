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