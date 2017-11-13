using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Helpers
{
    public static class DataPreparationHelper
    {
        public static void CalculateStudentPerformanceGroup()
        {
            AnalyticsWorker worker = new AnalyticsWorker();
            worker.CalculateStudentPerformanceGroup();
        }


    }
}
