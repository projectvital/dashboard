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
    public class CountController : BaseApiController
    {
        // GET api/<controller>/5
        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Count/{id}/{par}")]
        //public CountModel Post(string id, int? number)
        //{
        //}
        [HttpPost]
        [System.Web.Http.Route("v1/Data/Count/{id}/{number:int=0}")]
        public CountModel Post(string id, int number)
        {
            return HandleCountRequest(id, number, CountType.Total);
        }
        [HttpPost]
        [System.Web.Http.Route("v1/Data/Count/{id}/Median/{number:int=0}")]
        public CountModel PostMedian(string id, int number)
        {
            return HandleCountRequest(id, number, CountType.Median);
        }
        [HttpPost]
        [System.Web.Http.Route("v1/Data/Count/{id}/Average/{number:int=0}")]
        public CountModel PostAverage(string id, int number)
        {
            return HandleCountRequest(id, number, CountType.Average);
        }

        private CountModel HandleCountRequest(string id, int number, CountType type)
        {
            CheckAuthorisation();

            string cleanId = ("" + id).ToLower();
            CountModel result = null;
            switch (cleanId)
            {
                case "completedexercises": result = GetCountCompletedExercisesWithMinimumScore(number, type); break;
                case "playedaudio": result = GetCountPlayedAudio(type); break;
                case "printedpdf": result = GetCountPrintedPdf(type); break;
                case "recordedvoice": result = GetCountRecordedVoice(type); break;
                case "completedassessments": result = GetCountCompletedAssessmentsWithMinimumScore(number, type); break;
                case "viewedexamples": result = GetCountViewedExamples(type); break;
                case "accessedresources": result = GetCountAccessedResources(type); break;
                case "posts": result = GetCountPosts(type); break;
                case "accessedfeedback": result = GetCountAccessedFeedback(type); break;
                default: break;
            }

            //completed exercise: UvA: > 55%

            if (result == null)
                return new CountModel() { Count = -1, Name = "Invalid request" };

            if (string.IsNullOrWhiteSpace(result.Name))
                result.Name = id + number;
            return result;
        }

        private CountModel GetCountPrintedPdf(CountType type = CountType.Total)
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
                string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
                query = @"select " + operation + @"(counter)
                    from
                    (
                        select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                        from LogStatement
                        inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://www.project-vital.eu/xapi/verb/printed-to-pdf'
                        inner join LogContext on LogContext.LogContextId = LogStatement.LogContextId
                        inner join LogContextActivity on LogContextActivity.LogContextId = LogContext.LogContextId AND LogContextActivity.LogContextActivityTypeId=1
                        inner join LogActivity on LogActivity.LogActivityId = LogContextActivity.LogActivityId
                        inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl  = LogActivity.Id and timeblock = (select min(timeblock) from LogMetadataActivityInCourse lmaic where lmaic.LogActivityUrl = LogActivity.Id)
                        inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                        inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                        inner join LogExtension on LogExtension.LogActivityId = LogStatement.TargetLogActivityId and LogExtension.Uri = 'http://www.project-vital.eu/xapi/extension/mime-type' and LogExtension.Token = 'application/pdf'
                        where 1=1
                        AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
                        " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                            ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                            ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                        group by logstatement.logagentid
                    ) sub
                ";
            }
            //else
            {

            }

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            if (result.RowCount == 1)
            {
                model.Count = result.GetInt32(0, 0, 0);
                if (type == CountType.Average && !logAgentId.HasValue)
                    model.Name = "Average ";
                model.Name += "Printed Pdfs";
            }
            return model;
        }
        private CountModel GetCountAccessedFeedback(CountType type = CountType.Total)
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

            string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string query = @"select " + operation + @"(counter)
                    from
                    (
                        select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                        from LogStatement
                        inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://www.tincanapi.co.uk/verbs/evaluated'
                        inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                        "+((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                        group by logstatement.logagentid
                    ) sub
            ";
            //inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId and LogActivityDefinition.Type = 'http://activitystrea.ms/schema/1.0/page'
            //inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl  = LogActivity.Id and timeblock = (select min(timeblock) from LogMetadataActivityInCourse lmaic where lmaic.LogActivityUrl = LogActivity.Id)
               // inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
               // inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
               // where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
               // " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
               //((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            if (result.RowCount == 1)
            {
                model.Count = result.GetInt32(0, 0, 0);
                if (type == CountType.Average && !logAgentId.HasValue)
                    model.Name = "Average ";
                model.Name += "Accessed Feedback";
            }
            return model;
        }
        private CountModel GetCountPosts(CountType type = CountType.Total)
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

            string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string query = @"select " + operation + @"(counter)
                from
                (
                    select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                    from LogStatement
                    inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://adlnet.gov/expapi/verbs/commented'
                    inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                    inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId and LogActivityDefinition.Type = 'http://activitystrea.ms/schema/1.0/page'
                    inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl  = LogActivity.Id and timeblock = (select min(timeblock) from LogMetadataActivityInCourse lmaic where lmaic.LogActivityUrl = LogActivity.Id)
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
                    " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                   ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                   ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                    group by logstatement.logagentid
                ) sub
            ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            if (result.RowCount == 1)
            {
                model.Count = result.GetInt32(0, 0, 0);
                if (type == CountType.Average && !logAgentId.HasValue)
                    model.Name = "Average ";
                model.Name += "Posts";
            }
            return model;
        }
        private CountModel GetCountAccessedResources(CountType type = CountType.Total)
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

            string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string query = @"select " + operation + @"(counter)
                from
                (
                    select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                    from LogStatement
                    inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/access'
                    inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                    inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId and LogActivityDefinition.Type = 'http://activitystrea.ms/schema/1.0/page'
                    inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl  = LogActivity.Id and timeblock = (select min(timeblock) from LogMetadataActivityInCourse lmaic where lmaic.LogActivityUrl = LogActivity.Id)
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                    inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                    where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"    
                    "+((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                    ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +             
                    ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                    group by logstatement.logagentid
                ) sub
            ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            if (result.RowCount == 1)
            {
                model.Count = result.GetInt32(0, 0, 0);
                if (type == CountType.Average && !logAgentId.HasValue)
                    model.Name = "Average ";
                model.Name += "Accessed Resources";
            }
            return model;
        }
        private CountModel GetCountViewedExamples(CountType type = CountType.Total)
        {
            throw new NotImplementedException();
        }
        private CountModel GetCountCompletedAssessmentsWithMinimumScore(int percentage, CountType type = CountType.Total)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            float fPercentage = (float)percentage / 100;
            //string programmeKey = GetParameterString(parameters, "programmeid");
            //string group = null; long? programmeId = null;
            //if (!logAgentId.HasValue)
            //{//Only limit further on programme/group, if no agent was specified.
            //    programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            //}
            string partnermode = GetParameterString(parameters, "partnermode");

            string query = "";
            //if (partnermode == "uva")
            {
                string operation = (type == CountType.Total)? "sum" : (type == CountType.Average)? "avg" : "sum";

                query = @"select "+ operation +@"(counter)
                    from
                    (
                        select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                        from logstatement
                        inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                        inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId and LogActivityDefinition.Type = 'http://adlnet.gov/expapi/activities/assessment'
                        inner join logverb on LogVerb.LogVerbId = LogStatement.LogVerbId and LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/complete'
                        inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId
                        inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
                        where LogScore.Scaled >= " + Kernel.MakeSqlSafe(fPercentage) + @"
                        " + ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                        group by logstatement.logagentid
                    ) sub
                ";
            }
            //else
            //{
            //    return new CountModel();
            //}

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            if (result.RowCount == 1)
            {
                model.Count = result.GetInt32(0, 0, 0);
                if (type == CountType.Average && !logAgentId.HasValue)
                    model.Name = "Average ";
                model.Name += "Completed Assessments";
                if (percentage > 0)
                    model.Name += " (Score > " + percentage + "%)";
            }
            return model;
        }
        private CountModel GetCountCompletedAssessments(CountType type = CountType.Total)
        {
            return GetCountCompletedAssessmentsWithMinimumScore(0, type);
        }
        private CountModel GetCountRecordedVoice(CountType type = CountType.Total)
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

            string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string query = @"select " + operation + @"(counter)
                    from
                    (
                        select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                        from LogStatement
                        inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://www.project-vital.eu/xapi/verb/voice-recorded'
                        inner join LogContext on LogContext.LogContextId = LogStatement.LogContextId
                        inner join LogContextActivity on LogContextActivity.LogContextId = LogContext.LogContextId AND LogContextActivity.LogContextActivityTypeId=1
                        inner join LogActivity on LogActivity.LogActivityId = LogContextActivity.LogActivityId
                        inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl  = LogActivity.Id and timeblock = (select min(timeblock) from LogMetadataActivityInCourse lmaic where lmaic.LogActivityUrl = LogActivity.Id)
                        inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                        inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                        where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"                 
                        " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                        ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +             
                        ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                        group by logstatement.logagentid
                    ) sub
            ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            if (result.RowCount == 1)
            {
                model.Count = result.GetInt32(0, 0, 0);
                if (type == CountType.Average && !logAgentId.HasValue)
                    model.Name = "Average ";
                model.Name += "Recorded Voice Fragments";
            }
            return model;
        }
        private CountModel GetCountPlayedAudio(CountType type = CountType.Total)
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

            string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string query = @"select " + operation + @"(counter)
                    from
                    (
                        select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                        from LogStatement
                        inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/play'
                        inner join LogContext on LogContext.LogContextId = LogStatement.LogContextId
                        inner join LogContextActivity on LogContextActivity.LogContextId = LogContext.LogContextId AND LogContextActivity.LogContextActivityTypeId=1
                        inner join LogActivity on LogActivity.LogActivityId = LogContextActivity.LogActivityId
                        inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl  = LogActivity.Id and timeblock = (select min(timeblock) from LogMetadataActivityInCourse lmaic where lmaic.LogActivityUrl = LogActivity.Id)
                        inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                        inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                        inner join LogExtension on LogExtension.LogActivityId = LogStatement.TargetLogActivityId and LogExtension.Uri = 'http://www.project-vital.eu/xapi/extension/mime-type' and LogExtension.Token = 'audio/mpeg'
                        where 1=1
                        AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"                 
                        " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                        ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +                             
                        ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                        group by logstatement.logagentid
                    ) sub
            ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            if (result.RowCount == 1)
            {
                model.Count = result.GetInt32(0, 0, 0);
                if (type == CountType.Average && !logAgentId.HasValue)
                    model.Name = "Average ";
                model.Name += "Played Audio Fragments";
            }
            return model;
        }
        private CountModel GetCountCompletedExercisesWithMinimumScore(int percentage, CountType type = CountType.Total)
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            float fPercentage = (float)percentage / 100;
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = null;
            if (!logAgentId.HasValue)
            {//Only limit further on programme/group, if no agent was specified.
                programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
            }
            string partnermode = GetParameterString(parameters, "partnermode");

            string operation = (type == CountType.Total) ? "sum" : (type == CountType.Average) ? "avg" : "sum";
            string query = "";
            if (partnermode == "uva")
            {
                query = @"select " + operation + @"(counter)
                    from
                    (
                        select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                        from logstatement
                        inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                        inner join LogActivityDefinition on LogActivityDefinition.LogActivityDefinitionId = LogActivity.LogActivityDefinitionId and LogActivityDefinition.Type = 'http://adlnet.gov/expapi/activities/interaction'
                        inner join logverb on LogVerb.LogVerbId = LogStatement.LogVerbId and LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/complete'
                        inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId
                        inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
                        where 1=1
                        " + ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                        AND LogScore.Scaled >= " + Kernel.MakeSqlSafe(fPercentage) + @"
                        group by logstatement.logagentid
                    ) sub
                ";
            }
            else
            {
                query = @"select " + operation + @"(counter)
                    from
                    (
                        select logstatement.logagentid as 'logagentid', count(*) as 'counter'
                        from logstatement
                        inner join LogVerb on LogVerb.LogVerbId = LogStatement.LogVerbId AND LogVerb.Uri = 'http://activitystrea.ms/schema/1.0/complete'
                        inner join LogResult on LogResult.LogResultId = LogStatement.LogResultId
                        inner join LogScore on LogScore.LogScoreId = LogResult.LogScoreId
                        inner join LogActivity on LogActivity.LogActivityId = LogStatement.TargetLogActivityId
                        inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl  = LogActivity.Id and timeblock = (select min(timeblock) from LogMetadataActivityInCourse lmaic where lmaic.LogActivityUrl = LogActivity.Id)
                        inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                        inner join LogMetadataAgentInCourseInstance on LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId = LogMetadataCourseInstance.LogMetadataCourseInstanceId AND LogMetadataAgentInCourseInstance.LogAgentId = LogStatement.LogAgentId
                        where LogStatement.Timestamp >= LogMetadataCourseInstance.FromDate and LogStatement.Timestamp < LogMetadataCourseInstance.UntilDate
                        AND LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + @"                 
                        " + ((programmeId.HasValue) ? " and LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                            ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataAgentInCourseInstance.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataAgentInCourseInstance.[group] is null)" + System.Environment.NewLine : "") +
                            ((logAgentId.HasValue) ? " AND logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") + @"
                        AND LogScore.Scaled >= " + Kernel.MakeSqlSafe(fPercentage) + @"
                        group by logstatement.logagentid
                    ) sub
                    ";
            }

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            CountModel model = new CountModel();
            if (result.RowCount == 1)
            {
                model.Count = result.GetInt32(0, 0, 0);
                if (type == CountType.Average && !logAgentId.HasValue)
                    model.Name = "Average ";
                model.Name += "Completed Exercises";
                if (percentage > 0)
                    model.Name += " (Score > " + percentage + "%)";
            }
            return model;
        }
        private CountModel GetCountCompletedExercises(CountType type = CountType.Total)
        {
            return GetCountCompletedExercisesWithMinimumScore(0, type);
        }

        

        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Student/{id}")]
        //public string Post(string id)
        //{
        //    return "value";
        //}

        ////// POST api/<controller>
        ////public void Post([FromBody]string value)
        ////{
        ////}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}