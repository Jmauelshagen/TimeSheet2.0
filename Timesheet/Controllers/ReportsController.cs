using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;

namespace Reports.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports,
        public ActionResult Reports()
        {
            LoginDatabaseEntities1 db = new LoginDatabaseEntities1();

            ViewBag.Employees = new SelectList(db.Employees, "EmpId", "FirstName");
            return View();
        }
    }
}