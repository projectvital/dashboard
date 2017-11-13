using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Objects.Helpers
{
    public static class ParameterHelper
    {
        public static long? ParseProgramme(string programmeKey, ref string group)
        {
            if(string.IsNullOrWhiteSpace(programmeKey))
                return null;

            string part_id = programmeKey;
            if (programmeKey.Contains('|'))
            {//Look for group
                string[] parts = programmeKey.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    part_id = parts[0];
                    group = parts[1];
                }
            }

            long programmeId;
            if (long.TryParse(part_id, out programmeId))
                return programmeId;
            else
                return null;
        }
    }
}