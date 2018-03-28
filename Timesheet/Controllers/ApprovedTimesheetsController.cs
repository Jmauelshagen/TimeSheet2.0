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
            var sup = (Employee)Session["Supervisor"];
            ViewBag.Employees = new SelectList(db.Employees, "Banner_ID", "First_Name");
            var model = new TimeSheet
            {
                EmpNames = GetEmployeeNames(sup.Banner_ID),
                
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ApprovedData(TimeSheet model)
        {            
            int emp = Convert.ToInt32(model.Name.Trim());            
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


        //Obtains a list of employee names from the db and adds them to a select list
        //to be used in the UI as a menu
        private IEnumerable<SelectListItem> GetEmployeeNames(int sid)
        {
            TimeSheet timeSheet = new TimeSheet();
            var namesList = new List<SelectListItem>();
            foreach (string names in timeSheet.GetEmployeeNames(sid))
            {
                namesList.Add(new SelectListItem
                {
                    Value = names.Split(',')[1].Trim(),
                    Text = names
                });
            }
            return namesList;
        }
    }
}