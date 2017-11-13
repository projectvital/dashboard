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