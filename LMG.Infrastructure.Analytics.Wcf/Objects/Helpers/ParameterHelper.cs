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