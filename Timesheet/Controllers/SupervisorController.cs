﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Timesheet.Controllers
{
    public class SupervisorController : Controller
    {
        // GET: Supervisor
        public ActionResult Index()
        {
            Session.Remove("AppTimeSheetData");
            Session.Remove("TimeSheetData");
            Session.Remove("Message");
            return View();
        }
    }
}