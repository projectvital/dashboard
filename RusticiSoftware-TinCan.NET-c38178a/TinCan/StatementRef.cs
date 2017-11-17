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
    public class StatementRef : JsonModel, StatementTarget
    {
        public static readonly String OBJECT_TYPE = "StatementRef";
        public String ObjectType { get { return OBJECT_TYPE; } }

        public Nullable<Guid> id { get; set; }

        public StatementRef() {}
        public StatementRef(Guid id)
        {
            this.id = id;
        }

        public StatementRef(StringOfJSON json): this(json.toJObject()) {}

        public StatementRef(JObject jobj)
        {
            if (jobj["id"] != null)
            {
                id = new Guid(jobj.Value<String>("id"));
            }
        }

        public override JObject ToJObject(TCAPIVersion version) {
            JObject result = new JObject();
            result.Add("objectType", ObjectType);

            if (id != null)
            {
                result.Add("id", id.ToString());
            }

            return result;
        }

        public static explicit operator StatementRef(JObject jobj)
        {
            return new StatementRef(jobj);
        }
    }
}
