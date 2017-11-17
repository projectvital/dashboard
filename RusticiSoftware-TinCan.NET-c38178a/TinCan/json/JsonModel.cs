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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TinCan.Json
{
    public abstract class JsonModel : IJsonModel
    {
        // TODO: rename methods to ToJObject and ToJSON
        public abstract JObject ToJObject(TCAPIVersion version);

        public JObject ToJObject()
        {
            return ToJObject(TCAPIVersion.latest());
        }

        public String ToJSON(TCAPIVersion version, Boolean pretty = false)
        {
            Formatting formatting = Formatting.None;
            if (pretty)
            {
                formatting = Formatting.Indented;
            }

            return JsonConvert.SerializeObject(ToJObject(version), formatting);
        }

        public String ToJSON(Boolean pretty = false)
        {
            return ToJSON(TCAPIVersion.latest(), pretty);
        }
    }
}
