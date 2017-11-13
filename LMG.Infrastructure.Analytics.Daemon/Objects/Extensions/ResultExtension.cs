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
