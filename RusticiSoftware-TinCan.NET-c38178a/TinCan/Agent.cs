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
    public class Agent : JsonModel, StatementTarget
    {
        public static readonly String OBJECT_TYPE = "Agent";
        public virtual String ObjectType { get { return OBJECT_TYPE; } }

        public String name { get; set; }
        public String mbox { get; set; }
        public String mbox_sha1sum { get; set; }
        public String openid { get; set; }
        public AgentAccount account { get; set; }

        public Agent() { }

        public Agent(StringOfJSON json) : this(json.toJObject()) { }

        public Agent(JObject jobj)
        {
            if (jobj["name"] != null)
            {
                name = jobj.Value<String>("name");
            }

            if (jobj["mbox"] != null)
            {
                mbox = jobj.Value<String>("mbox");
            }
            if (jobj["mbox_sha1sum"] != null)
            {
                mbox_sha1sum = jobj.Value<String>("mbox_sha1sum");
            }
            if (jobj["openid"] != null)
            {
                openid = jobj.Value<String>("openid");
            }
            if (jobj["account"] != null)
            {
                account = (AgentAccount)jobj.Value<JObject>("account");
            }
        }

        public override JObject ToJObject(TCAPIVersion version)
        {
            JObject result = new JObject();
            result.Add("objectType", ObjectType);

            if (name != null)
            {
                result.Add("name", name);
            }

            if (account != null)
            {
                result.Add("account", account.ToJObject(version));
            }
            else if (mbox != null)
            {
                result.Add("mbox", mbox);
            }
            else if (mbox_sha1sum != null)
            {
                result.Add("mbox_sha1sum", mbox_sha1sum);
            }
            else if (openid != null)
            {
                result.Add("openid", openid);
            }

            return result;
        }

        public static explicit operator Agent(JObject jobj)
        {
            return new Agent(jobj);
        }
    }
}
