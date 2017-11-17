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

namespace LMG.Infrastructure.Analytics.Wcf.Controllers
{
    public class ClassController : BaseApiController
    {
        // GET api/<controller>/5
        [HttpPost]
        [System.Web.Http.Route("v1/Data/Classes")]
        public List<ClassModel> Post()
        {
            List<ClassModel> classes = new List<ClassModel>();

            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            string programmeKey = GetParameterString(parameters, "programmeid");
            string group = null; long? programmeId = ParameterHelper.ParseProgramme(programmeKey, ref group);
          

            string query = @"select LogMetadataCourseInstanceClass.LogMetadataCourseInstanceClassId, LogMetadataCourseInstanceClass.FromDate, LogMetadataCourseInstanceClass.UntilDate, LogMetadataCourseInstanceClassType.Name as 'ClassType', LogMetadataTeacher.Name as 'TeacherName', LogMetadataCourseProgramme.Name as 'ProgrammeName', LogMetadataCourseInstanceClass.[Group]
                from LogMetadataCourseInstanceClass
                inner join LogMetadataCourseInstanceClassType on LogMetadataCourseInstanceClassType.LogMetadataCourseInstanceClassTypeId = LogMetadataCourseInstanceClass.LogMetadataCourseInstanceClassTypeId
                inner join LogMetadataTeacher on LogMetadataTeacher.LogMetadataTeacherId = LogMetadataCourseInstanceClass.LogMetadataTeacherId
                inner join LogMetadataCourseProgramme on LogMetadataCourseProgramme.LogMetadataCourseProgrammeId = LogMetadataCourseInstanceClass.LogMetadataCourseProgrammeId
                where LogMetadataCourseInstanceClass.LogMetadataCourseInstanceId = " + courseinstanceid + " " +
                ((programmeId.HasValue) ? " and LogMetadataCourseInstanceClass.LogMetadataCourseProgrammeId = " + Kernel.MakeSqlSafe(programmeId) + System.Environment.NewLine : "") +
                ((!string.IsNullOrWhiteSpace(group)) ? " and (LogMetadataCourseInstanceClass.[group] = " + Kernel.MakeSqlSafe(group) + " or LogMetadataCourseInstanceClass.[group] is null)" + System.Environment.NewLine : "") + @"
                order by LogMetadataCourseInstanceClass.FromDate
            ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    classes.Add(new ClassModel() 
                    {
                        Id = result.GetInt64(i, 0, -1),
                        FromDate = result.GetDateTime(i, 1, DateTime.MinValue),
                        UntilDate = result.GetDateTime(i, 2, DateTime.MinValue),
                        ClassType = result.GetString(i, 3, ""),
                        TeacherName = result.GetString(i, 4, ""),
                        ProgrammeName = result.GetString(i, 5, ""),
                        Group = result.GetString(i, 6, "")
                    });
                }
            }

            return classes;
        }

        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Verb/{id}")]
        //public string Post(string id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

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