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
    public class VerbController : BaseApiController
    {
        // GET api/<controller>/5
        [HttpPost]
        [System.Web.Http.Route("v1/Data/Verbs")]
        public List<VerbModel> Post()
        {
            CheckAuthorisation();

            List<VerbModel> verbs = new List<VerbModel>();

            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            //int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            string partnermode = GetParameterString(parameters, "partnermode");

            string query = @"select logverbid, uri from logverb";

//            string query = @"select logverb.logverbid from logverb
//                inner join LogVerbLabel on LogVerb.LogVerbId = LogVerbLabel.LogVerbId
//                ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            List<string> skipList = new List<string>();
            if (partnermode == "uhasselt")
            {
                skipList.Add("https://w3id.org/xapi/adl/verbs/logged-in");
                //skipList.Add("https://w3id.org/xapi/adl/verbs/logged-out");
                skipList.Add("http://activitystrea.ms/schema/1.0/search");
                skipList.Add("http://id.tincanapi.com/verb/viewed");
                skipList.Add("http://www.project-vital.eu/xapi/verb/voice-recorded");
                skipList.Add("http://activitystrea.ms/schema/1.0/play");
                skipList.Add("http://id.tincanapi.com/verb/paused");
                skipList.Add("http://activitystrea.ms/schema/1.0/watch");
                skipList.Add("http://activitystrea.ms/schema/1.0/listen");
                skipList.Add("http://adlnet.gov/expapi/activities/link");
                skipList.Add("http://id.tincanapi.com/verb/previewed");
                skipList.Add("http://www.project-vital.eu/xapi/verb/printed-to-pdf");
                skipList.Add("http://id.tincanapi.com/verb/skipped");
                skipList.Add("http://adlnet.gov/expapi/verbs/interacted");
            } 
            else if (partnermode == "uva")
            {
                skipList.Add("https://w3id.org/xapi/adl/verbs/logged-in");
                skipList.Add("https://w3id.org/xapi/adl/verbs/logged-out");
                skipList.Add("http://activitystrea.ms/schema/1.0/search");
                skipList.Add("http://www.project-vital.eu/xapi/verb/voice-recorded");
                skipList.Add("http://activitystrea.ms/schema/1.0/play");
                skipList.Add("http://id.tincanapi.com/verb/paused");
                skipList.Add("http://activitystrea.ms/schema/1.0/watch");
                skipList.Add("http://activitystrea.ms/schema/1.0/listen");
                skipList.Add("http://adlnet.gov/expapi/activities/link");
                skipList.Add("http://id.tincanapi.com/verb/previewed");
                skipList.Add("http://www.project-vital.eu/xapi/verb/printed-to-pdf");
                skipList.Add("http://id.tincanapi.com/verb/skipped");
                skipList.Add("http://adlnet.gov/expapi/verbs/interacted");
                skipList.Add("https://w3id.org/xapi/adl/verbs/abandoned");
                skipList.Add("http://activitystrea.ms/schema/1.0/remove");
                skipList.Add("http://activitystrea.ms/schema/1.0/attach");
                skipList.Add("http://activitystrea.ms/schema/1.0/unlike");
                skipList.Add("http://activitystrea.ms/schema/1.0/like");
            }
            else if (partnermode == "uclan")
            {
                skipList.Add("https://brindlewaye.com/xAPITerms/verbs/loggedin/");
                skipList.Add("http://www.tincanapi.co.uk/verbs/evaluated");
            }

            verbs.Add(new VerbModel() { Id = -1, Name = "{All verbs}" });
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    string uri = result.GetString(i, 1, "");

                    if (skipList.Contains(uri))
                        continue;

                    verbs.Add(new VerbModel() { Id = result.GetInt64(i, 0, 0), Name = uri });
                }
            }

            return verbs;
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