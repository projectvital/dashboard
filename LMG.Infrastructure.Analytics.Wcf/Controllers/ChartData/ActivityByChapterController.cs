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

using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Wcf.Objects.Exceptions;
using OriginDatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LMG.Infrastructure.Analytics.Wcf.Controllers
{
    public class ActivityByChapterController : BaseApiController
    {
        // GET api/<controller>
        //[HttpGet]
        [HttpPost]
        [System.Web.Http.Route("v1/ChartData/ActivityByChapter")]
        public HttpResponseMessage Post()
        {
            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            string query = GetQuery(parameters);
            //bool filterOnLogAgentId = IsParameterPresent("logagentid", parameters);

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            Dictionary<DateTime, Dictionary<string, int>> dictionary = new Dictionary<DateTime, Dictionary<string, int>>();

            string data = "date,chapter,count";
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    DateTime date = result.GetDate(i, 0, DateTime.MinValue);
                    if (!dictionary.ContainsKey(date))
                        dictionary.Add(date, new Dictionary<string, int>());

                    string chapter = result.GetString(i, 1, "<unknown chapter>");
                    if (!dictionary[date].ContainsKey(chapter))
                    {
                        dictionary[date].Add(chapter, result.GetInt32(i, 2, 0));
                    }
                }
            }

            DateTime min, max;
            DateTime? from = null, until = null;
            if (GetCourseInstanceDates(parameters, ref from, ref until))
            {
                min = from.Value;
                max = until.Value;
            }
            else
            {
                min = dictionary.Keys.Min();
                max = dictionary.Keys.Max();
            }

            for (DateTime d = min; d <= max; d = d.AddDays(1))
            {
                if (dictionary.ContainsKey(d))
                    continue;
                else
                    dictionary.Add(d, new Dictionary<string, int>());
            }

            List<DateTime> keys = dictionary.Keys.ToList();
            keys.Sort();

            foreach (DateTime date in keys)
            {
                //data += System.Environment.NewLine + date.ToString("dd/MM/yyyy");
                //if(dictionary[date]

                if (dictionary[date].Count == 0)
                {
                    data += System.Environment.NewLine + date.ToString("dd/MM/yyyy") + ",\"" + "" + "\"," + 0;
                }
                else
                {
                    foreach (string chapter in dictionary[date].Keys)
                    {
                        data += System.Environment.NewLine + date.ToString("dd/MM/yyyy") + ",\"" + chapter + "\"," + dictionary[date][chapter];
                    }
                }
            }

            

            return ReturnData(data);
        }

        private string GetQuery(Newtonsoft.Json.Linq.JObject parameters, bool ignoreLogAgentId = false)
        {
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            //long? timeblock = GetParameter<long>(parameters, "timeblock");
            long? logAgentId = GetParameter<long>(parameters, "logagentid");
            long? logVerbId = GetParameter<long>(parameters, "logverbid");

            string query = @"select CAST(Timestamp as DATE) as 'dt',LogMetadataActivityInCourse.Chapter as 'chapter',count(*) as 'count'
                from logstatement
                inner join LogActivity on LogStatement.TargetLogActivityId = LogActivity.LogActivityId
                inner join LogMetadataActivityInCourse on LogMetadataActivityInCourse.LogActivityUrl = LogActivity.Id
                inner join LogMetadataCourseInstance on LogMetadataActivityInCourse.LogMetadataCourseId = LogMetadataCourseInstance.LogMetadataCourseId
                where logstatement.timestamp > LogMetadataCourseInstance.FromDate and logstatement.timestamp < LogMetadataCourseInstance.UntilDate
                and LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseinstanceid + " " +
                ((logAgentId.HasValue && !ignoreLogAgentId) ? " and logstatement.LogAgentId = " + logAgentId.Value + System.Environment.NewLine : "") +
                ((logVerbId.HasValue) ? " and logstatement.LogVerbid = " + logVerbId.Value + System.Environment.NewLine : "") +
                @" group by CAST(Timestamp as DATE), LogMetadataActivityInCourse.Chapter
                order by LogMetadataActivityInCourse.Chapter";

            return query;
        }


        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}