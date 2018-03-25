using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
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

        [HttpPost]
        public ActionResult ApprovedData(TimeSheet model)
        {
            string emp = Request.Form["Employee"].ToString();
            Debug.WriteLine("Emp value is : " + emp + " ]");
            TimeSheet timesheet = new TimeSheet();           
            List<TimeSheet> approveList = timesheet.GetApprovedTimesheets(emp);
            Session["TimeSheetData"] = approveList;
            return RedirectToAction("ApprovedTimesheets", "ApprovedTimesheets");
        }
    }
}