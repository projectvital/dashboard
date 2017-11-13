using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Models
{
    public class VerbModel
    {
        private long m_id;
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

        public VerbModel()
        {

        }
    }
}