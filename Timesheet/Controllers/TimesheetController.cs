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
            return RedirectToAction("DailyTimesheet","Timesheet", tsheets);
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
                //Pull the employee object from the session.
                Employee emp = (Employee)Session["Employee"];
                List<string> dates = (List<string>)Session["Dates"];
                Debug.WriteLine((string)model.TimeIn + " 1 in the weekly save result");
                Debug.WriteLine((string)model.OutForLunch + " 2 in the weekly save result");
                Debug.WriteLine((string)model.InFromLunch + " 3 in the weekly save result");
                Debug.WriteLine((string)model.TimeOut + " 4 in the weekly save result");

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
                sheet.UpdateTimeSheet(sheet);

                //Get list of TimeSheet objects based on date and employee id and add list to session
                List<TimeSheet> tsheets = sheet.GetTimeSheetByWeek(emp.EmpId, dates);
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
        public ActionResult QuickTimeStamp()//Makes a timestamp for the current date up to 4 stamps.
        {
            Debug.WriteLine("In QuickTimeStamp");
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

            string CurrentDate = DateTime.Now.ToShortDateString();
            string message;
            for (int i = 0; i < 7; i++)
            {
                if (tsheets[i].Date.ToString().Trim().Equals(CurrentDate))
                {
                    string today = DateTime.Now.ToString("HH:mm");

                    if (tsheets[i].TimeIn.ToString().Trim().Equals("0:00"))
                    {
                        tsheets[i].TimeIn = today;
                        TimeSheet sheet = new TimeSheet
                        {
                            Id = tsheets[i].Id,
                            WeekEnding = tsheets[i].WeekEnding,
                            Date = tsheets[i].Date,
                            TimeIn = tsheets[i].TimeIn,
                            OutForLunch = tsheets[i].OutForLunch,
                            InFromLunch = tsheets[i].InFromLunch,
                            TimeOut = tsheets[i].TimeOut,
                            LeaveId = tsheets[i].LeaveId,
                            LeaveHours = tsheets[i].LeaveHours,
                            AdditionalHours = tsheets[i].AdditionalHours,
                            TotalHoursWorked = tsheets[i].TotalHoursWorked,
                            Submitted = tsheets[i].Submitted,
                            AuthorizedBySupervisor = tsheets[i].AuthorizedBySupervisor,
                            EmpId = tsheets[i].EmpId
                        };
                        sheet.UpdateTimeSheet(sheet);
                        message = "1st punch has been added at: "+ DateTime.Now.ToString("h:mm:ss tt");
                        Session["QuickTimeStamp"] = message;
                    }
                    else
                    {
                        if (tsheets[i].OutForLunch.ToString().Trim().Equals("0:00"))
                        {
                            tsheets[i].OutForLunch = today;
                            TimeSheet sheet = new TimeSheet
                            {
                                Id = tsheets[i].Id,
                                WeekEnding = tsheets[i].WeekEnding,
                                Date = tsheets[i].Date,
                                TimeIn = tsheets[i].TimeIn,
                                OutForLunch = tsheets[i].OutForLunch,
                                InFromLunch = tsheets[i].InFromLunch,
                                TimeOut = tsheets[i].TimeOut,
                                LeaveId = tsheets[i].LeaveId,
                                LeaveHours = tsheets[i].LeaveHours,
                                AdditionalHours = tsheets[i].AdditionalHours,
                                TotalHoursWorked = tsheets[i].TotalHoursWorked,
                                Submitted = tsheets[i].Submitted,
                                AuthorizedBySupervisor = tsheets[i].AuthorizedBySupervisor,
                                EmpId = tsheets[i].EmpId
                            };
                            sheet.UpdateTimeSheet(sheet);
                            message = "2nd punch has been added at: " + DateTime.Now.ToString("h:mm:ss tt");
                            Session["QuickTimeStamp"] = message;
                        }
                        else
                        {
                            if (tsheets[i].InFromLunch.ToString().Trim().Equals("0:00"))
                            {
                                tsheets[i].InFromLunch = today;
                                TimeSheet sheet = new TimeSheet
                                {
                                    Id = tsheets[i].Id,
                                    WeekEnding = tsheets[i].WeekEnding,
                                    Date = tsheets[i].Date,
                                    TimeIn = tsheets[i].TimeIn,
                                    OutForLunch = tsheets[i].OutForLunch,
                                    InFromLunch = tsheets[i].InFromLunch,
                                    TimeOut = tsheets[i].TimeOut,
                                    LeaveId = tsheets[i].LeaveId,
                                    LeaveHours = tsheets[i].LeaveHours,
                                    AdditionalHours = tsheets[i].AdditionalHours,
                                    TotalHoursWorked = tsheets[i].TotalHoursWorked,
                                    Submitted = tsheets[i].Submitted,
                                    AuthorizedBySupervisor = tsheets[i].AuthorizedBySupervisor,
                                    EmpId = tsheets[i].EmpId
                                };
                                sheet.UpdateTimeSheet(sheet);
                                message = "3rd punch has been added at: " + DateTime.Now.ToString("h:mm:ss tt");
                                Session["QuickTimeStamp"] = message;
                            }
                            else
                            {
                                if (tsheets[i].TimeOut.ToString().Trim().Equals("0:00"))
                                {
                                    tsheets[i].TimeOut = today;
                                    TimeSheet sheet = new TimeSheet
                                    {
                                        Id = tsheets[i].Id,
                                        WeekEnding = tsheets[i].WeekEnding,
                                        Date = tsheets[i].Date,
                                        TimeIn = tsheets[i].TimeIn,
                                        OutForLunch = tsheets[i].OutForLunch,
                                        InFromLunch = tsheets[i].InFromLunch,
                                        TimeOut = tsheets[i].TimeOut,
                                        LeaveId = tsheets[i].LeaveId,
                                        LeaveHours = tsheets[i].LeaveHours,
                                        AdditionalHours = tsheets[i].AdditionalHours,
                                        TotalHoursWorked = tsheets[i].TotalHoursWorked,
                                        Submitted = tsheets[i].Submitted,
                                        AuthorizedBySupervisor = tsheets[i].AuthorizedBySupervisor,
                                        EmpId = tsheets[i].EmpId
                                    };
                                    sheet.UpdateTimeSheet(sheet);
                                    message = "4th punch has been added at: " + DateTime.Now.ToString("h:mm:ss tt");
                                    Session["QuickTimeStamp"] = message;
                                }
                                else
                                {
                                    message = "All 4 punches have been used, please go to daily timesheet to enter in additional hours.";
                                    Session["QuickTimeStamp"] = message;
                                }
                            }
                        }
                    }

                }

            }

            return RedirectToAction("Index", "Employees");
        }
        

    }
}