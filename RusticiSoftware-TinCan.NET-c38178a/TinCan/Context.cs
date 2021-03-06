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
using Newtonsoft.Json.Linq;
using TinCan.Json;

namespace TinCan
{
    public class Context : JsonModel
    {
        public Nullable<Guid> registration { get; set; }
        public Agent instructor { get; set; }
        public Agent team { get; set; }
        public ContextActivities contextActivities { get; set; }
        public String revision { get; set; }
        public String platform { get; set; }
        public String language { get; set; }
        public StatementRef statement { get; set; }
        public Extensions extensions { get; set; }

        public Context() {}

        public Context(StringOfJSON json): this(json.toJObject()) {}

        public Context(JObject jobj)
        {
            if (jobj["registration"] != null)
            {
                registration = new Guid(jobj.Value<String>("registration"));
            }
            if (jobj["instructor"] != null)
            {
                // TODO: can be Group?
                instructor = (Agent)jobj.Value<JObject>("instructor");
            }
            if (jobj["team"] != null)
            {
                // TODO: can be Group?
                team = (Agent)jobj.Value<JObject>("team");
            }
            if (jobj["contextActivities"] != null)
            {
                JToken dummy = null;
                jobj.TryGetValue("contextActivities", out dummy);
                if (dummy.HasValues)
                    contextActivities = (ContextActivities)jobj.Value<JObject>("contextActivities");
            }
            if (jobj["revision"] != null)
            {
                revision = jobj.Value<String>("revision");
            }
            if (jobj["platform"] != null)
            {
                platform = jobj.Value<String>("platform");
            }
            if (jobj["language"] != null)
            {
                language = jobj.Value<String>("language");
            }
            if (jobj["statement"] != null)
            {
                statement = (StatementRef)jobj.Value<JObject>("statement");
            }
            if (jobj["extensions"] != null)
            {
                JToken dummy = null;
                jobj.TryGetValue("extensions", out dummy);
                if(dummy.HasValues)
                    extensions = (Extensions)jobj.Value<JObject>("extensions");
            }
        }

        public override JObject ToJObject(TCAPIVersion version) {
            JObject result = new JObject();

            if (registration != null)
            {
                result.Add("registration", registration.ToString());
            }
            if (instructor != null)
            {
                result.Add("instructor", instructor.ToJObject(version));
            }
            if (team != null)
            {
                result.Add("team", team.ToJObject(version));
            }
            if (contextActivities != null)
            {
                result.Add("contextActivities", contextActivities.ToJObject(version));
            }
            if (revision != null)
            {
                result.Add("revision", revision);
            }
            if (platform != null)
            {
                result.Add("platform", platform);
            }
            if (language != null)
            {
                result.Add("language", language);
            }
            if (statement != null)
            {
                result.Add("statement", statement.ToJObject(version));
            }
            if (extensions != null)
            {
                result.Add("extensions", extensions.ToJObject(version));
            }

            return result;
        }

        public static explicit operator Context(JObject jobj)
        {
            return new Context(jobj);
        }
    }
}
