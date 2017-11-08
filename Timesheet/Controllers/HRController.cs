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
            var wED = model.WeekEnding;
            //get list of employee ids that match the week end date
            //then query the TimeSheet table for each employee's time sheet matching end date
            //calculate total hours and overtime hours for week
            //grab employee name, total hours, overtime hours, supervisor name and submittal status,
            //and load data into a new class to hold this info
            //instantiate list of this new class and put list into session to be used by the view
            return View();
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