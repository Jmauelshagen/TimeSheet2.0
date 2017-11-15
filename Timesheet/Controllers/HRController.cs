using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;

namespace Timesheet.Controllers
{
    public class HRController : Controller
    {
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();
        // GET: HR
        public ActionResult Index()
        {
            var model = new TimeSheet();
            model.WeekEndingDates = GetWeekEndingDateList();
            return View(model);
        }

        public ActionResult GetPayData(TimeSheet model)
        {
            List<PaySummary> paySumList = new List<PaySummary>();
            PaySummary paySum = new PaySummary();
            var wED = model.WeekEnding;
            List<int> empIds = paySum.GetEmpIdsByWeekEndDate(wED);
            foreach(int empId in empIds)
            {
                paySumList.Add(new PaySummary(empId, wED));
            }

            Session["PaySummaryList"] = paySumList;
            
            return RedirectToAction("Index", "HR");
        }

        public ActionResult ApprovedTimesheets()
        {
            return View();
        }

        public ActionResult TimesheetReports()
        {
            return View();
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
    }
}