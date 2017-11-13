using LMG.Infrastructure.Analytics.Objects;
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
    public class ActivityByTimeOfDayController : BaseApiController
    {
        // GET api/<controller>
        //[HttpGet]
        [HttpPost]
        [System.Web.Http.Route("v1/ChartData/ActivityByTimeOfDay")]
        public HttpResponseMessage Post()
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            //long? courseinstanceid = GetParameter<long>(parameters, "courseinstanceid", true);
            //long? logAgentId = GetParameter<long>(parameters, "logagentid");

            string query = GetQuery(parameters); /*@"select Datepart(HOUR, statements.Timestamp) as 'hour', count(*) as 'count'
                from
                (
                select * from logstatement
                where logagentid in (select Student_x_StudentGroup.StudentId from Student_x_StudentGroup where studentgroupid = 901 and StudentId = logstatement.logagentid)
                and storedTimestamp is not null
                and logagentid in (select logagentid from logagentlrsfilter)
                and CAST(Timestamp as DATE) >= '2016-09-19'
                and CAST(Timestamp as DATE) <= '2017-02-15' " +
                ((logAgentId.HasValue) ? " and LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                @" ) as statements
                group by Datepart(HOUR, statements.Timestamp)
                order by Datepart(HOUR, statements.Timestamp)
                ";
            */


            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            Dictionary<int, int> countByHour = new Dictionary<int, int>();
            for (int i = 0; i < 24; i++)
                countByHour.Add(i, 0);//Initialize with counts of 0

            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    int hour = result.GetInt32(i, 0, 0);
                    if(!countByHour.ContainsKey(hour))//Should not occur, since dictionary is initialized with hours 0-23. Added nevertheless for error prevention.
                        countByHour.Add(hour, 0);
                    countByHour[hour] = result.GetInt32(i, 1, 0);
                }
            }

            string data = "hour,count";
            foreach(int hour in countByHour.Keys)
            {
                data += System.Environment.NewLine + hour + "," + countByHour[hour];
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
            //long? logVerbId = GetParameter<long>(parameters, "logverbid");

            string query = @"select Datepart(HOUR, statements.Timestamp) as 'hour', count(*) as 'count'
               from
               (
                   select  logstatement.timestamp, LogStatement.LogStatementId  from logstatement 
                   inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                   inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                   where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                   and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + " " +
                   ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                   ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                   ((logAgentId.HasValue) ? " and logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
              @" ) as statements
               group by Datepart(HOUR, statements.Timestamp)
               order by Datepart(HOUR, statements.Timestamp)
               ";

            /*string query = @"select CAST(Timestamp as DATE) as 'dt',coalesce(LogActivityDefinition.Name, concat(LogActivityDefinition.Type,'|',LogActivityDefinition.LogActivityDefinitionId)) as 'LogActivityDefinitionId',count(*) as 'count'
                from logstatement
                inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId
                inner join LogMetadataAgentInCourseInstance on logstatement.LogAgentId = LogMetadataAgentInCourseInstance.LogAgentId
                inner join LogMetadataCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                and CAST(StoredTimestamp as DATE) >= '2016-01-01'                
                and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + " " +
                ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                ((logAgentId.HasValue) ? " and logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                @" group by CAST(Timestamp as DATE), LogActivityDefinition.Name, LogActivityDefinition.Type, LogActivityDefinition.LogActivityDefinitionId
                order by dt";*/

            return query;
        }

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

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