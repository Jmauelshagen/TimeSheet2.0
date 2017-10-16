using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Timesheet.Controllers
{
    public class ApprovedTimesheetsController : Controller
    {
        // GET: ApprovedTimesheets
        public ActionResult Index()
        {
            return View();
        }
    }
}