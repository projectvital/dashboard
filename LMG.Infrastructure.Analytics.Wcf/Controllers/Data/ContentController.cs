using System.Web.Http;
using LMG.Infrastructure.Analytics.Objects;
using OriginDatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LMG.Infrastructure.Analytics.Wcf.Models;
using LMG.Infrastructure.Analytics.Wcf.Objects.Helpers;
using LMG.Infrastructure.Analytics.Wcf.Objects.Types;

namespace LMG.Infrastructure.Analytics.Wcf.Controllers
{
    public class ContentController : BaseApiController
    {
        // GET api/<controller>/5
        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Count/{id}/{par}")]
        //public CountModel Post(string id, int? number)
        //{
        //}
        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Content/{id}/{number:int=0}")]
        //public CountModel Post(string id, int number)
        //{
        //    return HandleCountRequest(id, number, CountType.Total);
        //}
        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Count/{id}/Median/{number:int=0}")]
        //public CountModel PostMedian(string id, int number)
        //{
        //    return HandleCountRequest(id, number, CountType.Median);
        //}
        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Count/{id}/Average/{number:int=0}")]
        //public CountModel PostAverage(string id, int number)
        //{
        //    return HandleCountRequest(id, number, CountType.Average);
        //}

        //private CountModel HandleCountRequest(string id, int number, CountType type)
        //{
        //    CheckAuthorisation();

        //    string cleanId = ("" + id).ToLower();
        //    CountModel result = null;
        //    switch (cleanId)
        //    {
        //        //case "completedexercises": result = GetCountCompletedExercisesWithMinimumScore(number, type); break;
        //        //case "playedaudio": result = GetCountPlayedAudio(type); break;
        //        //case "printedpdf": result = GetCountPrintedPdf(type); break;
        //        //case "recordedvoice": result = GetCountRecordedVoice(type); break;
        //        //case "completedassessments": result = GetCountCompletedAssessmentsWithMinimumScore(number, type); break;
        //        //case "viewedexamples": result = GetCountViewedExamples(type); break;
        //        //case "accessedresources": result = GetCountAccessedResources(type); break;
        //        //case "posts": result = GetCountPosts(type); break;
        //        //case "accessedfeedback": result = GetCountAccessedFeedback(type); break;
        //        default: break;
        //    }

        //    //completed exercise: UvA: > 55%

        //    if (result == null)
        //        return new CountModel() { Count = -1, Name = "Invalid request" };

        //    if (string.IsNullOrWhiteSpace(result.Name))
        //        result.Name = id + number;
        //    return result;
        //}



