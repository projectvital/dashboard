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