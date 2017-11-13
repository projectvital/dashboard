using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Models
{
    public class CountModel
    {
        private long m_count;
        public long Count
        {
            get { return m_count; }
            set { m_count = value; }
        }

        private string m_name = null;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public CountModel()
        {

        }
    }
}