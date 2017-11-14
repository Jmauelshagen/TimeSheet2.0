using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;

namespace ApprovedTimesheets.Controllers
{
    public class ApprovedTimesheetsController : Controller
    {
        // GET: ApprovedTimesheets
        public ActionResult ApprovedTimesheets()
        {
            LoginDatabaseEntities1 db = new LoginDatabaseEntities1();

            ViewBag.Employees = new SelectList(db.Employees, "EmpId", "FirstName");
            return View();
        }
    }
}