using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Timesheet.Models;

namespace Timesheet.Controllers
{
    public class HRController : Controller
    {
        //Declares/establishes the database connection
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();
        // GET: HR
        //Routes users to the HR screen after obtaining a list of weekending dates from the db
        public ActionResult Index()
        {
            var model = new TimeSheet();
            var pmod = new PaySummary();
            model.WeekEndingDates = GetWeekEndingDateList();
            return View(model);
        }

        public ActionResult Overview(TimeSheet model)
        {
            return View();
        }

        //This controller obtains a list of pay summary objects for the week selected in the UI
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
            Session["Weekend"] = wED;
            Session["PaySummaryList"] = paySumList;
            
            return RedirectToAction("Index", "HR");
        }

        [HttpPost]
        public ActionResult GetOverview(TimeSheet model)
        {
            Debug.WriteLine("In GetOverview");
            Debug.WriteLine(model.EmpId);
            //Remove the TimeSheet variable from the session if it exists
            if (Session["TimeSheetData"] != null)
            {
                Session.Remove("TimeSheetData");
            }
            //Pull the employee object from the session.
            Employee emp = new Employee();
            emp = emp.GetEmployee((int)model.EmpId);
            Session["NewEmp"] = emp;
            //Instantiate a TimeSheet object
            TimeSheet tsheet = new TimeSheet();

            //Get list of TimeSheet objects based on date and employee id and add list to session  
            string wed = (string)Session["Weekend"];
            List<TimeSheet> tsheets = model.GetTimeSheetByIdAndDate(emp.EmpId, wed);
            Session["TimeSheetData"] = tsheets;

            //Get list of dates for the selected weekend to create overview         
            List<string> dates = GetDaysInTimeSheet(emp.EmpId, wed);
            Session["Dates"] = dates;                                  
            
            //Return the TimeSheet view
            return RedirectToAction("Overview", "HR");
        }

        //Not used currently
        public ActionResult ApprovedTimesheets()
        {
            return View();
        }

        //Not used currently
        public ActionResult TimesheetReports()
        {
            return View();
        }

        private List<string> GetDaysInTimeSheet(int id, string wed)
        {
            List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];
            List<string> dates = new List<string>();
            foreach (string date in tsheets[0].GetDates(id, wed))
            {
                dates.Add(date);
            }
            return dates;
        }

        //Takes the list of week ending dates from the db and turns it into a select list
        //Used in the UI date menu
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