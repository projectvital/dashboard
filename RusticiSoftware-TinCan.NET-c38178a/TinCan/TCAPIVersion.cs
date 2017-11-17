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
    public sealed class TCAPIVersion
    {
        public static readonly TCAPIVersion V102 = new TCAPIVersion("1.0.2");
        public static readonly TCAPIVersion V101 = new TCAPIVersion("1.0.1");
        public static readonly TCAPIVersion V100 = new TCAPIVersion("1.0.0");
        public static readonly TCAPIVersion V095 = new TCAPIVersion("0.95");
        public static readonly TCAPIVersion V090 = new TCAPIVersion("0.9");

        public static TCAPIVersion latest()
        {
            return V101;
        }

        private static Dictionary<String, TCAPIVersion> known;
        private static Dictionary<String, TCAPIVersion> supported;

        public static Dictionary<String, TCAPIVersion> GetKnown()
        {
            if (known != null) {
                return known;
            }

            known = new Dictionary<String, TCAPIVersion>();
            known.Add("1.0.2", V102);
            known.Add("1.0.1", V101);
            known.Add("1.0.0", V100);
            known.Add("0.95", V095);
            known.Add("0.9", V090);

            return known;
        }

        public static Dictionary<String, TCAPIVersion> GetSupported()
        {
            if (supported != null) {
                return supported;
            }

            supported = new Dictionary<String, TCAPIVersion>();
            supported.Add("1.0.2", V102);
            supported.Add("1.0.1", V101);
            supported.Add("1.0.0", V100);

            return supported;
        }

        public static explicit operator TCAPIVersion(String vStr)
        {
            var s = GetKnown();
            if (!s.ContainsKey(vStr))
            {
                throw new ArgumentException("Unrecognized version: " + vStr);
            }

            return s[vStr];
        }

        private String text;

        private TCAPIVersion(String value)
        {
            text = value;
        }

        public override String ToString()
        {
            return text;
        }
    }
}
