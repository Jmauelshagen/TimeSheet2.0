using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;

namespace Timesheet.Controllers
{
    public class TimesheetController : Controller
    {
        // GET: Timesheet
        public ActionResult Timesheet()
        {
            return View();
        }

        public ActionResult GetTimeSheet()
        {
            //Pull the employee object from the session.
            Employee emp = (Employee)Session["Employee"];

            //Instantiate a TimeSheet object
            TimeSheet tsheet = new TimeSheet();

            //Get list of dates for the current week and add list to session
            List<string> dates = tsheet.GetDates();
            Session["Dates"] = dates;

            //Get list of TimeSheet objects based on date and employee id and add list to session
            List <TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.EmpId, dates);
            Session["TimeSheetData"] = tsheets;

            //Return the TimeSheet view
            return View();
        }

        
    }
}