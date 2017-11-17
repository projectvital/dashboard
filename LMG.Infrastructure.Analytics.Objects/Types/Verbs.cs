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
