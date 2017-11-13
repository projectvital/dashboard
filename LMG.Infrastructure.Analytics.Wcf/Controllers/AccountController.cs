using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Objects.DB;
using LMG.Infrastructure.Analytics.Wcf.Models;
using OriginDatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LMG.Infrastructure.Analytics.Wcf.Controllers
{
    public class AccountController : BaseApiController
    {

        public const long ADMIN_LOGAGENT_ID = -999;

        // POST api/<controller>
        [HttpPost]
        [System.Web.Http.Route("v1/Account/Login")]
        public AccountModel Login()
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            string username = GetParameterString(parameters, "username", true);
            string password = GetParameterString(parameters, "password");
            string partnermode = "uhasselt";// = GetParameterString(parameters, "partnermode");
            if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UvA")
                partnermode = "uva";
            else if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UCLan")
                partnermode = "uclan";


            LogAgentCollection matchingUsers = new LogAgentCollection();
            //important: Change login
            if (username.ToLower() == "admin" && password == "hardcodedPasswordForAdministrator")
            {
                matchingUsers.Add(new LogAgent() 
                {
                    LogAgentId = ADMIN_LOGAGENT_ID,
                    Name = "Administrator"
                });
            }
            else
            {
                bool superuser = false;

                //string passwordSalt = BCrypt.Net.BCrypt.GenerateSalt();
                //string password_crypt = BCrypt.Net.BCrypt.HashPassword(password ?? "", passwordSalt);

               
                if (partnermode == "uhasselt")
                {
                    StudentsCollection students = new StudentsCollection();
                    students.FillCollection("lower(EmailAddress) = " + SqlValue.Convert(username.ToLower()) + " OR concat('',Id) = " + SqlValue.Convert(username)
                        , "");

                    if (students.Count > 0)
                    {
                        string passwordSalt = students[0].PasswordSalt;
                        string password_crypt = BCrypt.Net.BCrypt.HashPassword(password ?? "", passwordSalt);

                        if (students[0].Password == password_crypt || superuser)
                        {
                            matchingUsers.Add(new LogAgent()
                            {
                                LogAgentId = students[0].Id,
                                Name = students[0].FirstName + " " + students[0].LastName
                            });
                        }
                    }
                }
                else
                {
                    string where = @"concat('',logagentid) = " + Kernel.MakeSqlSafe(username);
                    //Check unencrypted for now:
                    if (!superuser)
                        where += " and password = " + Kernel.MakeSqlSafe(password);

                    //where += " and coalesce(mbox, concat('',logagentid)) = " + Kernel.MakeSqlSafe(username)
                    //where += " and password = " + Kernel.MakeSqlSafe(password_crypt);


                    matchingUsers.FillCollection(where, "");
                }
            }

            AccountModel model = new AccountModel();

            if (matchingUsers.Count > 0)
            {
                DateTime now = DateTime.Now;
                DateTime expire = now.AddDays(1);

                model.Id = matchingUsers[0].LogAgentId;
                model.Name = matchingUsers[0].Name;
                model.SessionToken = Guid.NewGuid().ToString().Replace("-","");
                model.ExpirationTimestamp = expire;
                if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UvA")
                    model.PartnerMode = "uva";
                else if (Kernel.Connection.directOdbcConnection.Connection.Database == "VITAL.UCLan")
                    model.PartnerMode = "uclan";
                else
                    model.PartnerMode = "uhasselt";

                SqlQueryInsert ins = new SqlQueryInsert("LogAgentDashboardSession");
                ins.Value("Token", Kernel.MakeSqlSafe(model.SessionToken));
                if(model.Id != ADMIN_LOGAGENT_ID)
                    ins.Value("LogAgentId", Kernel.MakeSqlSafe(model.Id));
                ins.Value("Timestamp", Kernel.MakeSqlSafe(now));
                ins.Value("ExpirationTimestamp", Kernel.MakeSqlSafe(expire));
                SqlResult result = Kernel.Connection.ExecuteQuery(ins);

                if (result.UpdatedRows == 0)
                    return new AccountModel();
                
                SqlQueryCustom query = new SqlQueryCustom(@"select LogMetadataCourse.Name, LogMetadataCourseInstance.AcademicYear, LogMetadataCourseInstance.FromDate, LogMetadataCourseInstance.UntilDate
                    from LogMetadataAgentInCourseInstance
                    inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseInstanceId = LogMetadataAgentInCourseInstance.LogMetadataCourseInstanceId
                    inner join LogMetadataCourse on LogMetadataCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                    where LogMetadataAgentInCourseInstance.LogAgentId = "+ model.Id +@"
                    ");

                result = Kernel.Connection.ExecuteQuery(query);

                if (result.RowCount > 0)
                {
                    for(int i = 0 ; i < result.RowCount ; i++)
                    {
                        CourseInstanceModel courseInst = new CourseInstanceModel();
                        courseInst.Name = result.GetString(i, 0, "");
                        courseInst.AcademicYear = result.GetString(i, 1, "");
                        courseInst.FromDate = result.GetNullableDateTime(i, 2);
                        courseInst.UntilDate = result.GetNullableDateTime(i, 3);

                        model.CourseInstances.Add(courseInst);
                    }
                }
            }

            return model;            
        }

        [HttpPost]
        [System.Web.Http.Route("v1/Account/StudentIdForSessionToken/{session_token}")]
        public string GetStudentIdForSessionToken(string session_token)
        {
            SqlQueryCustom query = new SqlQueryCustom(@"select LogAgentId from LogAgentDashboardSession where Token = "+SqlValue.Convert(session_token));
            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            if (result.RowCount == 1)
            {
                long? logAgentId = result.GetNullableInt64(0, 0);
                if (logAgentId.HasValue)
                    return "" + logAgentId.Value;
                return "ADMIN";
            }
            return null;
        }

        // DELETE api/<controller>/5
        public void Delete(string token)
        {
            //TODO: Expire token
        }
    }
}