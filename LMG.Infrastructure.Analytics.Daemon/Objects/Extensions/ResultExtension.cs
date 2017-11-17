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
using System.Text;
using System.Threading.Tasks;
using TinCan;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Extensions
{
    public static class ResultExtension
    {
        #region Result
        public static Result SetScorePercentage(this Result result, double percentage)
        {
            if (result == null)
                return null;

            Score score = new Score();
            score.min = 0;
            score.max = 100;
            score.raw = percentage;

            result.score = score;
            return result;
        }
        public static Result SetSuccess(this Result result, bool success)
        {
            if (result == null)
                return null;

            result.success = success;
            return result;
        }
        public static Result SetResponse(this Result result, string response)
        {
            if (result == null)
                return null;

            result.response = response;
            return result;
        }
        public static Result SetCompletion(this Result result, bool complete)
        {
            if (result == null)
                return null;

            result.completion = complete;
            return result;
        }
        public static Result SetDuration(this Result result, TimeSpan duration)
        {
            if (result == null)
                return null;

            result.duration = duration;
            return result;
        }
        #endregion
    }
}
