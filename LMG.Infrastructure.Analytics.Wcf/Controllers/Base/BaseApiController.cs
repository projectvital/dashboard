using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Objects.DB;
using LMG.Infrastructure.Analytics.Wcf.Objects.Exceptions;
using OriginDatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LMG.Infrastructure.Analytics.Wcf.Controllers
{
    public class BaseApiController : ApiController
    {
        public string GetRequestContent()
        {
            return Request.Content.ReadAsStringAsync().Result;
        }
        public Newtonsoft.Json.Linq.JObject GetRequestContent_JSON()
        {
            try
            {
                return Newtonsoft.Json.Linq.JObject.Parse(GetRequestContent());
            }
            catch
            {
                return new Newtonsoft.Json.Linq.JObject();
            }
        }

        public HttpResponseMessage ReturnData(string data, HttpStatusCode status = HttpStatusCode.OK)
        {
            var resp = new HttpResponseMessage(status);
            resp.Content = new StringContent(data, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        public bool IsParameterPresent(string key)
        {
            return IsParameterPresent(key, GetRequestContent_JSON());
        }
        public bool IsParameterPresent(string key, Newtonsoft.Json.Linq.JObject parameters)
        {
            return (parameters[key] != null);
        }

        public string GetParameterString(Newtonsoft.Json.Linq.JObject parameters, string name, bool throwErrorIfMissing = false)
        {
            string value = null;
            if (parameters[name] != null)
            {
                try
                {
                    value = parameters[name].ToString();
                }
                catch
                {
                    throw new MissingParameterException(name);
                }
            }
            if (string.IsNullOrEmpty(value) && throwErrorIfMissing)
                throw new MissingParameterException(name);

            return value;
        }
        public Nullable<T> GetParameter<T>(Newtonsoft.Json.Linq.JObject parameters, string name, bool throwErrorIfMissing = false) where T : struct
        {
            Nullable<T> value = null;
            if (parameters[name] != null)
            {
                try
                {
                    value = (T)Convert.ChangeType(parameters[name].ToString(), typeof(T));
                }
                catch
                {
                    throw new MissingParameterException(name);
                }
            }
            if (value == null && throwErrorIfMissing)
                throw new MissingParameterException(name);

            value = CheckAuthorisationForParameter<T>(parameters, name, value);

            return value;
        }

        public bool GetCourseInstanceDates(Newtonsoft.Json.Linq.JObject parameters, ref DateTime? from, ref DateTime? until)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            //long? timeblock = GetParameter<long>(parameters, "timeblock");
            //long? logAgentId = GetParameter<long>(parameters, "logagentid");
            //string programmeKey = GetParameterString(parameters, "programmeid");
            //string group = null; long? programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            //long? logVerbId = GetParameter<long>(parameters, "logverbid");

            string query = @"select CAST(fromdate as DATE), CAST(untildate as DATE) from LogMetadataCourseInstance       
                where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid;

            from = null;
            until = null;

            SqlResult result = Kernel.Connection.ExecuteQuery(query);
            if (result.RowCount == 1)
            {
                from = result.GetNullableDate(0, 0);
                until = result.GetNullableDate(0, 1);
            }

            return (from != null && until != null);
        }
        public long? GetCourseInstanceCurrentTimeBlock(long? courseinstanceid)
        {
            string query = @"select timeblock from LogMetadataCourseInstanceTimeBlock where fromdate < getdate() and untildate > getdate() and logmetadatacourseinstanceid = " + courseinstanceid;

            SqlResult result = Kernel.Connection.ExecuteQuery(query);
            if (result.RowCount > 0)
            {
                return result.GetInt32(0, 0, 0);

            }
            return null;//-1;//= Invalid timeblock. This will cause empty results (since there's nothing going on currently).
        }

        private Nullable<T> CheckAuthorisationForParameter<T>(Newtonsoft.Json.Linq.JObject parameters, string name, Nullable<T> value) where T : struct
        {
            if (name == "logagentid")
            {
                LogAgentDashboardSession session = GetAuthorisedSession(parameters, false);
                if (session != null)
                {
                    if (session.LogAgentId.HasValue)
                    {
                        if (value is long?)
                        {
                            long? lValue = (long)Convert.ChangeType(value, typeof(long));
                            if (lValue.HasValue && lValue.Value != session.LogAgentId)
                            {//Session is linked to a different LogAgent than the one provided in the request. Intercept and adapt to the correct LogAgent
                                return (T)Convert.ChangeType(session.LogAgentId.Value, typeof(T));
                            }
                        }
                    }
                }
            }
            return value;
        }
        public bool CheckAuthorisation(bool throwErrorOnFailure = true)
        {
            return CheckAuthorisation(GetRequestContent_JSON(), throwErrorOnFailure);
        }
        public bool CheckAuthorisation(Newtonsoft.Json.Linq.JObject parameters, bool throwErrorOnFailure = true)
        {
            string auth = GetParameterString(parameters, "auth", throwErrorOnFailure);

            LogAgentDashboardSession session = GetAuthorisedSession(auth);
            
            if (session != null)
            {
                return true;
            }

            if (throwErrorOnFailure)
                throw new InvalidSessionException();
            else
                return false;
        }
        private LogAgentDashboardSession GetAuthorisedSession(Newtonsoft.Json.Linq.JObject parameters, bool throwErrorOnFailure = false)
        {
            string auth = GetParameterString(parameters, "auth", throwErrorOnFailure);
            return GetAuthorisedSession(auth);
        }
        private LogAgentDashboardSession GetAuthorisedSession(string auth)
        {
            LogAgentDashboardSessionCollection sessions = new LogAgentDashboardSessionCollection();
            sessions.FillCollection("Token = " + Kernel.MakeSqlSafe(auth) + " and ExpirationTimestamp > GETDATE()", "");
            if (sessions.Count > 0)
                return sessions[0];
            else
                return null;
        }

        public string GetPartnerMode()
        {
            if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UvA")
                return "uva";
            else if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UCLan")
                return "uclan";
            else
                return "uhasselt";
        }
    }
}
