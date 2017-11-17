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
            List<TimeSheet> reportList = new List<TimeSheet>();
            TimeSheet timeSheet = new TimeSheet();
            var names = model.EmpId;
            //List<int>empname=timeSheet.

            return RedirectToAction("Index", "Reports");
        }


    }
}