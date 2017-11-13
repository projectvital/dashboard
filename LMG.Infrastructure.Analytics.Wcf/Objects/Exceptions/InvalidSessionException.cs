using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Objects.Exceptions
{
    public class InvalidSessionException : Exception
    {
        //string m_missingParameterName = null;
        //public string MissingParameterName
        //{
        //    get { return m_missingParameterName; }
        //    set { m_missingParameterName = value; }
        //}

        public InvalidSessionException()//string missingParameterName)
        {
            //MissingParameterName = missingParameterName;
        }
    }
}