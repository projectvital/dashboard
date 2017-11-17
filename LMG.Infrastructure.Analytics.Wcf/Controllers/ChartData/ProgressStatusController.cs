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
    public class ProgressStatusController : BaseApiController
    {
        // GET api/<controller>
        //[HttpGet]
        [HttpPost]
        [System.Web.Http.Route("v1/ChartData/ProgressStatus")]
        public HttpResponseMessage Post()
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            string query = GetQuery(parameters);
            bool filterOnLogAgentId = IsParameterPresent("logagentid", parameters);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            Dictionary<int, int> buffer_total = new Dictionary<int, int>();
            Dictionary<int, int> buffer_count_allStudents = new Dictionary<int, int>();
            Dictionary<int, int> buffer_count_singleStudent = new Dictionary<int, int>();
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    int part_timeblock = result.GetInt32(i, 0, 0);
                    //int part_total = result.GetInt32(i, 1, 0);
                    int part_count = result.GetInt32(i, 1, 0);
                   
                    //buffer_total.Add(part_timeblock, part_total);
                    buffer_count_singleStudent.Add(part_timeblock, part_count);
                    if(!filterOnLogAgentId)
                        buffer_count_allStudents.Add(part_timeblock, part_count);
                }
            }

            query = GetQueryForAverage(parameters);
            result = Kernel.Connection.ExecuteQuery(query);
            Dictionary<int, int> buffer_average_allStudents = new Dictionary<int, int>();
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    int part_timeblock = result.GetInt32(i, 0, 0);
                    int part_average = result.GetInt32(i, 1, 0);

                    buffer_average_allStudents.Add(part_timeblock, part_average);
                }
            }

            query = GetQueryForTotal(parameters);
            result = Kernel.Connection.ExecuteQuery(query);
            //Dictionary<int, int> buffer_total_allStudents = new Dictionary<int, int>();
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    int part_timeblock = result.GetInt32(i, 0, 0);
                    int part_total = result.GetInt32(i, 1, 0);

                    buffer_total.Add(part_timeblock, part_total);
                }
            }

            if (filterOnLogAgentId)
            {
                query = GetQuery(parameters, true);

                result = Kernel.Connection.ExecuteQuery(query);
                if (result.RowCount > 0)
                {
                    for (int i = 0; i < result.RowCount; i++)
                    {
                        int part_timeblock = result.GetInt32(i, 0, 0);
                        //int part_total = result.GetInt32(i, 1, 0);
                        int part_count = result.GetInt32(i, 1, 0);
                        
                        buffer_count_allStudents.Add(part_timeblock, part_count);
                        if (!buffer_count_singleStudent.ContainsKey(part_timeblock))
                        {
                            //buffer_total.Add(part_timeblock, part_total);
                            buffer_count_singleStudent.Add(part_timeblock, 0);
                        }
                    }
                }
            }

            foreach (int key in buffer_total.Keys)
            {//rmenten 2017-06-23 - Fill collections with 0's in case no one handled content in a timeblock (can easily happen in very small groups)
                if (!buffer_count_allStudents.ContainsKey(key)) buffer_count_allStudents.Add(key, 0);
                if (!buffer_count_singleStudent.ContainsKey(key)) buffer_count_singleStudent.Add(key, 0);
                if (!buffer_average_allStudents.ContainsKey(key)) buffer_average_allStudents.Add(key, 0);
            }

            string data = "timeblock,total,groupcount,count,average";
            List<int> orderedList = buffer_total.Keys.ToList<int>();
            orderedList.Sort();
            foreach (int key in orderedList)
            {
                data += System.Environment.NewLine + key + "," + buffer_total[key] + "," + buffer_count_allStudents[key] + "," + buffer_count_singleStudent[key] + "," + buffer_average_allStudents[key];
            }

            return ReturnData(data);
        }

       

        private string GetQuery(Newtonsoft.Json.Linq.JObject parameters, bool ignoreLogAgentId = false)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? timeblock = null;
            if (GetParameterString(parameters, "timeblock") == "current")
                timeblock = GetCourseInstanceCurrentTimeBlock(courseinstanceid);
            else
                timeblock = GetParameter<long>(parameters, "timeblock");
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            long? logVerbId = GetParameter<long>(parameters, "logverbid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }
            
            //(select count(distinct sub.LogActivityUrl) from LogMetadataActivityInCourse sub where sub.TimeBlock = LogMetadataCourseInstanceTimeBlock.TimeBlock AND sub.LogMetadataCourseId = min(LogMetadataCourseInstance.LogMetadataCourseId) ) as 'total',
            string query = @"select LogMetadataCourseInstanceTimeBlock.TimeBlock as 'timeblock',
            count(distinct LogMetadataActivityInCourse.LogActivityUrl) as 'count'
            from logstatement 
                inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                inner join LogMetadataCourseInstanceTimeBlock on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstanceTimeBlock.LogMetadataCourseInstanceId
                inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                where LogMetadataCourseInstanceTimeBlock.TimeBlock = LogMetadataActivityInCourse.TimeBlock
                and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + " " +
                ((timeblock.HasValue) ? " and LogMetadataCourseInstanceTimeBlock.TimeBlock = " + timeblock.Value + System.Environment.NewLine : "") +
                ((logAgentId.HasValue && !ignoreLogAgentId) ? " and logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                ((logVerbId.HasValue) ? " and logstatement.LogVerbId = " + logVerbId.Value + System.Environment.NewLine : "") +
                @" group by LogMetadataCourseInstanceTimeBlock.TimeBlock
                order by LogMetadataCourseInstanceTimeBlock.TimeBlock
                ";


            //////Add this line to the query to limit results to content processed in the intended week:
            //logstatement.timestamp > LogMetadataCourseInstanceTimeBlock.FromDate and logstatement.timestamp < LogMetadataCourseInstanceTimeBlock.UntilDate
            //    and 

            return query;
        }
        private string GetQueryForTotal(Newtonsoft.Json.Linq.JObject parameters)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? timeblock = null;
            if (GetParameterString(parameters, "timeblock") == "current")
                timeblock = GetCourseInstanceCurrentTimeBlock(courseinstanceid);
            else
                timeblock = GetParameter<long>(parameters, "timeblock");

            string query = @"select LogMetadataActivityInCourse.TimeBlock, count(distinct LogMetadataActivityInCourse.LogActivityUrl) 
                from LogMetadataActivityInCourse 
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @" 
                " + ((timeblock.HasValue) ? " and LogMetadataActivityInCourse.TimeBlock = " + timeblock.Value + System.Environment.NewLine : "") + @"
                AND LogMetadataActivityInCourse.TimeBlock is not null
                group by LogMetadataActivityInCourse.TimeBlock
                order by LogMetadataActivityInCourse.TimeBlock";
            
            return query;
        }
        private string GetQueryForAverage(Newtonsoft.Json.Linq.JObject parameters, bool ignoreLogAgentId = false)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? timeblock = null;
            if (GetParameterString(parameters, "timeblock") == "current")
                timeblock = GetCourseInstanceCurrentTimeBlock(courseinstanceid);
            else
                timeblock = GetParameter<long>(parameters, "timeblock");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            //if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }
            //long? logAgentId = GetParameter<long>(parameters, "logagentid");
            //long? logVerbId = GetParameter<long>(parameters, "logverbid");

            string query = @"select timeblock, avg(count) as 'average'
                from
                (
                select LogMetadataCourseInstanceTimeBlock.TimeBlock as 'timeblock',logstatement.LogAgentId as 'logagentid',
                count(distinct LogMetadataActivityInCourse.LogActivityUrl) as 'count'
                from logstatement 
                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                    inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                    inner join LogMetadataCourseInstanceTimeBlock on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstanceTimeBlock.LogMetadataCourseInstanceId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    where LogMetadataCourseInstanceTimeBlock.TimeBlock = LogMetadataActivityInCourse.TimeBlock
                    and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + " " +
                    ((timeblock.HasValue) ? " and LogMetadataCourseInstanceTimeBlock.TimeBlock = " + timeblock.Value + System.Environment.NewLine : "") +
                    ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
	                @" group by LogMetadataCourseInstanceTimeBlock.TimeBlock, logstatement.LogAgentId
                ) sub
                group by timeblock
                order by timeblock
                ";
            
            //////Add this line to the query to limit results to content processed in the intended week:
            //logstatement.timestamp > LogMetadataCourseInstanceTimeBlock.FromDate and logstatement.timestamp < LogMetadataCourseInstanceTimeBlock.UntilDate
            //    and 

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