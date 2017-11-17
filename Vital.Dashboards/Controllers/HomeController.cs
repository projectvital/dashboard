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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Vital.Dashboards.Controllers.Base;

namespace Vital.Dashboards.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return RedirectToAction("Student");// View();
        }

        public ActionResult Dashboard()
        {
            SetViewBagVariables();
            return View("Dashboard");
        }

        public ActionResult Login()
        {
            SetViewBagVariables();
            return View("Login");
        }

        public ActionResult Student(string id)
        {
            if (id == null)
            {
                if (Request.Url.Host.ToLower().Contains(".uva.") || (GlobalActionFilter.ApiUrl.Contains(".uva.")))
                    id = "UvA";
                else if (Request.Url.Host.ToLower().Contains(".uclan.") || (GlobalActionFilter.ApiUrl.Contains(".uclan.")))
                    id = "UCLan";
                //else if (Request.Url.Host.ToLower().Contains(".uhasselt.") || (System.Web.Configuration.WebConfigurationManager.AppSettings["ApiUrl"].Contains(".uhasselt.")))
                //    id = "";
            }

            SetViewBagVariables();
            if(string.IsNullOrWhiteSpace(id) || id.ToLower() == "uva" || id.ToLower() == "uclan")
                return View("Student" + id);
            else
                return View("Student");
        }
        public ActionResult Instructor(string id)
        {
            string session_token = Request.Cookies["session-token"].Value;

            string api_url = GlobalActionFilter.ApiUrlServerSide + "/v1/Account/StudentIdForSessionToken/" + session_token;

            string value = "";
            try
            {
                HttpClient client = new HttpClient();
                //var content = new FormUrlEncodedContent();
                var response = client.PostAsync(api_url, null).Result;
                value = response.Content.ReadAsStringAsync().Result;
                //var responseString = await response.Content.ReadAsStringAsync();
            }
            catch
            {

            }

            if (value == "\"ADMIN\"")
            {
                if (id == null)
                {
                    if (Request.Url.Host.ToLower().Contains(".uva.") || (GlobalActionFilter.ApiUrl.Contains(".uva.")) || (System.Diagnostics.Debugger.IsAttached && Request.Cookies["partner-mode"]!=null && Request.Cookies["partner-mode"].Value == "uva"))
                        id = "UvA";
                    else if (Request.Url.Host.ToLower().Contains(".uclan.") || (GlobalActionFilter.ApiUrl.Contains(".uclan.")) || (System.Diagnostics.Debugger.IsAttached && Request.Cookies["partner-mode"]!=null && Request.Cookies["partner-mode"].Value == "uclan"))
                        id = "UCLan";
                    //else if (Request.Url.Host.ToLower().Contains(".uhasselt.") || (System.Web.Configuration.WebConfigurationManager.AppSettings["ApiUrl"].Contains(".uhasselt.")))
                    //    id = "";
                }

                if (string.IsNullOrWhiteSpace(id) || id.ToLower() == "uva" || id.ToLower() == "uclan")
                    return View("Instructor" + id);
                else
                    return View("Instructor");
            }
            else
                return RedirectToAction("Student");
        }

        public ActionResult More()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            try
            {
                string user = Request.Params["x_username"];
                string pass = Request.Params["x_password"];

                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("username", user);
                pairs.Add("password", pass);

                var httpContent = new StringContent(JsonConvert.SerializeObject(pairs), Encoding.UTF8, "application/json");

                var client = new HttpClient { BaseAddress = new Uri(GlobalActionFilter.ApiUrl) };

                // call sync
                var response = client.PostAsync("/v1/Account/Login", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                }

                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.LoginError = "ERROR: " + ex.Message;
                return View("Login");
            }
        }
	}
}