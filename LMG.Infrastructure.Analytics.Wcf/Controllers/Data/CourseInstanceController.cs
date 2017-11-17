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

namespace LMG.Infrastructure.Analytics.Wcf.Controllers
{
    public class CourseInstanceController : BaseApiController
    {
        // GET api/<controller>/5
        [HttpPost]
        [System.Web.Http.Route("v1/Data/CourseInstances")]
        public List<CourseInstanceModel> Post()
        {
            List<CourseInstanceModel> courseInstances = new List<CourseInstanceModel>();

            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            int? logagentid = GetParameter<int>(parameters, "logagentid");

            if (logagentid.HasValue && logagentid.Value == AccountController.ADMIN_LOGAGENT_ID)
                logagentid = null;

            SqlQueryCustom query = new SqlQueryCustom(@"select distinct LogMetadataCourse.Name, LogMetadataCourseInstance.AcademicYear, LogMetadataCourseInstance.FromDate, LogMetadataCourseInstance.UntilDate, LogMetadataCourseInstance.LogMetadataCourseInstanceId
                    from LogMetadataAgentInCourseInstance
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                    inner join LogMetadataCourse on LogMetadataCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    " + ((logagentid.HasValue)? "where LogMetadataAgentInCourseInstance.LogAgentId = "+ logagentid.Value : "") +@"
                    ");

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            if (result.RowCount > 0)
            {
                for(int i = 0 ; i < result.RowCount ; i++)
                {
                    CourseInstanceModel courseInst = new CourseInstanceModel();
                    courseInst.Name = result.GetString(i, 0, "");
                    courseInst.AcademicYear = result.GetString(i, 1, "");
                    courseInst.FromDate = result.GetNullableDateTime(i, 2);
                    courseInst.UntilDate = result.GetNullableDateTime(i, 3);
                    courseInst.Id = result.GetInt64(i, 4, -1);

                    courseInstances.Add(courseInst);
                }
            }


            return courseInstances;
        }

        [HttpPost]
        [System.Web.Http.Route("v1/Data/CourseInstanceTimeBlocks")]
        public List<CourseInstanceTimeBlockModel> GetCourseInstanceTimeBlocks()
        {
            List<CourseInstanceTimeBlockModel> list = new List<CourseInstanceTimeBlockModel>();

            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);

            SqlQueryCustom query = new SqlQueryCustom(@"select distinct timeblock, chapter from LogMetadataActivityInCourse
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                    where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid.Value + @"
                    order by timeblock, chapter");
            SqlResult chapterResult = Kernel.Connection.ExecuteQuery(query);
            Dictionary<int, List<string>> chaptersInTimeblock = new Dictionary<int, List<string>>();
            for (int i = 0; i < chapterResult.RowCount; i++)
            {
                int timeblock = chapterResult.GetInt32(i, 0, -1);
                string chapter = chapterResult.GetString(i, 1, "");

                if (!chaptersInTimeblock.ContainsKey(timeblock))
                    chaptersInTimeblock.Add(timeblock, new List<string>());

                chaptersInTimeblock[timeblock].Add(chapter);
            }



            query = new SqlQueryCustom(@"select TimeBlock, FromDate, UntilDate FROM LogMetadataCourseInstanceTimeBlock
                    where LogMetadataCourseInstanceTimeBlock.LogMetadataCourseInstanceId = " + courseinstanceid.Value + @"
                    ");

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    CourseInstanceTimeBlockModel obj = new CourseInstanceTimeBlockModel();
                    obj.CourseInstanceId = courseinstanceid.Value;
                    obj.TimeBlock = result.GetInt32(i, 0, -1);
                    obj.FromDate = result.GetNullableDateTime(i, 1);
                    obj.UntilDate = result.GetNullableDateTime(i, 2);

                    if (chaptersInTimeblock.ContainsKey((int)obj.TimeBlock))
                    {
                        obj.HandledChapters.AddRange(chaptersInTimeblock[(int)obj.TimeBlock]);
                    }

                    list.Add(obj);
                }
            }

            return list;
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