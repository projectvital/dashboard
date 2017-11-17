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
using System.Web.Mvc;
using Vital.Dashboards.Controllers.Base;

namespace Vital.Dashboards.Controllers
{
    public class DashboardsController : BaseController
    {
        
        

        //
        // GET: /Dashboards/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActivityByTimeOfDay()
        {
            SetViewBagVariables();
            return View("ActivityByTimeOfDay");
        }
        public ActionResult ActivityByDay()
        {
            SetViewBagVariables();
            return View("ActivityByDay");
        }
        public ActionResult ExerciseActivityByDay()
        {
            SetViewBagVariables();
            return View("ExerciseActivityByDay");
        }
        public ActionResult ActivityTypesByDay()
        {
            SetViewBagVariables();
            return View("ActivityTypesByDay");
        }
        public ActionResult SessionActivityByDay()
        {
            SetViewBagVariables();
            return View("SessionActivityByDay");
        }
        public ActionResult ProgressStatusVertical()
        {
            SetViewBagVariables();
            return View("ProgressStatusVertical");
        }
        public ActionResult ProgressStatusHorizontal()
        {
            SetViewBagVariables();
            return View("ProgressStatusHorizontal");
        }
        public ActionResult ActivityByChapter()
        {
            SetViewBagVariables();
            return View("ActivityByChapter");
        }
        public ActionResult TimeSpentByChapter()
        {
            SetViewBagVariables();
            return View("TimeSpentByChapter");
        }
        public ActionResult TopTimeSpentByChapter()
        {
            SetViewBagVariables();
            return View("TopTimeSpentByChapter");
        }
        public ActionResult ActivityByTypeByDay()
        {
            SetViewBagVariables();
            return View("ActivityByTypeByDay");
        }
        public ActionResult Counts()
        {
            SetViewBagVariables();
            return View("Counts");
        }
        public ActionResult Ranking()
        {
            SetViewBagVariables();
            return View("Ranking");
        }
        public ActionResult GoogleTest()
        {
            return View("GoogleTest");
        }
	}
}