        #region Ranking of Exercises
        /*
         
         select top 10 LogMetadataActivityInCourse.LogActivityUrl, count(*), AVG(LogScore.Scaled) average
from logstatement
inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId
inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id
inner join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId
inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
and LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/complete'
group by LogMetadataActivityInCourse.LogActivityUrl
order by average asc


         
         */
        [System.Web.Http.Route("v1/Data/Content/ExercisesByDifficulty/")]
        //[System.Web.Http.Route("v1/Data/Content/ExercisesByDifficulty/{top:int=0}")]
        public CountModel GetExercisesByDifficulty(int? top = null)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string query = "";
            //if (type == CountType.Total)
            {
                string topQ = (top.HasValue) ? "top " + top : "";
                //string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
                query = @"select "+topQ+@" LogMetadataActivityInCourse.LogActivityUrl, count(*), AVG(LogScore.Scaled) average
                    from logstatement
                    inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId
                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                    inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id
                    inner join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId
                    inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
                    where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                    and LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/complete'
                    AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
                    " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                        ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                        ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                    group by LogMetadataActivityInCourse.LogActivityUrl
                    order by average asc
                ";
            }
            //else
            {

            }

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            //if (result.RowCount == 1)
            //{
            //    model.Count = result.GetInt32(0, 0, 0);
            //    //if (type == CountType.Average && !logAgentId.HasValue)
            //    //    model.Name = "Average ";
            //    model.Name += "Printed Pdfs";
            //}
            return model;
        }

        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/ExercisesByAmountOfRetakes/")]
        [System.Web.Http.Route("v1/Data/Content/ExercisesByAmountOfRetakes/{top:int=0}")]
        public List<RankingItemModel> GetExercisesByAmountOfRetakes(int? top = null)
        {
            string query = GetExercisesByAmountOfRetakes_Query(top);
            
            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                model.Name = result.GetString(i, 0, "");
                model.Uri = result.GetString(i, 1, "");
                model.RetakeAverage = result.GetInt32(i, 2, 0);

                rankings.Add(model);
            }
            return rankings;
        }
        private string GetExercisesByAmountOfRetakes_Query(int? top, bool sort = true)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string query = "";
            string topQ = (top.HasValue) ? "top " + top : "";
            string sortOrder = (top.HasValue && top < 0) ? "asc" : "desc";
            //string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string activityTypeExercise = (GetPartnerMode() == "uhasselt") ? "ActivityType = 'Exercise'" : "lower(ActivityType) like '%exercise%'";

            query = @"select " + topQ + @" ";
            if (sort)
                query += "name, uri, avg(amount_retakes) as avg_amount_retakes";
            else
                query += @"uri, avg(amount_retakes) as avg_amount_retakes, sum(amount_retakes) as total_amount_retakes, null as avg_score, null as 'avg_duration', null as 'total_duration'";
            query += @" from 
                (
                    select " + ((sort)?"LogMetadataActivityInCourse.Name as name, ":"") + @"LogMetadataActivityInCourse.LogActivityUrl as uri, LogStatement.LogAgentId, count(*)-1 as amount_retakes
                    from logstatement
                    inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId
                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                    /*inner join LogActivityDefinitionDetail on LogActivityDefinitionDetail.LogActivityId = LogActivity.LogActivityId AND LogActivityDefinitionDetail.LogActivityDefinitionDetailTypeId = 1*/
                    inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id AND " + activityTypeExercise + @"
                    inner join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                    and LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/complete'
                    AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
                    " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                        ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                        ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                    group by "+ ((sort)?"LogMetadataActivityInCourse.Name, ":"") + @"LogMetadataActivityInCourse.LogActivityUrl, LogStatement.LogAgentId
                    having count(*) > 1
                ) sub
                group by " + ((sort) ? "name, " : "") + @"uri";
            if (sort)
                query += " order by avg(amount_retakes) desc, sum(amount_retakes) " + sortOrder;
            return query;
        }

        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/ExercisesByScore/")]
        [System.Web.Http.Route("v1/Data/Content/ExercisesByScore/{top:int=0}")]
        public List<RankingItemModel> GetExercisesByScore(int? top = null)
        {
            string query = GetExercisesByScore_Query(top);
            
            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                model.Name = result.GetString(i, 0, "");
                model.Uri = result.GetString(i, 1, "");
                model.ScoreAverage = (decimal)result.GetFloat64(i, 2, 0);

                rankings.Add(model);
            }
            return rankings;
        }
        private string GetExercisesByScore_Query(int? top, bool sort = true)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string query = "";
            string topQ = (top.HasValue) ? "top " + Math.Abs(top.Value) : "";
            string sortOrder = (top.HasValue && top < 0) ? "desc" : "asc";
            //string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string activityTypeExercise = (GetPartnerMode() == "uhasselt") ? "ActivityType = 'Exercise'" : "lower(ActivityType) like '%exercise%'";

            query = @"select " + topQ + @" ";
            if (sort)
                query += "name, uri, (avg(raw_score) + avg(scaled_score)) / 2 as avg_score, avg(raw_score), avg(scaled_score)";
            else
                query += "uri, null as avg_amount_retakes, null as total_amount_retakes, (avg(raw_score) + avg(scaled_score)) / 2 as avg_score, null as 'avg_duration', null as 'total_duration'";
            query += @" from 
                (
                    select " + ((sort) ? "LogMetadataActivityInCourse.Name as name, " : "") + @"LogMetadataActivityInCourse.LogActivityUrl as uri, LogStatement.LogAgentId, LogScore.Raw, logScore.Max, LogScore.Raw / logScore.Max as raw_score, LogScore.Scaled as scaled_score
                    from logstatement
                    inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId
                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                    /*inner join LogActivityDefinitionDetail on LogActivityDefinitionDetail.LogActivityId = LogActivity.LogActivityId AND LogActivityDefinitionDetail.LogActivityDefinitionDetailTypeId = 1*/
                    inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id AND " + activityTypeExercise + @"
                    inner join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId and LogResult.IsCompleted = 1
                    inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
                    where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                    and LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/complete'
                    AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
                    " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                        ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                        ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                ) sub
                group by " + ((sort) ? "name, " : "") + @"uri";
            if(sort)
                query += " order by ((avg(raw_score) + avg(scaled_score)) / 2) " + sortOrder;
            return query;
        }


        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/ExercisesByTimeSpent/")]
        [System.Web.Http.Route("v1/Data/Content/ExercisesByTimeSpent/{top:int=0}")]
        public List<RankingItemModel> GetExercisesByTimeSpent(int? top = null)
        {
            string query = GetExercisesByTimeSpent_Query(top);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                model.Name = result.GetString(i, 0, "");
                model.Uri = result.GetString(i, 1, "");
                model.TimeSpentAverage = result.GetNullableInt32(i, 2);
                model.TimeSpentTotal = result.GetNullableInt32(i, 3);

                rankings.Add(model);
            }
            return rankings;
        }
        private string GetExercisesByTimeSpent_Query(int? top, bool sort = true)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string partnerMode = GetPartnerMode();
            string query = "";
            string topQ = (top.HasValue) ? "top " + Math.Abs(top.Value) : "";
            string sortOrder = (top.HasValue && top < 0) ? "asc" : "desc";
            //string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string activityTypeExercise = (partnerMode == "uhasselt") ? "ActivityType = 'Exercise'" : "lower(ActivityType) like '%exercise%'";

            query = @"select " + topQ + @" ";
            if (sort)
                query += "name, uri, avg(timespent) as 'avg_duration', sum(timespent) as 'total_duration'";
            else
                query += "uri, null as avg_amount_retakes, null as total_amount_retakes, null as avg_score, avg(timespent) as 'avg_duration', sum(timespent) as 'total_duration'";
            query += @" from
                (
                    select " + ((sort) ? "LogMetadataActivityInCourse.Name as name, " : "") + @"LogActivity.Id as uri, LogStatement.LogAgentId, coalesce(instance_ext.Token, convert(nvarchar(MAX), Timestamp, 112)) as 'instance', sum(logstatementdetails.CorrectedTimespent) as 'timespent'
                    from
                    logstatement
                    inner join logstatementdetails on logstatementdetails.logstatementid = logstatement.logstatementid
                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                    " +((partnerMode == "uhasselt")?"inner":"left outer") + @" join logextension instance_ext on LogStatement.LogContextId = instance_ext.LogContextId AND instance_ext.uri = 'http://www.project-vital.eu/xapi/extension/instance-id'
                    ";                    
            if(false && partnerMode == "uhasselt")
                query += @" inner join LogStatementLink on LogStatementLink.LogStatementId = LogStatement.LogStatementId and LogStatementLink.TableName='Exercise'
                    left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.ActivityType = LogStatementLink.TableName AND LogMetadataActivityInCourse.ObjectId = LogStatementLink.TableId AND ActivityType = 'Exercise'";//--LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id -- and timeblock = (select min(timeblock) from LogMetadataActivityInCourse where LogActivityUrl = LogActivity.Id)
            else
                query += " left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id AND " + activityTypeExercise;// -- and timeblock = (select min(timeblock) from LogMetadataActivityInCourse where LogActivityUrl = LogActivity.Id)
            query += @" left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = logstatement.LogAgentId
                    where logstatement.Timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                    and LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"
                    " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                    ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                    group by " + ((sort) ? "LogMetadataActivityInCourse.Name, " : "") + @"LogActivity.Id, LogStatement.LogAgentId, coalesce(instance_ext.Token, convert(nvarchar(MAX), Timestamp, 112))
                ) isub
                group by " + ((sort) ? "name, " : "") + @"uri
		        ";
            if (sort)
                query += " order by avg(timespent) " + sortOrder;
            return query;

            #region old (commented) query with inline timespent calculation
