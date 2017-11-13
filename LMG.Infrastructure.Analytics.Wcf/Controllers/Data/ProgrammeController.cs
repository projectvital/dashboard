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
    public class ProgrammeController : BaseApiController
    {
        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        [HttpPost]
        [System.Web.Http.Route("v1/Data/Programmes")]
        public List<ProgrammeModel> Post()
        {
            List<ProgrammeModel> list = new List<ProgrammeModel>();

            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            int? logagentid = GetParameter<int>(parameters, "logagentid");

            if (logagentid.HasValue && logagentid.Value == AccountController.ADMIN_LOGAGENT_ID)
                logagentid = null;

            string query = @"select distinct concat(LogMetadataCourseProgramme.Name, ' - ', [group]) as 'key', 
                        LogMetadataCourseProgramme.LogMetadataCourseProgrammeId, 
                        LogMetadataCourseProgramme.Name, 
                        [group] 
                from LogMetadataAgentInCourseInstance
                inner join LogMetadataCourseProgramme on LogMetadataCourseProgramme.LogMetadataCourseProgrammeId = LogMetadataAgentInCourseInstance.LogMetadataCourseProgrammeId" + @"
				where logmetadatacourseinstanceid = " + courseinstanceid + @"
                " + (logagentid.HasValue ? "and LogMetadataAgentInCourseInstance.logagentid = " + logagentid.Value : "") + @"
                order by LogMetadataCourseProgramme.Name, [group]
                ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            if(!logagentid.HasValue)//If a specific student is provided, do not give option for all programmes
                list.Add(new ProgrammeModel() { Id = "", Name = "{All programmes}"});
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    string name = result.GetString(i, 2, "");
                    string group = result.GetString(i, 3, "");
                    string key = "" + result.GetInt64(i, 1, 0) + (string.IsNullOrWhiteSpace(group) ? "" : "|" + group);
                    string label = name + (string.IsNullOrWhiteSpace(group) ? "" : " - " + group);

                    list.Add(new ProgrammeModel() { Id = key, Name = label });
                }
            }

            return list;
        }

        //[HttpPost]
        //[System.Web.Http.Route("v1/Data/Programme/{id}")]
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