using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Models
{
    public class ClassModel
    {
        private long m_id;
        public long Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private DateTime m_fromDate;
        public DateTime FromDate
        {
            get { return m_fromDate; }
            set { m_fromDate = value; }
        }

        private DateTime m_untilDate;
        public DateTime UntilDate
        {
            get { return m_untilDate; }
            set { m_untilDate = value; }
        }

        private string m_classType = null;
        public string ClassType
        {
            get { return m_classType; }
            set { m_classType = value; }
        }

        private string m_teacherName = null;
        public string TeacherName
        {
            get { return m_teacherName; }
            set { m_teacherName = value; }
        }

        private string m_programmeName = null;
        public string ProgrammeName
        {
            get { return m_programmeName; }
            set { m_programmeName = value; }
        }

        private string m_group= null;
        public string Group
        {
            get { return m_group; }
            set { m_group = value; }
        }

        public ClassModel()
        {

        }
    }
}