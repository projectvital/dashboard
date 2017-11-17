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

/*
    Copyright 2014 Rustici Software

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/
using System;
using System.Xml;
using Newtonsoft.Json.Linq;
using TinCan.Json;

namespace TinCan
{
    public class Result : JsonModel
    {
        public Nullable<Boolean> completion { get; set; }
        public Nullable<Boolean> success { get; set; }
        public String response { get; set; }
        public Nullable<TimeSpan> duration { get; set; }
        public Score score { get; set; }
        public Extensions extensions { get; set; }

        public Result() {}

        public Result(StringOfJSON json): this(json.toJObject()) {}

        public Result(JObject jobj)
        {
            if (jobj["completion"] != null)
            {
                completion = jobj.Value<Boolean>("completion");
            }
            if (jobj["success"] != null)
            {
                success = jobj.Value<Boolean>("success");
            }
            if (jobj["response"] != null)
            {
                response = jobj.Value<String>("response");
            }
            if (jobj["duration"] != null)
            {
                duration = XmlConvert.ToTimeSpan(jobj.Value<String>("duration"));
            }
            if (jobj["score"] != null)
            {
                JToken dummy = null;
                jobj.TryGetValue("score", out dummy);
                if (dummy.HasValues)
                    score = (Score)jobj.Value<JObject>("score");
            }
            if (jobj["extensions"] != null)
            {
                JToken dummy = null;
                jobj.TryGetValue("extensions", out dummy);
                if (dummy.HasValues)
                    extensions = (Extensions)jobj.Value<JObject>("extensions");
            }
        }

        public override JObject ToJObject(TCAPIVersion version) {
            JObject result = new JObject();

            if (completion != null)
            {
                result.Add("completion", completion);
            }
            if (success != null)
            {
                result.Add("success", success);
            }
            if (response != null)
            {
                result.Add("response", response);
            }
            if (duration != null)
            {
                result.Add("duration", XmlConvert.ToString((TimeSpan)duration));
            }
            if (score != null)
            {
                result.Add("score", score.ToJObject(version));
            }
            if (extensions != null)
            {
                result.Add("extensions", extensions.ToJObject(version));
            }

            return result;
        }

        public static explicit operator Result(JObject jobj)
        {
            return new Result(jobj);
        }
    }
}
