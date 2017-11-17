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
using System.Collections.Generic;

namespace TinCan
{
    public class StatementsQuery
    {
        // TODO: put in common location
        private const String ISODateTimeFormat = "o";

        public Agent agent { get; set; }
        public Uri verbId { get; set; }
        public Uri activityId { get; set; }
        public Nullable<Guid> registration { get; set; }
        public Nullable<Boolean> relatedActivities { get; set; }
        public Nullable<Boolean> relatedAgents { get; set; }
        public Nullable<DateTime> since { get; set; }
        public Nullable<DateTime> until { get; set; }
        public Nullable<Int32> limit { get; set; }
        public StatementsQueryResultFormat format { get; set; }
        public Nullable<Boolean> ascending { get; set; }

        public StatementsQuery() {}

        public Dictionary<String, String> ToParameterMap (TCAPIVersion version)
        {
            var result = new Dictionary<String, String>();

            if (agent != null)
            {
                result.Add("agent", agent.ToJSON(version));
            }
            if (verbId != null)
            {
                result.Add("verb", verbId.AbsoluteUri);
            }
            if (activityId != null)
            {
                result.Add("activity", activityId.AbsoluteUri);
            }
            if (registration != null)
            {
                result.Add("registration", registration.Value.ToString());
            }
            if (relatedActivities != null)
            {
                result.Add("related_activities", relatedActivities.Value.ToString());
            }
            if (relatedAgents != null)
            {
                result.Add("related_agents", relatedAgents.Value.ToString());
            }
            if (since != null)
            {
                result.Add("since", since.Value.ToString(ISODateTimeFormat));
            }
            if (until != null)
            {
                result.Add("until", until.Value.ToString(ISODateTimeFormat));
            }
            if (limit != null)
            {
                result.Add("limit", limit.ToString());
            }
            if (format != null)
            {
                result.Add("format", format.ToString());
            }
            if (ascending != null)
            {
                result.Add("ascending", ascending.Value.ToString());
            }

            return result;
        }
    }
}
