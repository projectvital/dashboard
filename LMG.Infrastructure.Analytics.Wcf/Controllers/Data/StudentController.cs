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
    public class StudentController : BaseApiController
    {
        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        [HttpPost]
        [System.Web.Http.Route("v1/Data/Students")]
        public List<StudentModel> Post()
        {
            List<StudentModel> students = new List<StudentModel>();

            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            int? logagentid = GetParameter<int>(parameters, "logagentid");

            if (logagentid.HasValue && logagentid.Value == AccountController.ADMIN_LOGAGENT_ID)
                logagentid = null;

            string query = @"select logagent.logagentid as 'id', logagent.Name as 'name', LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId as 'programmeid', LogMetadataAgentInCourseInstance.[Group] as 'group'
				from LogMetadataAgentInCourseInstance 
				inner join logagent on logagent.LogAgentId = logmetadataagentincourseinstance.LogAgentId
				where logmetadatacourseinstanceid = " + courseinstanceid + @" 
                " + (logagentid.HasValue ? "and logagent.logagentid = " + logagentid.Value : "") + @"
                order by logagent.logagentid
                ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            students.Add(new StudentModel() { Id = -1, Name = "{All students}"});
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    students.Add(new StudentModel() { Id = result.GetInt64(i, 0, 0), Name = ""+result.GetInt64(i, 0, 0), ProgrammeId = ""+result.GetInt64(i, 2, -1), Group = result.GetString(i, 3, "") });
                }
            }

            return students;
        }

        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Student/{id}")]
        //public string Post(string id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}