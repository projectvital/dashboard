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
    public class ActivityDefinition : JsonModel
    {
        public Uri type { get; set; }
        public Uri moreInfo { get; set; }
        public LanguageMap name { get; set; }
        public LanguageMap description { get; set; }
        public Extensions extensions { get; set; }
        //public InteractionType interactionType { get; set; }
        //public List<String> correctResponsesPattern { get; set; }
        //public List<InteractionComponent> choices { get; set; }
        //public List<InteractionComponent> scale { get; set; }
        //public List<InteractionComponent> source { get; set; }
        //public List<InteractionComponent> target { get; set; }
        //public List<InteractionComponent> steps { get; set; }

        public ActivityDefinition() {}

        public ActivityDefinition(StringOfJSON json): this(json.toJObject()) {}

        public ActivityDefinition(JObject jobj)
        {
            if (jobj["type"] != null)
            {
                type = new Uri(jobj.Value<String>("type"));
            }
            if (jobj["moreInfo"] != null)
            {
                moreInfo = new Uri(jobj.Value<String>("moreInfo"));
            }
            if (jobj["name"] != null)
            {
                name = (LanguageMap)jobj.Value<JObject>("name");
            }
            if (jobj["description"] != null)
            {
                description = (LanguageMap)jobj.Value<JObject>("description");
            }
            if (jobj["extensions"] != null)
            {
                extensions = (Extensions)jobj.Value<JObject>("extensions");
            }
        }

        public override JObject ToJObject(TCAPIVersion version) {
            JObject result = new JObject();

            if (type != null)
            {
                result.Add("type", type.AbsoluteUri);
            }
            if (moreInfo != null)
            {
                result.Add("moreInfo", moreInfo.AbsoluteUri);
            }
            if (name != null && ! name.isEmpty())
            {
                result.Add("name", name.ToJObject(version));
            }
            if (description != null && ! description.isEmpty())
            {
                result.Add("description", description.ToJObject(version));
            }
            if (extensions != null && ! extensions.isEmpty())
            {
                result.Add("extensions", extensions.ToJObject(version));
            }

            return result;
        }

        public static explicit operator ActivityDefinition(JObject jobj)
        {
            return new ActivityDefinition(jobj);
        }
    }
}