//            query = @"select " + topQ + @" name, uri";
//            if (sort)
//                query += ", avg(timespent) as 'avg_duration', sum(timespent) as 'total_duration'";
//            else
//                query += ", null as avg_amount_retakes, null as total_amount_retakes, null as avg_score, avg(timespent) as 'avg_duration', sum(timespent) as 'total_duration'";
//            query += @" from
//                (
//                    select LogActivity.Id as 'uri', LogStatement.LogStatementId, LogActivityDefinition.LogActivityDefinitionId as 'LogActivityDefinitionId', LogActivityDefinition.Type as 'LogActivityDefinitionType',
//                    LogStatement.Timestamp, LogStatement.LogAgentId,
//                    "/*+ dbo.GetMinimum(*/ + @"
//                    datediff(SECOND,LogStatement.Timestamp, LEAD(LogStatement.TimeStamp,1,(select max(LogStatement.TimeStamp) from LogStatement inner join LogContext lc on LogStatement.LogContextId=lc.LogContextId where lc.Registration = LogContext.Registration)) 
//                        OVER (PARTITION BY LogContext.Registration ORDER BY LogStatement.TimeStamp))
//                    "/* , 3600) */ + @"
//                    as 'timespent'
//                    from logstatement
//                    inner join LogContext on LogStatement.LogContextId = LogContext.LogContextId
//                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
//                    inner join LogActivityDefinition on LogActivity.LogActivityDefinitionId = LogActivityDefinition.LogActivityDefinitionId
//                ) isub
//		        left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = isub.uri -- and timeblock = (select min(timeblock) from LogMetadataActivityInCourse where LogActivityUrl = LogActivity.Id)
//                left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
//		        inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = isub.LogAgentId
//                where isub.Timestamp > LogMetadataCourseInstance.FromDate and isub.timestamp < LogMetadataCourseInstance.UntilDate
//
//                and LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"
//                " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
//                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
//                    ((logAgentId.HasValue) ? " AND isub.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
//                group by name, uri
//		        ";
//            if(sort)
//                query += " order by avg(timespent) " + sortOrder;
            //            return query;
            #endregion


        }

        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/ExercisesByMultipleCriteria/")]
        [System.Web.Http.Route("v1/Data/Content/ExercisesByMultipleCriteria/{top:int=0}")]
        public List<RankingItemModel> GetExercisesByMultipleCriteria(int? top = null)
        {
            string query = GetExercisesByMultipleCriteria_Query(top);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                model.Name = result.GetString(i, 0, "");
                model.Uri = result.GetString(i, 1, "");
                model.RetakeAverage = result.GetNullableInt32(i, 2);
                model.RetakeTotal = result.GetNullableInt32(i, 3);
                model.ScoreAverage = (decimal?)result.GetNullableFloat64(i, 4);
                model.TimeSpentAverage = result.GetNullableInt32(i, 5);
                model.TimeSpentTotal = result.GetNullableInt32(i, 6);

                rankings.Add(model);
            }

            if (rankings.Count > 0)
            {
                int maxRetakeAverage = rankings.Max(x => (x.RetakeAverage.HasValue) ? x.RetakeAverage.Value : 0);
                int maxRetakeTotal = rankings.Max(x => (x.RetakeTotal.HasValue) ? x.RetakeTotal.Value : 0);
                int maxTimeSpentAverage = rankings.Max(x => (x.TimeSpentAverage.HasValue) ? x.TimeSpentAverage.Value : 0);
                int maxTimeSpentTotal = rankings.Max(x => (x.TimeSpentTotal.HasValue) ? x.TimeSpentTotal.Value : 0);
                rankings = rankings.OrderByDescending((x => x.GetCalculatedOrderFactorForExercises(maxRetakeAverage, maxRetakeTotal, maxTimeSpentAverage, maxTimeSpentTotal))).ToList();
            }
            rankings.RemoveAll(x => x.OrderFactor == 0);
            return rankings;
        }
        private string GetExercisesByMultipleCriteria_Query(int? top)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            string activityTypeExercise = (GetPartnerMode() == "uhasselt") ? "ActivityType = 'Exercise'" : "lower(ActivityType) like '%exercise%'";

            string query = @"select LogMetadataActivityInCourse.Name, uri, max(avg_amount_retakes), max(total_amount_retakes), max(avg_score), max(avg_duration), max(total_duration)
            from
            (
                select *
                from
                (
                    select LogMetadataActivityInCourse.LogActivityUrl as uri, null as avg_amount_retakes, null as total_amount_retakes, null as avg_score, null as 'avg_duration', null as 'total_duration'
                    from LogMetadataActivityInCourse
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                    where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"
                    AND " + activityTypeExercise + @"
                ) activity
                union
                (
                " + GetExercisesByAmountOfRetakes_Query(null, false) + @"
                ) 
                union
                (
                " + GetExercisesByScore_Query(null, false) + @"
                ) 
                union
                (
                " + GetExercisesByTimeSpent_Query(null, false) + @"
                ) 
            ) union_query
            inner join LogMetadataActivityInCourse on LogActivityUrl = union_query.uri
            group by LogMetadataActivityInCourse.Name,uri";
            
            return query;
        }

        #endregion
        #region Ranking of Theory Pages
        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/TheoryPagesByTimeSpent/")]
        [System.Web.Http.Route("v1/Data/Content/TheoryPagesByTimeSpent/{top:int=0}")]
        public List<RankingItemModel> GetTheoryPagesByTimeSpent(int? top = null)
        {
            string query = GetTheoryPagesByTimeSpent_Query(top);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                model.Name = result.GetString(i, 0, "");
                model.Uri = result.GetString(i, 1, "");
                model.TimeSpentAverage = result.GetNullableInt32(i, 2);
                model.TimeSpentTotal = result.GetNullableInt32(i, 3);

                rankings.Add(model);
            }
            return rankings;
        }
        private string GetTheoryPagesByTimeSpent_Query(int? top, bool sort = true)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string partnerMode = GetPartnerMode();
            string query = "";
            string topQ = (top.HasValue) ? "top " + Math.Abs(top.Value) : "";
            string sortOrder = (top.HasValue && top < 0) ? "asc" : "desc";
            //string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            //string activityTypeExercise = "ActivityType = 'TheoryPage'";// (partnerMode == "uhasselt") ? "ActivityType = 'TheoryPage'" : "lower(ActivityType) like '%exercise%'";
            string activityTypeExercise = (GetPartnerMode() == "uclan") ? "ActivityType IS NULL" : "ActivityType = 'TheoryPage'";

            query = @"select " + topQ + @" ";
            if (sort)
                query += "name, uri, avg(timespent) as 'avg_duration', sum(timespent) as 'total_duration'";
            else
                query += "uri, null as avg_amount_retakes, null as total_amount_retakes, null as avg_print_count, null as total_print_count, avg(timespent) as 'avg_duration', sum(timespent) as 'total_duration'";
            query += @" from
                (
                    select " + ((sort) ? "LogMetadataActivityInCourse.Name as name, " : "") + @"LogActivity.Id as uri, LogStatement.LogAgentId, sum(logstatementdetails.CorrectedTimespent) as 'timespent'
                    from
                    logstatement
                    inner join logstatementdetails on logstatementdetails.logstatementid = logstatement.logstatementid
                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                    left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id AND " + activityTypeExercise;// -- and timeblock = (select min(timeblock) from LogMetadataActivityInCourse where LogActivityUrl = LogActivity.Id)
            query += @" left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = logstatement.LogAgentId
                    where logstatement.Timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                    and LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"
                    " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                    ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                    group by " + ((sort) ? "LogMetadataActivityInCourse.Name, " : "") + @"LogActivity.Id, LogStatement.LogAgentId
                ) isub
                group by " + ((sort) ? "name, " : "") + @"uri
		        ";
            if (sort)
                query += " order by avg(timespent) " + sortOrder;
            return query;
        }

        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/TheoryPagesByAmountOfRetakes/")]
        [System.Web.Http.Route("v1/Data/Content/TheoryPagesByAmountOfRetakes/{top:int=0}")]
        public List<RankingItemModel> GetTheoryPagesByAmountOfRetakes(int? top = null)
        {
            string query = GetTheoryPagesByAmountOfRetakes_Query(top);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                model.Name = result.GetString(i, 0, "");
                model.Uri = result.GetString(i, 1, "");
                model.RetakeAverage = result.GetInt32(i, 2, 0);

                rankings.Add(model);
            }
            return rankings;
        }
        private string GetTheoryPagesByAmountOfRetakes_Query(int? top, bool sort = true)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string query = "";
            string topQ = (top.HasValue) ? "top " + top : "";
            string sortOrder = (top.HasValue && top < 0) ? "asc" : "desc";
            //string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
