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