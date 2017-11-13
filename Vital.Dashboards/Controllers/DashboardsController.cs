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