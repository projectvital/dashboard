using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Wcf.Models;
using LMG.Infrastructure.Analytics.Wcf.Objects.Exceptions;
using LMG.Infrastructure.Analytics.Wcf.Objects.Helpers;
using OriginDatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Http;

namespace LMG.Infrastructure.Analytics.Wcf.Controllers
{
    public class TimeSpentByChapterController : BaseApiController
    {
        // GET api/<controller>
        //[HttpGet]
        [HttpPost]
        [System.Web.Http.Route("v1/ChartData/TimeSpentByChapter")]
        public HttpResponseMessage Post()
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            
            //bool filterOnLogAgentId = IsParameterPresent("logagentid", parameters);
            
            Dictionary<string, int> chapter_to_exercise_duration = new Dictionary<string,int>();
            Dictionary<string, int> chapter_to_average_duration = new Dictionary<string, int>();
            List<ChapterModel> chapters = ChapterController.GetChaptersInOrder(courseinstanceid.Value);
            foreach (ChapterModel chapter in chapters)
            {
                chapter_to_exercise_duration.Add(chapter.Name, 0);
                chapter_to_average_duration.Add(chapter.Name, 0);
            }

            string query = GetQuery(parameters);
            SqlResult result = Kernel.Connection.ExecuteQuery(query);
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    string chapter = result.GetString(i, 0, "<unknown chapter>");

