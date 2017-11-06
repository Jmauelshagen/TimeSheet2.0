using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Timesheet.Controllers
{
    public class HRController : Controller
    {
        // GET: HR
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApprovedTimesheets()
        {
            return View();
        }

        public ActionResult TimesheetReports()
        {
            return View();
        }
    }
}