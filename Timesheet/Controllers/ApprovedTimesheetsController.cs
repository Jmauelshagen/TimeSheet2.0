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

            ViewBag.Employees = new SelectList(db.Employees, "Banner_ID", "First_Name");            
            return View();
        }

        [HttpPost]
        public ActionResult ApprovedData(TimeSheet model)
        {
            int emp = Int32.Parse(Request.Form["Employee"].ToString());
            Debug.WriteLine("Emp value is : " + emp + " ]");
            Employee emp1 = new Employee();
            Session["Employee"] = emp1.GetEmployee(emp);
            TimeSheet timesheet = new TimeSheet();           
            List<List<TimeSheet>> approveList = new List<List<TimeSheet>>();           
            foreach (string date in timesheet.GetApprovedWeekendsList(emp))
            {
                approveList.Add(timesheet.GetTimeSheetByIdAndDate(emp, date));
                Debug.WriteLine("The size of approveList is: " + approveList.Count);
            }
            Debug.WriteLine("The size of approveList is: " + approveList.Count);
            Session["AppTimeSheetData"] = approveList;
            return RedirectToAction("ApprovedTimesheets", "ApprovedTimesheets");
        }
    }
}