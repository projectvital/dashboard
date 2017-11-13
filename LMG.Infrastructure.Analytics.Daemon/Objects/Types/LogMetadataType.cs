using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Daemon.Objects.Types
{
    public enum LogMetadataLoadType
    {
        Agent,
        Extension,
        CourseInstance,
        CourseInstanceTimeBlock,
        ActivityInCourse,
        CourseInstanceClass,
        AgentInCourseInstance,
        Unknown
    }
}
