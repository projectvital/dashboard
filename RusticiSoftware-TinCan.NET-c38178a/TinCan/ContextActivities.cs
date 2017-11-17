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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TinCan.Json;

namespace TinCan
{
    public class ContextActivities : JsonModel
    {
        public List<Activity> parent { get; set; }
        public List<Activity> grouping { get; set; }
        public List<Activity> category { get; set; }
        public List<Activity> other { get; set; }

        public ContextActivities() {}

        public ContextActivities(StringOfJSON json): this(json.toJObject()) {}

        public ContextActivities(JObject jobj)
        {
            if (jobj["parent"] != null)
            {
                parent = new List<Activity>();
                foreach (JObject jactivity in jobj["parent"]) {
                    parent.Add((Activity)jactivity);
                }
            }
            if (jobj["grouping"] != null)
            {
                grouping = new List<Activity>();
                foreach (JObject jactivity in jobj["grouping"]) {
                    grouping.Add((Activity)jactivity);
                }
            }
            if (jobj["category"] != null)
            {
                category = new List<Activity>();
                foreach (JObject jactivity in jobj["category"]) {
                    category.Add((Activity)jactivity);
                }
            }
            if (jobj["other"] != null)
            {
                other = new List<Activity>();
                foreach (JObject jactivity in jobj["other"]) {
                    other.Add((Activity)jactivity);
                }
            }
        }

        public override JObject ToJObject(TCAPIVersion version) {
            JObject result = new JObject();

            if (parent != null && parent.Count > 0)
            {
                var jparent = new JArray();
                result.Add("parent", jparent);

                foreach (Activity activity in parent)
                {
                    jparent.Add(activity.ToJObject(version));
                }
            }
            if (grouping != null && grouping.Count > 0)
            {
                var jgrouping = new JArray();
                result.Add("grouping", jgrouping);

                foreach (Activity activity in grouping)
                {
                    jgrouping.Add(activity.ToJObject(version));
                }
            }
            if (category != null && category.Count > 0)
            {
                var jcategory = new JArray();
                result.Add("category", jcategory);

                foreach (Activity activity in category)
                {
                    jcategory.Add(activity.ToJObject(version));
                }
            }
            if (other != null && other.Count > 0)
            {
                var jother = new JArray();
                result.Add("other", jother);

                foreach (Activity activity in other)
                {
                    jother.Add(activity.ToJObject(version));
                }
            }

            return result;
        }

        public static explicit operator ContextActivities(JObject jobj)
        {
            return new ContextActivities(jobj);
        }
    }
}
