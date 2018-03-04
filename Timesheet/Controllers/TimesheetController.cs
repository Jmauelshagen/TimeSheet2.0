using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;
using System.Diagnostics;

namespace Timesheet.Controllers
{
    public class TimesheetController : Controller
    {
        // GET: Timesheet
        public ActionResult Timesheet()
        {
            return View();
        }
        public ActionResult DailyTimesheet()
        {
            return View();
        }

        public ActionResult GetDailyTimeSheet()
        {
            Debug.WriteLine("In GetDailyTimeSheet");
            //Remove the TimeSheet variable from the session if it exists
            if (Session["TimeSheetData"] != null)
            {
                Session.Remove("TimeSheetData");
            }
            //Pull the employee object from the session.
            Employee emp = (Employee)Session["Employee"];


            //Instantiate a TimeSheet object
            TimeSheet tsheet = new TimeSheet();

            //Get list of dates for the current week and add list to session
            List<string> dates = tsheet.GetDates();
            Session["Dates"] = dates;

            //Get list of TimeSheet objects based on date and employee id and add list to session
            List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.EmpId, dates);
            Session["TimeSheetData"] = tsheets;

            //Return the TimeSheet view
            return RedirectToAction("DailyTimesheet","Timesheet");
        }

        public ActionResult GetTimeSheet()
        {
            Debug.WriteLine("In GetTimeSheet");
            //Remove the TimeSheet variable from the session if it exists
            if (Session["TimeSheetData"] != null)
            {
                Session.Remove("TimeSheetData");
            }
            //Pull the employee object from the session.
            Employee emp = (Employee)Session["Employee"];


            //Instantiate a TimeSheet object
            TimeSheet tsheet = new TimeSheet();

            //Get list of dates for the current week and add list to session
            List<string> dates = tsheet.GetDates();
            Session["Dates"] = dates;

            //Get list of TimeSheet objects based on date and employee id and add list to session
            List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.EmpId, dates);
            Session["TimeSheetData"] = tsheets;

