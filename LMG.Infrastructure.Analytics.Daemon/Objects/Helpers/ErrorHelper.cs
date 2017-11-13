using LMG.Infrastructure.Analytics.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class ErrorHelper
    {
        public static void ThrowErrorIfNotNull(ErrorLog error)
        {
            if (error != null)
                throw error;
        }
        public static void ThrowError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
                throw new ErrorLog(error);
        }
    }
}
