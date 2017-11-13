using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMG.Infrastructure.Analytics.Objects.Types
{
    public enum Verbs
    {
        LoggedIn         = 1  ,
        LoggedOut        = 2  ,
        Accessed         = 3  ,
        Completed        = 4  ,
        Attempted        = 5  ,
        Searched         = 6  ,
        //Viewed           = 7  ,
        Recorded         = 8  ,
        Played           = 9  ,
        Paused           = 10 ,
        //Watched          = 11 ,
        //Listened         = 12 ,
        //Linked           = 13 ,
        //Previewed        = 14 ,
        Printed          = 15 ,
        Skipped          = 16 ,
        Interacted       = 17 ,
        SessionAbandoned = 18
    }
}
