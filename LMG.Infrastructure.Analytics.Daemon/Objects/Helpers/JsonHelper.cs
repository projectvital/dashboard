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
