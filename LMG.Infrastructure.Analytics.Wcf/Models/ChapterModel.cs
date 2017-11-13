using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Models
{
    public class ChapterModel
    {
        private long m_orderId;
        public long OrderId
        {
            get { return m_orderId; }
            set { m_orderId = value; }
        }

        private string m_name = null;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public ChapterModel()
        {

        }
    }
}