                    if (!chapter_to_exercise_duration.ContainsKey(chapter))
                        chapter_to_exercise_duration.Add(chapter, 0);
                    chapter_to_exercise_duration[chapter] += result.GetInt32(i, 1, 0);
                }
            }

            string cacheKey_avg = this.GetType().FullName + "|" + courseinstanceid + "|AVG";
            Dictionary<string, int> cached_chapter_to_average_duration = null;// MemoryCacheHelper.Get(cacheKey_avg) as Dictionary<string, int>;
            if (cached_chapter_to_average_duration == null)
            {
                query = GetQueryForAverage(parameters);
                result = Kernel.Connection.ExecuteQuery(query);
                if (result.RowCount > 0)
                {
                    for (int i = 0; i < result.RowCount; i++)
                    {
                        string chapter = result.GetString(i, 0, "<unknown chapter>");

                        if (!chapter_to_average_duration.ContainsKey(chapter))
                            chapter_to_average_duration.Add(chapter, 0);
                        chapter_to_average_duration[chapter] += result.GetInt32(i, 1, 0);
                    }
                }

                MemoryCacheHelper.Set(cacheKey_avg, chapter_to_average_duration);
            }
            else
            {
                chapter_to_average_duration = cached_chapter_to_average_duration;
            }

            string data = "chapter,duration,duration_avg";
            foreach(string chapter in chapter_to_exercise_duration.Keys)
            {
                data += System.Environment.NewLine + "\"" + chapter + "\"," + chapter_to_exercise_duration[chapter] + "," + chapter_to_average_duration[chapter];
            }

            return ReturnData(data);
        }

        private string GetQuery(Newtonsoft.Json.Linq.JObject parameters, bool ignoreLogAgentId = false)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string query = @"select chapter as 'chapter', sum(timespent) as 'duration', LogActivityDefinitionId, LogActivityDefinitionType
            from
            (
            select LogMetadataActivityInCourse.Chapter as 'chapter', LogStatement.LogStatementId, LogActivityDefinition.LogActivityDefinitionId as 'LogActivityDefinitionId', LogActivityDefinition.Type as 'LogActivityDefinitionType',LogMetadataCourseInstance.LogMetadataCourseInstanceId,
            LogStatement.Timestamp, LogStatement.LogAgentId,
            "+//datediff(SECOND,LogStatement.Timestamp, LEAD(LogStatement.TimeStamp,1,(select max(LogStatement.TimeStamp) from LogStatement inner join LogContext lc on LogStatement.LogContextId=lc.LogContextId where lc.Registration = LogContext.Registration)) OVER (PARTITION BY LogContext.Registration ORDER BY LogStatement.TimeStamp)) as 'timespent'
            "LogStatementDetails.CorrectedTimespent as 'timespent'"+
            @"
            from logstatement
            inner join LogStatementDetails on LogStatement.LogStatementId = LogStatementDetails.LogStatementId
            inner join LogContext on LogStatement.LogContextId = LogContext.LogContextId
            inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
            inner join LogActivityDefinition on LogActivity.LogActivityDefinitionId = LogActivityDefinition.LogActivityDefinitionId
            left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id-- and timeblock = (select min(timeblock) from LogMetadataActivityInCourse where LogActivityUrl = LogActivity.Id)
            left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
            left outer join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = logstatement.LogAgentId
            where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
            " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
            ) sub
            where LogMetadataCourseInstanceId = " + courseinstanceid + @" 
            group by chapter, LogActivityDefinitionId, LogActivityDefinitionType
            order by chapter, LogActivityDefinitionId, LogActivityDefinitionType";


            /*rmenten 2017-05-19
             * using LEAD()
             *    **Question: only use this for theorypages? Or no limit at all.
             *    -- Remember the influence of long lasting sessions
             *    
             * //LEAD(LogStatement.TimeStamp) OVER (PARTITION BY LogContext.Registration ORDER BY LogStatement.TimeStamp) as 'lead', 
        */
            return query;
        }
        private string GetQueryForAverage(Newtonsoft.Json.Linq.JObject parameters)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            //long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            //if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }


            string query = @"select chapter as 'chapter', avg(duration) as 'avg_duration', LogActivityDefinitionId, LogActivityDefinitionType
                from
                (
                    select chapter as 'chapter', sum(timespent) as 'duration', LogActivityDefinitionId, LogActivityDefinitionType, logagentid
                    from
                    (
                        select LogMetadataActivityInCourse.Chapter as 'chapter', LogStatement.LogStatementId, LogActivityDefinition.LogActivityDefinitionId as 'LogActivityDefinitionId', LogActivityDefinition.Type as 'LogActivityDefinitionType',LogMetadataCourseInstance.LogMetadataCourseInstanceId,
                        LogStatement.Timestamp, LogStatement.LogAgentId,
                        "+//datediff(SECOND,LogStatement.Timestamp, LEAD(LogStatement.TimeStamp,1,(select max(LogStatement.TimeStamp) from LogStatement inner join LogContext lc on LogStatement.LogContextId=lc.LogContextId where lc.Registration = LogContext.Registration)) 
                                                     //OVER (PARTITION BY LogContext.Registration ORDER BY LogStatement.TimeStamp, LogMetadataActivityInCourse.TimeBlock)) as 'timespent'
                        "LogStatementDetails.CorrectedTimespent as 'timespent'" +
                        @"
                        from logstatement
                        inner join LogStatementDetails on LogStatement.LogStatementId = LogStatementDetails.LogStatementId
                        inner join LogContext on LogStatement.LogContextId = LogContext.LogContextId
                        inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                        inner join LogActivityDefinition on LogActivity.LogActivityDefinitionId = LogActivityDefinition.LogActivityDefinitionId
                        left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id-- and timeblock = (select min(timeblock) from LogMetadataActivityInCourse where LogActivityUrl = LogActivity.Id)
                        left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                        left outer join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = logstatement.LogAgentId
                        where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                        " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                        ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") + @"
                    ) isub
                    where LogMetadataCourseInstanceId = " + courseinstanceid + @" 
                    group by chapter, LogActivityDefinitionId, LogActivityDefinitionType, logagentid
                ) sub
                group by chapter, LogActivityDefinitionId, LogActivityDefinitionType
                order by chapter, LogActivityDefinitionId, LogActivityDefinitionType
            ";
            return query;
        }
        private string GetOriginalExercisesQuery(Newtonsoft.Json.Linq.JObject parameters, bool ignoreLogAgentId = false)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            //long? timeblock = GetParameter<long>(parameters, "timeblock");
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            //long? logVerbId = GetParameter<long>(parameters, "logverbid");

            string query = @"select chapter as 'chapter', sum(timespent) as 'duration'
                from
                (
                select LogMetadataActivityInCourse.Chapter as 'chapter', LogExtension.Token as 'token', datediff(SECOND, min(LogStatement.Timestamp), max(LogStatement.Timestamp, LogMetadataActivityInCourse.TimeBlock)) as 'timespent'
                from logstatement
                inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id
                inner join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                inner join LogExtension on LogExtension.LogContextId = LogStatement.LogContextId
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + " " +
                ((logAgentId.HasValue) ? " and logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                @" and LogExtension.Uri = 'http://www.project-vital.eu/xapi/extension/instance-id'
                group by LogMetadataActivityInCourse.Chapter, LogExtension.Token
                ) sub
                group by chapter";

            // or LogActivity.Id like LogMetadataActivityInCourse.LogActivityUrl + '?%'
            //min(LogStatement.Timestamp) as 'from', max(LogStatement.Timestamp) as 'until', 



            /*rmenten 2017-05-19
             * using LEAD()
             *    **Question: only use this for theorypages? Or no limit at all.
             *    -- Remember the influence of long lasting sessions
             
        select chapter as 'chapter', LogActivityDefinitionId, sum(timespent) as 'duration'
            from
            (
            select LogMetadataActivityInCourse.Chapter as 'chapter', LogStatement.LogStatementId, LogActivityDefinitionId,LogMetadataCourseInstance.LogMetadataCourseInstanceId,
            LogStatement.Timestamp, LogStatement.LogAgentId,
            LEAD(LogStatement.TimeStamp) OVER (PARTITION BY LogContext.Registration ORDER BY LogStatement.TimeStamp) as 'lead', 
            datediff(SECOND,LogStatement.Timestamp, LEAD(LogStatement.TimeStamp) OVER (PARTITION BY LogContext.Registration ORDER BY LogStatement.TimeStamp)) as 'timespent'
            from logstatement
            inner join LogContext on LogStatement.LogContextId = LogContext.LogContextId
            inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
            left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id-- and timeblock = (select min(timeblock) from LogMetadataActivityInCourse where LogActivityUrl = LogActivity.Id)
            left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
            where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
            and logagentid = 64771 --and CAST(Timestamp as DATE) = '2017-01-29'
            --order by timespent desc
            ) sub
            where 1=1
            --and sub.LogActivityDefinitionId = 4 and 
            and LogMetadataCourseInstanceId = 13
            group by chapter, LogActivityDefinitionId
            order by chapter, LogActivityDefinitionId
             
             
             */



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