using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMG.Infrastructure.Analytics.Objects.Types
{
    public enum CustomXAPIDefinitionActivities
    {
        NavigationArrow,
        NavigationBreadcrumb
    }

    public static class CustomXAPIDefinitionActivitiesConverter
    {
        public static string Convert(CustomXAPIDefinitionActivities value)
        {
            switch (value)
            {
                case CustomXAPIDefinitionActivities.NavigationArrow: return "activity/navigation-arrow";
                case CustomXAPIDefinitionActivities.NavigationBreadcrumb: return "activity/navigation-breadcrumb";
                default: return null;
            }
        }
    }

}
