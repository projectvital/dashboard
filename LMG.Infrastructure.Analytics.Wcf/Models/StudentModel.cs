using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Models
{
    public class StudentModel
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

        private string m_programmeId = null;
        public string ProgrammeId 
        {
            get { return m_programmeId; }
            set { m_programmeId = value; }
        }

        private string m_group = null;
        public string Group
        {
            get { return m_group; }
            set { m_group = value; }
        }

        public StudentModel()
        {

        }
    }
}