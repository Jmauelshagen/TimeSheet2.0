using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Timesheet.Controllers
{
    public class TimesheetController : Controller
    {
        // GET: Timesheet
        public ActionResult Timesheet()
        {
            return View();
        }
    }
}