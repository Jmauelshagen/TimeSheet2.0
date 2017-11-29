using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;

namespace Timesheet.Controllers
{
    public class ReportsController : Controller
    {
        //Establish db connection
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();
        // GET: Reports
        //Forwards user to Supervisor screen after getting employee names and week ending dates
        //to be used in the menu/form
        public ActionResult Index()
        {
            var model = new TimeSheet();
            model.EmpNames = GetEmployeeNames();
            model.WeekEndingDates = GetWeekEndingDateList();
            return View(model);
        }

        //Obtains a list of week ending dates and adds them to a select list object
        //to be used in the UI as a menu
        private IEnumerable<SelectListItem> GetWeekEndingDateList()
        {
            TimeSheet tsheet = new TimeSheet();
            var dateList = new List<SelectListItem>();
            foreach (string date in tsheet.GetWeekEndingDateList())
            {
                dateList.Add(new SelectListItem
                {
                    Value = date,
                    Text = date
                });
            }
            return dateList;
        }

        //Obtains a list of employee names from the db and adds them to a select list
        //to be used in the UI as a menu
        private IEnumerable<SelectListItem> GetEmployeeNames()
        {
            TimeSheet timeSheet = new TimeSheet();
            var namesList = new List<SelectListItem>();
            foreach (string names in timeSheet.GetEmployeeNames())
            {
                namesList.Add(new SelectListItem
                {
                    Value = names,
                    Text = names
                });
            }
            return namesList;
        }

        //Obtains the time sheet data corresponding to the selected employee name and week ending date
        //Redirects users back to the supervisor screen after putting time sheet info into the session object
        [HttpPost]
        public ActionResult ReportData(TimeSheet model)
        {
            if(Session["Message"] != null)
            {
                Session.Remove("Message");
            }
            TimeSheet timeSheet = new TimeSheet();
            var name = model.Name.Trim();
            var wED = model.WeekEnding.Trim();
            List<TimeSheet> reportList = timeSheet.GetTimeSheetByNameAndDate(name, wED);
            Session["TimeSheetList"] = reportList;

            return RedirectToAction("Index", "Reports");
        }

        //Updates the AuthorizedBySupervisor column to True for each day of the week that is approved
        //Redirects back to the Supervisor screen with a success message
        [HttpPost]
        public ActionResult Approve()
        {
            List<TimeSheet> list = (List<TimeSheet>)Session["TimeSheetList"];
            foreach(TimeSheet sheet in list)
            {
                sheet.AuthorizedBySupervisor = "True";
                sheet.UpdateTimeSheet(sheet);
            }
            string message = "Time sheet is approved.";
            Session["Message"] = message;
            return RedirectToAction("Index", "Reports");
        }

        //Updates the Submitted column to False for each day of the week that is declined (the entire week
        //is declined all at once). Redirects back to the Supervisor screen with a denial message.
        [HttpPost]
        public ActionResult Deny()
        {
            List<TimeSheet> list = (List<TimeSheet>)Session["TimeSheetList"];
            foreach (TimeSheet sheet in list)
            {
                sheet.Submitted = "False";
                sheet.UpdateTimeSheet(sheet);
            }
            string message = "Time sheet is denied. Contact employee to have corrections made.";
            Session["Message"] = message;
            return RedirectToAction("Index", "Reports");
        }
        

    }
}