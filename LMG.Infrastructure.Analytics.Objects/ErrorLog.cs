using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LMG.Infrastructure.Analytics.Objects
{
    [Serializable]
    public class ErrorLog : Exception
    {
        public string Message = "";

        //[XmlElement]
        //public string Message
        //{
        //    get { return m_message; }
        //    set { m_message = value; }
        //}
        public Exception Exception = null;
        //[XmlElement]
        //public Exception Exception
        //{
        //    get { return m_exception; }
        //    set { m_exception = value; }
        //}

        public ErrorLog(string test)
            :this(test, null)
        {
            
        }
        public ErrorLog(string test, Exception ex)
        {
            Message = test;
            Exception = ex;
        }
        public ErrorLog(Exception ex)
        {
            if (ex != null)
            {
                Message = ex.Message;
                Exception = ex;

                try
                {
                    string sSource;
                    string sLog;
                    string sEvent;

                    sSource = "LMG.Infrastructure.Analytics::AnalyticsWorker";
                    sLog = "Application";
                    sEvent = Message + " - " + ex.StackTrace + " " + ((ex.InnerException != null)?ex.InnerException.Message +"-"+ ex.StackTrace:"");

                    if (!System.Diagnostics.EventLog.SourceExists(sSource))
                        System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);
                    System.Diagnostics.EventLog.WriteEntry(sSource, sEvent, System.Diagnostics.EventLogEntryType.Error, 666);
                }
                catch
                {
                }
            }
        }

        public void Throw()
        {
            throw new Exception(Message, Exception);
        }
        public string ToExtendedString()
        {
            string str = Message + System.Environment.NewLine;
            Exception ex = Exception;
            while(ex != null)
            {
                str += Exception.Message + System.Environment.NewLine + Exception.StackTrace + System.Environment.NewLine;
                ex = ex.InnerException;
            }
            return str;
        }
    }
}