            //Return the TimeSheet view
            return RedirectToAction("Timesheet", "Timesheet");
        }

        [HttpPost]
        public ActionResult SaveTimeSheet(TimeSheet model)
        {
            try
            {
                Employee emp = (Employee)Session["Employee"];
                List<string> dates = (List<string>)Session["Dates"];
                List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];
                for(int i = 0; i < 7; i++)
                {
                    //Pull the employee object from the session.                   
                    Debug.WriteLine((string)model.TimeIn + " 1 in the weekly save result");
                    Debug.WriteLine((string)model.OutForLunch + " 2 in the weekly save result");
                    Debug.WriteLine((string)model.InFromLunch + " 3 in the weekly save result");
                    Debug.WriteLine((string)model.TimeOut + " 4 in the weekly save result");

                    string timeIn = "";
                    string outForLunch = "";
                    string inFromLunch = "";
                    string timeOut = "";

                    if (!String.IsNullOrEmpty(tsheets[i].TimeIn) && !model.TimeIn.ToString().Trim().Equals("0:00"))
                    {
                        timeIn = model.TimeIn;
                    }
                    if (!String.IsNullOrEmpty(model.OutForLunch) && !model.OutForLunch.ToString().Trim().Equals("0:00"))
                    {
                        outForLunch = model.OutForLunch;
                    }
                    if (!String.IsNullOrEmpty(model.InFromLunch) && !model.InFromLunch.ToString().Trim().Equals("0:00"))
                    {
                        inFromLunch = model.InFromLunch;
                    }
                    if (!String.IsNullOrEmpty(model.TimeOut) && !model.TimeOut.ToString().Trim().Equals("0:00"))
                    {
                        timeOut = model.TimeOut;
                    }
                    sheet.Id = model.Id;
                    sheet.WeekEnding = model.WeekEnding;
                    sheet.Date = model.Date;
                    sheet.TimeIn = timeIn;
                    sheet.OutForLunch = outForLunch;
                    sheet.InFromLunch = inFromLunch;
                    sheet.TimeOut = timeOut;
                    sheet.LeaveId = model.LeaveId;
                    sheet.LeaveHours = model.LeaveHours;
                    sheet.AdditionalHours = model.AdditionalHours;
                    sheet.TotalHoursWorked = model.TotalHoursWorked;
                    sheet.Submitted = model.Submitted;
                    sheet.AuthorizedBySupervisor = model.AuthorizedBySupervisor;
                    sheet.EmpId = model.EmpId;
                    sheet.UpdateTimeSheet(sheet);
                }
                //Instantiate TimeSheet object with data from form
                TimeSheet sheets = new TimeSheet();                

                //Get list of TimeSheet objects based on date and employee id and add list to session

                tsheets = sheets.GetTimeSheetByWeek(emp.EmpId, dates);
                Session["TimeSheetData"] = tsheets;

                //Return the TimeSheet view
                return RedirectToAction("Timesheet", "Timesheet");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return RedirectToAction("Timesheet", "Timesheet");
            }
        }

        [HttpPost]
        public ActionResult SaveDailyTimeSheet(TimeSheet model)
        {
            try
            {               
                //Pull the employee object from the session.
                Employee emp = (Employee)Session["Employee"];
                List<string> dates = (List<string>)Session["Dates"];
                Debug.WriteLine((string)model.TimeIn + " 1 in the daily save result");
                Debug.WriteLine((string)model.OutForLunch + " 2 in the daily save result");
                Debug.WriteLine((string)model.InFromLunch + " 3 in the daily save result");
                Debug.WriteLine((string)model.TimeOut + " 4 in the daily save result");

                string timeIn = "";
                string outForLunch = "";
                string inFromLunch = "";
                string timeOut = "";

                if (!String.IsNullOrEmpty(model.TimeIn) && !model.TimeIn.ToString().Trim().Equals("0:00"))
                {
                    timeIn = model.TimeIn;
                }
                if (!String.IsNullOrEmpty(model.OutForLunch) && !model.OutForLunch.ToString().Trim().Equals("0:00"))
                {
                    outForLunch = model.OutForLunch;
                }
                if (!String.IsNullOrEmpty(model.InFromLunch) && !model.InFromLunch.ToString().Trim().Equals("0:00"))
                {
                    inFromLunch = model.InFromLunch;
                }
                if (!String.IsNullOrEmpty(model.TimeOut) && !model.TimeOut.ToString().Trim().Equals("0:00"))
                {
                    timeOut = model.TimeOut;
                }
                    
                //Instantiate TimeSheet object with data from form
                TimeSheet sheet = new TimeSheet
                {
                    Id = model.Id,
                    WeekEnding = model.WeekEnding,
                    Date = model.Date,
                    TimeIn = timeIn,
                    OutForLunch = outForLunch,
                    InFromLunch = inFromLunch,
                    TimeOut = timeOut,
                    LeaveId = model.LeaveId,
                    LeaveHours = model.LeaveHours,
                    AdditionalHours = model.AdditionalHours,
                    TotalHoursWorked = model.TotalHoursWorked,
                    Submitted = model.Submitted,
                    AuthorizedBySupervisor = model.AuthorizedBySupervisor,
                    EmpId = model.EmpId
                };
                   
                Debug.WriteLine(model.Id + "      Model ID");
                Debug.WriteLine(model.EmpId + "      Emp ID");

                sheet.UpdateTimeSheet(sheet);                

                //Get list of TimeSheet objects based on date and employee id and add list to session  
                List<TimeSheet> tsheets = sheet.GetTimeSheetByWeek(emp.EmpId, dates);
                Session["TimeSheetData"] = tsheets;

                //Return the TimeSheet view
                return RedirectToAction("DailyTimesheet", "Timesheet");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return RedirectToAction("DailyTimesheet", "Timesheet");
            }
        }

        public ActionResult SubmitTimesheet(TimeSheet model)
        {
            Employee emp = (Employee)Session["Employee"];
            List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];


            foreach (TimeSheet sheet in tsheets)
            {
                sheet.Submitted = "True";
                sheet.UpdateTimeSheet(sheet);
            }
            string message = "Your time sheet was successfully submitted.";
            Session["Message"] = message;

            return RedirectToAction("Timesheet", "Timesheet");

        }
    }
}