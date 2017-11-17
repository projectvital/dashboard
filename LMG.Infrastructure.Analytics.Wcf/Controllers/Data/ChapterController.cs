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
    public class ChapterController : BaseApiController
    {
        // GET api/<controller>/5
        [HttpPost]
        [System.Web.Http.Route("v1/Data/ChaptersInOrder")]
        public List<ChapterModel> Post()
        {
            List<ChapterModel> chapters = new List<ChapterModel>();

            Newtonsoft.Json.Linq.JObject parameters = GetRequestContent_JSON();
            CheckAuthorisation(parameters);
            int? courseinstanceid = GetParameter<int>(parameters, "courseinstanceid", true);
            chapters = GetChaptersInOrder(courseinstanceid.Value);

            return chapters;
        }

        public static List<ChapterModel> GetChaptersInOrder(int courseInstanceId)
        {
            List<ChapterModel> chapters = new List<ChapterModel>();
            string query = @"select timeblock, chapter
                from LogMetadataActivityInCourse
                inner join LogMetadataCourseInstance on LogMetadataCourseInstance.LogMetadataCourseId = LogMetadataActivityInCourse.LogMetadataCourseId
                where LogMetadataCourseInstance.LogMetadataCourseInstanceId = " + courseInstanceId + @" 
                group by timeblock, chapter
                order by timeblock, chapter
                ";

            SqlResult result = Kernel.Connection.ExecuteQuery(query);

            //students.Add(new ChapterModel() { Id = -1, Name = "{All students}"});
            List<string> chapterNames = new List<string>();
            if (result.RowCount > 0)
            {
                for (int i = 0; i < result.RowCount; i++)
                {
                    string chapterName = result.GetString(i, 1, "");
                    if (!chapterNames.Contains(chapterName))
                        chapterNames.Add(chapterName);
                }
                for (int i = 0; i < chapterNames.Count; i++)
                {
                    chapters.Add(new ChapterModel() { OrderId = i + 1, Name = chapterNames[i] });
                }
            }
            
            return chapters;
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