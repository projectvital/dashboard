/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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
    public class ActivityTypesByDayController : BaseApiController
    {
        // GET api/<controller>
        //[HttpGet]
        [HttpPost]
        [System.Web.Http.Route("v1/ChartData/ActivityTypesByDay")]
        public HttpResponseMessage Post()
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            string query = GetQuery(parameters);
            //bool filterOnLogAgentId = IsParameterPresent("logagentid", parameters);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            Dictionary<DateTime, string> dictionary = new Dictionary<DateTime, string>();
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    dictionary.Add(result.GetDate(i, 0, DateTime.MinValue), "" + result.GetInt32(i, 1, 0) + "," + result.GetInt32(i, 2, 0) + "," + result.GetInt32(i, 3, 0));
                    //data += System.Environment.NewLine + result.GetDate(i, 0, DateTime.MinValue).ToShortDateString() + "," +
                    //    result.GetInt32(i, 1, 0) + "," +
                    //    result.GetInt32(i, 2, 0) + "," +
                    //    result.GetInt32(i, 3, 0);
                }
            }

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
                    dictionary.Add(d, "0,0,0");
            }
            List<DateTime> keys = dictionary.Keys.ToList();
            keys.Sort();

            string data = "date,amount_accessed_exercises,amount_completed_exercises,amount_accessed_theorypages";
            foreach (DateTime date in keys)
            {
                data += System.Environment.NewLine + date.ToString("dd/MM/yyyy") + "," + dictionary[date];
            }

            return ReturnData(data);
        }
        private string GetQuery(Newtonsoft.Json.Linq.JObject parameters, bool ignoreLogAgentId = false)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            //long? timeblock = GetParameter<long>(parameters, "timeblock");
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            //long? logVerbId = GetParameter<long>(parameters, "logverbid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string query = @"select dt, max(access_ex),  max(complete_ex),  max(access_tp)
                from
                (
                    select cast(LogStatement.Timestamp as DATE) as 'dt', NULL as 'access_ex', null as 'complete_ex', null as 'access_tp'
                    from logstatement						
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                    where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate						
                    and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"						
                    group by cast(LogStatement.Timestamp as DATE)

                    UNION     

                    select cast(LogStatement.Timestamp as DATE) as 'dt', count(distinct ext_instance.Token) as 'access_ex', null as 'complete_ex', null as 'access_tp'
                    from logstatement						
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                    inner join LogExtension ext_instance on ext_instance.Uri='http://www.project-vital.eu/xapi/extension/instance-id' and ext_instance.LogContextId = LogStatement.LogContextId						
                    inner join LogStatementLink link_exercise on link_exercise.TableName = 'Exercise' and link_exercise.LogStatementId = LogStatement.LogStatementId	
                    where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate						
                    and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"						
                    and LogStatement.LogVerbId = 3" +
                    ((logAgentId.HasValue) ? " and LogStatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                    ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                    @" group by cast(LogStatement.Timestamp as DATE)

                    union

                    select cast(LogStatement.Timestamp as DATE) as 'dt', null as 'access_ex', count(distinct ext_instance.Token) as 'complete_ex', null as 'access_tp'
                    from logstatement						
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                    inner join LogExtension ext_instance on ext_instance.Uri='http://www.project-vital.eu/xapi/extension/instance-id' and ext_instance.LogContextId = LogStatement.LogContextId						
                    inner join LogStatementLink link_exercise on link_exercise.TableName = 'Exercise' and link_exercise.LogStatementId = LogStatement.LogStatementId	
                    where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate						
                    and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"						
                    and LogStatement.LogVerbId = 4" +
                    ((logAgentId.HasValue) ? " and LogStatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                    ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                    @" group by cast(LogStatement.Timestamp as DATE)

                    union

                    select cast(LogStatement.Timestamp as DATE) as 'dt', null as 'access_ex', null as 'complete_ex', count(*) as 'access_tp'
                    from logstatement						
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                    inner join LogStatementLink link_exercise on link_exercise.TableName = 'TheoryPage' and link_exercise.LogStatementId = LogStatement.LogStatementId	
                    left outer join LogExtension ext on ext.LogActivityId = logstatement.TargetLogActivityId and ext.uri = 'http://www.project-vital.eu/xapi/extension/access-type' and ext.Token = 'TheoryPage'
                    where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate						
                    and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"						
                    and LogStatement.LogVerbId = 3" +
                    ((logAgentId.HasValue) ? " and LogStatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                    ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                    @" group by cast(LogStatement.Timestamp as DATE)
                ) sub
                group by dt
                order by dt asc";

            return query;
        }
        private string GetQuery_old(Newtonsoft.Json.Linq.JObject parameters, bool ignoreLogAgentId = false)
        {
            //int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            //long? timeblock = GetParameter<long>(parameters, "timeblock");
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            //long? logVerbId = GetParameter<long>(parameters, "logverbid");

            string query = @"select dates.dt date, coalesce(accesses.cnt,0) amount_accessed,coalesce(completions.cnt, 0) amount_completed,coalesce(theoryaccesses.cnt, 0) amount_accessed_theory
                from
                (
                /*Dates*/
                select distinct CAST(Timestamp as DATE) dt
                from LogStatement_DatasetVital201701 statements 
                ) as dates
                left outer join 
                (
                /*Exercises accessed*/
                select CAST(Timestamp as DATE) dt, count(distinct instanceID.Token) cnt
                from LogStatement_DatasetVital201701 statements
                inner join LogActivity on LogActivity.LogActivityId = statements.TargetLogActivityId
                inner join LogActivityDefinition on LogActivity.LogActivityDefinitionId = LogActivityDefinition.LogActivityDefinitionId 
                inner join LogExtension instanceID on instanceID.LogContextId = statements.LogContextId and instanceID.uri = 'http://www.project-vital.eu/xapi/extension/instance-id'
                where
                (
                LogActivityDefinition.Type = 'http://adlnet.gov/expapi/activities/interaction'
                )
                and LogVerbId = 3" +
                ((logAgentId.HasValue) ? " and statements.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                @" group by CAST(Timestamp as DATE)
                ) as accesses on (accesses.dt = dates.dt or accesses.dt is null)
                left outer join
                (
                /*Exercises completed*/
                select CAST(Timestamp as DATE) dt, count(distinct instanceID.Token) cnt
                from LogStatement_DatasetVital201701 statements 
                inner join LogActivity on LogActivity.LogActivityId = statements.TargetLogActivityId
                inner join LogActivityDefinition on LogActivity.LogActivityDefinitionId = LogActivityDefinition.LogActivityDefinitionId
                inner join LogExtension instanceID on instanceID.LogContextId = statements.LogContextId and instanceID.uri = 'http://www.project-vital.eu/xapi/extension/instance-id'
                where 
                (
                LogActivityDefinition.Type = 'http://adlnet.gov/expapi/activities/interaction'
                )
                and LogVerbId = 4" +
                ((logAgentId.HasValue) ? " and statements.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                @" group by CAST(Timestamp as DATE)
                ) as completions on (completions.dt = dates.dt or completions.dt is null)
                left outer join
                (
                /*Exercises completed*/
                select CAST(Timestamp as DATE) dt, count(*) cnt
                from LogStatement_DatasetVital201701 statements 
                inner join LogActivity on LogActivity.LogActivityId = statements.TargetLogActivityId
                inner join LogActivityDefinition on LogActivity.LogActivityDefinitionId = LogActivityDefinition.LogActivityDefinitionId
                inner join LogExtension ext on ext.LogActivityId = LogActivity.LogActivityId and ext.uri = 'http://www.project-vital.eu/xapi/extension/access-type' and ext.Token = 'TheoryPage'
                where 
                (
                LogActivityDefinition.Type = 'http://activitystrea.ms/schema/1.0/page'
                )
                and LogVerbId = 3" +
                ((logAgentId.HasValue) ? " and statements.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                @" group by CAST(Timestamp as DATE)
                ) as theoryaccesses on (theoryaccesses.dt = dates.dt or theoryaccesses.dt is null)
                where coalesce(accesses.cnt,0)  > 0 or coalesce(completions.cnt, 0)  > 0 or coalesce(theoryaccesses.cnt, 0)  > 0
                order by dates.dt asc";

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