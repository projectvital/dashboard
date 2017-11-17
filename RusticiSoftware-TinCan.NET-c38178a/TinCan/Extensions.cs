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
using Newtonsoft.Json.Linq;
using TinCan.Json;

namespace TinCan
{
    public class Extensions : JsonModel
    {
        private Dictionary<Uri, JToken> map;

        public Extensions()
        {
            map = new Dictionary<Uri, JToken>();
        }

        public Extensions(JObject jobj) : this()
        {
            foreach (var item in jobj)
            {
                map.Add(new Uri(item.Key), item.Value); 
            }
        }

        public override JObject ToJObject(TCAPIVersion version)
        {
            JObject result = new JObject();
            foreach (KeyValuePair<Uri, JToken> entry in map)
            {
                result.Add(entry.Key.AbsoluteUri, entry.Value);
            }

            return result;
        }

        public Boolean isEmpty()
        {
            return map.Count > 0 ? false : true;
        }

        public static explicit operator Extensions(JObject jobj)
        {
            return new Extensions(jobj);
        }
    }
}