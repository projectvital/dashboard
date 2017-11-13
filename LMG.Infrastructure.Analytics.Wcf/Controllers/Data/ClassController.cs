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