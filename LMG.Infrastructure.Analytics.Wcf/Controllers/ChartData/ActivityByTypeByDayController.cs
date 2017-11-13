using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Wcf.Objects.Exceptions;
using LMG.Infrastructure.Analytics.Wcf.Objects.Helpers;
using OriginDatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LMG.Infrastructure.Analytics.Wcf.Controllers
{
    public class ActivityByTypeByDayController : BaseApiController
    {
        // GET api/<controller>
        //[HttpGet]
        [HttpPost]
        [System.Web.Http.Route("v1/ChartData/ActivityByTypeByDay")]
        public HttpResponseMessage Post()
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            string query = GetQuery(parameters);
            //bool filterOnLogAgentId = IsParameterPresent("logagentid", parameters);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            //string data = "date,type,count";
            //if (result.RowCount > 0)
            //{
            //    for (int i = 0; i < result.RowCount; i++)
            //    {
            //        data += System.Environment.NewLine + result.GetDate(i, 0, DateTime.MinValue).ToShortDateString() + "," +
            //            result.GetInt64(i, 1, 0) + "," +
            //            result.GetInt32(i, 2, 0);
            //    }
            //}

            Dictionary<DateTime, Dictionary<string, int>> dictionary = new Dictionary<DateTime, Dictionary<string, int>>();
            List<string> types = new List<string>();
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    DateTime dt = result.GetDate(i, 0, DateTime.MinValue);
                    if (!dictionary.ContainsKey(dt))
                        dictionary.Add(dt, new Dictionary<string, int>());

                    string type = result.GetString(i, 1, "");

                    if (!dictionary[dt].ContainsKey(type))
                        dictionary[dt].Add(type, result.GetInt32(i, 2, 0));

                    if (!types.Contains(type))
                        types.Add(type);
                }
            }

            types.Sort();

            string data = "date";
            foreach (string type in types)
            {
                data += ",\"" + type + "\"";
            }

            if (dictionary.Count > 0)
            {
                DateTime min, max;
                DateTime? from = null, until = null;
                if (GetCourseInstanceDates(parameters, ref from, ref until))
                {
                    min = from.Value;
                    max = until.Value;
                }
                else
                {
                    min = dictionary.Keys.Min();
                    max = dictionary.Keys.Max();
                }

                for (DateTime d = min; d <= max; d = d.AddDays(1))
                {
                    if (dictionary.ContainsKey(d))
                        continue;
                    else
                        dictionary.Add(d, new Dictionary<string, int>());
                }
                List<DateTime> keys = dictionary.Keys.ToList();
                keys.Sort();

                foreach (DateTime date in keys)
                {
                    data += System.Environment.NewLine + date.ToString("dd/MM/yyyy");

                    foreach (string type in types)
                    {
                        data += ",";
                        if (dictionary[date].ContainsKey(type))
                            data += dictionary[date][type];
                        else
                            data += "0";
                    }
                }
            }

            return ReturnData(data);
        }

        private string GetQuery(Newtonsoft.Json.Linq.JObject parameters, bool ignoreLogAgentId = false)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            //long? timeblock = GetParameter<long>(parameters, "timeblock");
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }
            string alternativeBasis = GetParameterString(parameters, "alternativebasis");
            bool verbMode = (alternativeBasis == "verb");
                 
            //long? logVerbId = GetParameter<long>(parameters, "logverbid");

            string query = @"select CAST(Timestamp as DATE) as 'dt',";
            if(verbMode)
                query += "LogVerbLabel.Label";
            else
                query += "coalesce(LogActivityDefinition.Name, concat(LogActivityDefinition.Type,'|',LogActivityDefinition.LogActivityDefinitionId))";
            query += " as 'LogActivityDefinitionId',count(*) as 'count'";
            query += @" from logstatement
                inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId";
            if (verbMode)
                query += @" inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId
                    inner join LogVerbLabel on LogVerb.LogVerbId = LogVerbLabel.LogVerbId and lower(LogVerbLabel.Language) = 'en-gb'";
            else
                query += " inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId";
            query += @" inner join LogMetadataAgentInCourseInstance on logstatement.LogAgentId = LogMetadataAgentInCourseInstance.LogAgentId
                inner join LogMetadataCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                and CAST(StoredTimestamp as DATE) >= '2016-01-01'                
                and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + " " +
                ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                ((logAgentId.HasValue) ? " and logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "");
            if (verbMode)
                query += @" group by CAST(Timestamp as DATE), LogVerbLabel.Label";
            else
                query += @" group by CAST(Timestamp as DATE), LogActivityDefinition.Name, LogActivityDefinition.Type, LogActivityDefinition.LogActivityDefinitionId";
            query += " order by dt";

            return query;
        }

        


        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}