//            string activityTypeExercise = "ActivityType = 'TheoryPage'";// (GetPartnerMode() == "uhasselt") ? "ActivityType = 'TheoryPage'" : "lower(ActivityType) like '%exercise%'";
            string activityTypeExercise = (GetPartnerMode() == "uclan") ? "ActivityType IS NULL" : "ActivityType = 'TheoryPage'";

            query = @"select " + topQ + @" ";
            if (sort)
                query += "name, uri, avg(amount_retakes) as avg_amount_retakes";
            else
                query += @"uri, avg(amount_retakes) as avg_amount_retakes, sum(amount_retakes) as total_amount_retakes, null as avg_print_count, null as total_print_count, null as 'avg_duration', null as 'total_duration'";
            query += @" from 
                (
                    select " + ((sort) ? "LogMetadataActivityInCourse.Name as name, " : "") + @"LogMetadataActivityInCourse.LogActivityUrl as uri, LogStatement.LogAgentId, count(*)-1 as amount_retakes
                    from logstatement
                    inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId
                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                    inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id AND " + activityTypeExercise + @"
                    inner join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                    and LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/access'
                    AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
                    " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                        ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                        ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                    group by " + ((sort) ? "LogMetadataActivityInCourse.Name, " : "") + @"LogMetadataActivityInCourse.LogActivityUrl, LogStatement.LogAgentId
                    having count(*) > 1
                ) sub
                group by " + ((sort) ? "name, " : "") + @"uri";
            if (sort)
                query += " order by avg(amount_retakes) desc, sum(amount_retakes) " + sortOrder;
            return query;


            //3	http://activitystrea.ms/schema/1.0/access
            //18	https://w3id.org/xapi/adl/verbs/abandoned
        }


        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/TheoryPagesByAmountOfPdfPrints/")]
        [System.Web.Http.Route("v1/Data/Content/TheoryPagesByAmountOfPdfPrints/{top:int=0}")]
        public List<RankingItemModel> GetTheoryPagesByAmountOfPdfPrints(int? top = null)
        {
            string query = GetTheoryPagesByAmountOfPdfPrints_Query(top);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                model.Name = result.GetString(i, 0, "");
                model.Uri = result.GetString(i, 1, "");
                model.PdfPrintAverage = (decimal)result.GetFloat64(i, 2, 0); 
                model.PdfPrintTotal = result.GetInt32(i, 3, 0);

                rankings.Add(model);
            }
            return rankings;
        }
        private string GetTheoryPagesByAmountOfPdfPrints_Query(int? top, bool sort = true)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }

            string query = "";
            string topQ = (top.HasValue) ? "top " + Math.Abs(top.Value) : "";
            string sortOrder = "desc";// (top.HasValue && top < 0) ? "desc" : "asc";
            //string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            //string activityTypeExercise = (GetPartnerMode() == "uhasselt") ? "ActivityType = 'Exercise'" : "lower(ActivityType) like '%exercise%'";

       
            query = @"select " + topQ + @" ";
            if (sort)
                query += "name, uri, ROUND(avg(CAST(print_count AS FLOAT)),2), sum(print_count)";
            else
                query += "uri, null as avg_amount_retakes, null as total_amount_retakes, ROUND(avg(CAST(print_count AS FLOAT)),2) as avg_print_count, sum(print_count) as total_print_count, null as 'avg_duration', null as 'total_duration'";
            query += @" from 
                (
                    select " + ((sort) ? "name, " : "") + @"uri, logagentid, sum(print_count) as print_count from
	                (
	                    select * from
	                    (
		                    select " + ((sort) ? "LogMetadataActivityInCourse.Name as name, " : "") + @"LogMetadataActivityInCourse.LogActivityUrl as uri, LogMetadataAgentInCourseInstance.LogAgentId, 0 as print_count
		                    from LogMetadataActivityInCourse
		                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = 13
		                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId
		                    where LogMetadataActivityInCourse.ActivityType = 'TheoryPage' 
	                    ) X
	                    UNION
	                    (
		                    select " + ((sort) ? "LogMetadataActivityInCourse.Name as name, " : "") + @"LogMetadataActivityInCourse.LogActivityUrl as uri, LogStatement.LogAgentId, count(*) as print_count
		                    from logstatement 
		                    inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://www.project-vital.eu/xapi/verb/printed-to-pdf'
		                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
		                    inner join LogContextActivity on LogContextActivity.LogContextId = LogStatement.LogContextId AND LogContextActivityTypeId = 1
		                    inner join LogActivity parentActivity on LogContextActivity.LogActivityId = parentActivity.LogActivityId

		                    left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = parentActivity.Id AND ActivityType = 'TheoryPage' 
		                    left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
		                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = logstatement.LogAgentId
		                    where logstatement.Timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
		                    AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"     
                            " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                            ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                            ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
		                    group by " + ((sort) ? "LogMetadataActivityInCourse.Name, " : "") + @"LogMetadataActivityInCourse.LogActivityUrl, LogStatement.LogAgentId
	                    ) 
	                ) union_query
	                group by " + ((sort) ? "name, " : "") + @"uri, logagentid
                ) a
                group by " + ((sort) ? "name, " : "") + @"uri";

            if (sort)
                query += " order by ROUND(avg(CAST(print_count AS FLOAT)),2) " + sortOrder + ", sum(print_count) " + sortOrder;

            /*
            query = @"select " + topQ + @" ";
            if (sort)
                query += "name, uri, avg(print_count), sum(print_count)";
            else
                query += "uri, null as avg_amount_retakes, null as total_amount_retakes, avg(print_count) as avg_print_count, sum(print_count) as total_print_count, null as 'avg_duration', null as 'total_duration'";
            query += @" from 
                (
                    select " + ((sort) ? "LogMetadataActivityInCourse.Name as name, " : "") + @"LogMetadataActivityInCourse.LogActivityUrl as uri, LogStatement.LogAgentId, count(*) as print_count
                    from logstatement 
                    inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://www.project-vital.eu/xapi/verb/printed-to-pdf'
                    inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                    inner join LogContextActivity on LogContextActivity.LogContextId = LogStatement.LogContextId AND LogContextActivityTypeId = 1
                    inner join LogActivity parentActivity on LogContextActivity.LogActivityId = parentActivity.LogActivityId

                    left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = parentActivity.Id AND ActivityType = 'TheoryPage' 
                    left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = logstatement.LogAgentId
                    where logstatement.Timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                    AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
                    " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                        ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                        ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                    group by " + ((sort) ? "LogMetadataActivityInCourse.Name, " : "") + @"LogMetadataActivityInCourse.LogActivityUrl, LogStatement.LogAgentId
                ) sub
                group by " + ((sort) ? "name, " : "") + @"uri";
            if (sort)
                query += " order by avg(print_count) " + sortOrder + ", sum(print_count) " + sortOrder;*/


            return query;
        }

        #region (Commented) queries pdf print
        /*
         
         Amount of pdf prints:
         * 
select parentActivity.Id, count(*) 
from logstatement 
inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://www.project-vital.eu/xapi/verb/printed-to-pdf'
inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
inner join LogContextActivity on LogContextActivity.LogContextId = LogStatement.LogContextId AND LogContextActivityTypeId = 1
inner join LogActivity parentActivity on LogContextActivity.LogActivityId = parentActivity.LogActivityId

left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = parentActivity.Id AND ActivityType = 'TheoryPage' 
left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = logstatement.LogAgentId
where logstatement.Timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
and LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = 13
group by parentActivity.Id
         
         
         
         
         */
        /*
         Correction for averages (case where only 1 student does something. His total would be the average for the entire group (since other students don't match)).
         
select  name, uri, ROUND(avg(CAST(print_count AS FLOAT)),2), sum(print_count) from 
(
	select name, uri, logagentid, sum(print_count) as print_count from
	(
	select * from
	(
		select LogMetadataActivityInCourse.Name as name, LogMetadataActivityInCourse.LogActivityUrl as uri, LogMetadataAgentInCourseInstance.LogAgentId, 0 as print_count
		from LogMetadataActivityInCourse
		inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = 13
		inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId
		where LogMetadataActivityInCourse.ActivityType = 'TheoryPage' 
	) X
	UNION
	(
		select LogMetadataActivityInCourse.Name as name, LogMetadataActivityInCourse.LogActivityUrl as uri, LogStatement.LogAgentId, count(*) as print_count
		from logstatement 
		inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://www.project-vital.eu/xapi/verb/printed-to-pdf'
		inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
		inner join LogContextActivity on LogContextActivity.LogContextId = LogStatement.LogContextId AND LogContextActivityTypeId = 1
		inner join LogActivity parentActivity on LogContextActivity.LogActivityId = parentActivity.LogActivityId

		left outer join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = parentActivity.Id AND ActivityType = 'TheoryPage' 
		left outer join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
		inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = logstatement.LogAgentId
		where logstatement.Timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
		AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = 13    
                    
		group by LogMetadataActivityInCourse.Name, LogMetadataActivityInCourse.LogActivityUrl, LogStatement.LogAgentId
	) 
	) union_query
	group by name, uri, logagentid
--	order by uri, logagentid
) a
--where uri = 'https://student.commart.eu/Content/Show/1381/Theory/3752'
group by name, uri order by avg(print_count) desc, sum(print_count) desc


         
         
         
         */
        #endregion


        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/TheoryPagesByMultipleCriteria/")]
        [System.Web.Http.Route("v1/Data/Content/TheoryPagesByMultipleCriteria/{top:int=0}")]
        public List<RankingItemModel> GetTheoryPagesByMultipleCriteria(int? top = null)
        {
            string query = GetTheoryPagesByMultipleCriteria_Query(top);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                model.Name = result.GetString(i, 0, "");
                model.Uri = result.GetString(i, 1, "");
                model.RetakeAverage = result.GetNullableInt32(i, 2);
                model.RetakeTotal = result.GetNullableInt32(i, 3);
                model.PdfPrintAverage = (decimal?)result.GetNullableFloat64(i, 4);
                model.PdfPrintTotal = result.GetNullableInt32(i, 5);
                model.TimeSpentAverage = result.GetNullableInt32(i, 6);
                model.TimeSpentTotal = result.GetNullableInt32(i, 7);

                rankings.Add(model);
            }

            if (rankings.Count > 0)
            {
                int maxRetakeAverage = rankings.Max(x => (x.RetakeAverage.HasValue) ? x.RetakeAverage.Value : 0);
                int maxRetakeTotal = rankings.Max(x => (x.RetakeTotal.HasValue) ? x.RetakeTotal.Value : 0);
                decimal maxPdfPrintAverage = rankings.Max(x => (x.PdfPrintAverage.HasValue) ? x.PdfPrintAverage.Value : 0);
                int maxPdfPrintTotal = rankings.Max(x => (x.PdfPrintTotal.HasValue) ? x.PdfPrintTotal.Value : 0);
                int maxTimeSpentAverage = rankings.Max(x => (x.TimeSpentAverage.HasValue) ? x.TimeSpentAverage.Value : 0);
                int maxTimeSpentTotal = rankings.Max(x => (x.TimeSpentTotal.HasValue) ? x.TimeSpentTotal.Value : 0);
                rankings = rankings.OrderByDescending((x => x.GetCalculatedOrderFactorForTheoryPages(maxRetakeAverage, maxRetakeTotal, maxPdfPrintAverage, maxPdfPrintTotal, maxTimeSpentAverage, maxTimeSpentTotal))).ToList();
            }
            rankings.RemoveAll(x => x.OrderFactor == 0);
            return rankings;
        }
        private string GetTheoryPagesByMultipleCriteria_Query(int? top)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            string activityTypeExercise = "ActivityType = 'TheoryPage'";// (GetPartnerMode() == "uhasselt") ? "ActivityType = 'Exercise'" : "lower(ActivityType) like '%exercise%'";

            string query = @"select LogMetadataActivityInCourse.Name, uri, max(avg_amount_retakes), max(total_amount_retakes), max(avg_print_count), max(total_print_count), max(avg_duration), max(total_duration)
            from
            (
                select *
                from
                (
                    select LogMetadataActivityInCourse.LogActivityUrl as uri, null as avg_amount_retakes, null as total_amount_retakes, null as avg_print_count, null as total_print_count, null as 'avg_duration', null as 'total_duration'
                    from LogMetadataActivityInCourse
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                    where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"
                    AND " + activityTypeExercise + @"
                ) activity
                union
                (
                " + GetTheoryPagesByAmountOfRetakes_Query(null, false) + @"
                ) 
                union
                (
                " + GetTheoryPagesByAmountOfPdfPrints_Query(null, false) + @"
                ) 
                union
                (
                " + GetTheoryPagesByTimeSpent_Query(null, false) + @"
                ) 
            ) union_query
            inner join LogMetadataActivityInCourse on LogActivityUrl = union_query.uri
            group by LogMetadataActivityInCourse.Name,uri";

            return query;
        }

        //theory:
        // time spent, nbr of takes, nbr of retakes, pdf prints
        #endregion
        #region Ranking of Students

        [HttpPost]
        [System.Web.Http.Route("v1/Data/Content/StudentsByPerformance/{performanceGroup}")]
        [System.Web.Http.Route("v1/Data/Content/StudentsByPerformance/{performanceGroup}/{top:int=0}")]
        public List<RankingItemModel> GetStudentsByPerformance(string performanceGroup, int? top = null)
        {
            string query = GetStudentsByPerformance_Query(performanceGroup, top);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<RankingItemModel> rankings = new List<RankingItemModel>();
            for (int i = 0; i < result.RowCount; i++)
            {
                RankingItemModel model = new RankingItemModel();
                //model.Name = result.GetString(i, 0, "");
                model.Name = ""+result.GetInt64(i, 0, 0);
                if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UvA")
                    model.ScoreAverage = (decimal)result.GetFloat64(i, 1, 0);
                else
                    model.LastLogin = result.GetDateTime(i, 1, DateTime.MinValue);
                                
                rankings.Add(model);
            }
            return rankings;
        }
        private string GetStudentsByPerformance_Query(string performanceGroup, int? top, bool sort = true)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            //long? logAgentId = GetParameter<long>(parameters, "logagentid");
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            //if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }
            
            string query = "";
            string topQ = (top.HasValue) ? "top " + Math.Abs(top.Value) : "";
            string sortOrder = (performanceGroup == "A") ? "desc" : "asc";

            if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UvA")
            {
                query = @"select " + topQ + @" LogStatement.LogAgentId, avg(LogScore.Scaled)
                from logstatement
                inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId AND LogActivityDefinition.Type = 'http://adlnet.gov/expapi/activities/assessment'
                inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId
                inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
                inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId AND LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                AND LogMetadataAgentInCourseInstance.CalculatedPerformanceGroup = " + SqlValue.Convert(performanceGroup) + @"
                " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                    /*((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +*/ @"
                group by LogStatement.LogAgentId
                order by avg(LogScore.Scaled) " + sortOrder + @"
                ";
            }
            else
            {
                query = @"select " + topQ + @" logstatement.logagentid, max(timestamp)
                from logstatement
                inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'https://w3id.org/xapi/adl/verbs/logged-in'
                inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId AND LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                AND LogMetadataAgentInCourseInstance.CalculatedPerformanceGroup = " + SqlValue.Convert(performanceGroup) + @"
                " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                    /*((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +*/ @"
                group by logstatement.logagentid
                order by max(timestamp) " + sortOrder + @"
                ";
            }

            return query;
        }

        #endregion

        /*
        select LogActivity.Id, LogStatement.LogAgentId, count(*) from logstatement
        inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
        inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId
        inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id
        inner join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
        inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
        where LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/complete'
        group by LogActivity.Id, LogStatement.LogAgentId
        having count(*) > 1
        order by count(*) desc
         */

    }
}