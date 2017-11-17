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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class JsonHelper
    {
        public static void GetCsvColumns(List<string> columns, JArray objects)
        {
            //List<string> columns = new List<string>();

            for (int i = 0; i < objects.Count; i++)
            {
                RecursiveParse(columns, objects[i]);
                //HandleJToken(objects[i], columns);

            }
            columns.Sort();
            //return columns;
        }

        public static void RecursiveParse(List<string> columns, JToken token)
        {
            foreach (var item in token.Children())
            {
                if (item.HasValues)
                {
                    RecursiveParse(columns, item);
                }
                else
                {
                    string path = item.Path.Substring(item.Path.IndexOf(']')+2);
                    if(!columns.Contains(path))
                        columns.Add(path);
                }
            }

        }   


        public static string GetTokenValue(JToken token)
        {
            if (token is JValue)
            {
                return ""+(token as JValue).Value;
            }
            else
                return ""+token;
        }
    }
}
