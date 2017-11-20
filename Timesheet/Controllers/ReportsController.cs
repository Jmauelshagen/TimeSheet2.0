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
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();
        // GET: Reports
        public ActionResult Index()
        {
            var model = new TimeSheet();
            model.EmpNames = GetEmployeeNames();
            model.WeekEndingDates = GetWeekEndingDateList();
            return View(model);
        }

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