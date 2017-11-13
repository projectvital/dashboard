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