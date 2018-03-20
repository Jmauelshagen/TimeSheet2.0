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
            Session["datelist"] = GetListOfDays();
            return View();
        }
        public ActionResult DailyTimesheet()
        {
            return View();
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

            //resets the QuickTimeStamp and DailyMessage to blank when you go to another page
            string message = "";
            Session["QuickTimeStamp"] = message;
            Session["DailyMessage"] = message;

            //Note Error checking
            Session["Message"] = message;

            //Return the TimeSheet view
            return RedirectToAction("DailyTimesheet", "Timesheet");
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

            //resets the QuickTimeStamp and DailyMessage to blank when you go to another page
            string message = "";
            Session["QuickTimeStamp"] = message;
            Session["DailyMessage"] = message;

            //Note Error Checking
            Session["Message"] = message;
            //Return the TimeSheet view
            return RedirectToAction("Timesheet", "Timesheet");
        }
        private IEnumerable<SelectListItem> GetListOfDays()
        {
            TimeSheet tsheet = new TimeSheet();
            var dateList = new List<SelectListItem>();
            foreach (string date in tsheet.GetDates().Skip(1))
            {
                dateList.Add(new SelectListItem
                {
                    Value = date,
                    Text = date
                });
            }
            return dateList;
        }

        [HttpPost]
        public ActionResult SaveTimeNote(TimeSheet model)
        {
            try
            {
                /**This next seciton returns the stored date selected from the drop down. It then calls a method
                * to retrieves a specific timesheet based on the day and user. Then sets the current model
                * to the timesheet to ensure integrity and allow the note to be updated below**/

                Debug.WriteLine("In SaveTimeNote");
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

                //string CurrentDate = Request.Form["Date"].ToString();
                string CurrentDate = model.Date;
                string message;

                for (int i = 0; i < 7; i++)
                {
                    if (tsheets[i].Date.ToString().Trim().Equals(CurrentDate))
                    {
                        if (tsheets[i].AuthorizedBySupervisor.ToString().Trim().Equals("False"))
                        {
                            if (!String.IsNullOrEmpty(model.Note))
                            {
                                if (model.Note.ToString().Equals("None") || model.Note.ToString().Equals("none")) { tsheets[i].Note = ""; }
                                else { tsheets[i].Note = model.Note; }
                            }
                           /* if (model.AdditionalHours.ToString().Trim().Equals("0:00") && !String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 1");
                                string mess = "You created a note but have no additional hours. This may be a mistake.";
                                Session["Message"] = mess;
                            }
                            if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 2");
                                string mess = "You have addtional hours. You might want to make a note.";
                                Session["Message"] = mess;
                            }*/
                            message = "Timesheet Saved Succesfully";
                            Session["WeeklyMessage"] = message;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                        }
                        else
                        {
                            message = "Timesheet has already been approved. no changes can be made";
                            Session["WeeklyMessage"] = message;
                        }

                    }
                }

                return RedirectToAction("Timesheet", "Timesheet");
                /*
                //Pull the employee object from the session.
                Employee emp = (Employee)Session["Employee"];
                List<string> dates = (List<string>)Session["Dates"];
                Session["Message"] = "";

                /**This next seciton returns the stored date selected from the drop down. It then calls a method
                 * to retrieves a specific timesheet based on the day and user. Then sets the current model
                 * to the timesheet to ensure integrity and allow the note to be updated below
                string date = Request.Form["Date"].ToString();
                Debug.WriteLine("The Date String is:" + date + "]");
                if (!String.IsNullOrEmpty(date))
                {
                    int empId = emp.EmpId;
                    TimeSheet ts = new TimeSheet();
                    ts = ts.GetDates(empId, date);
                    Debug.WriteLine("The new id should be: " + ts);
                    model.Id = ts.Id;
                    model.WeekEnding = ts.WeekEnding;
                    model.Date = ts.Date;
                    model.TimeIn = ts.TimeIn;
                    model.OutForLunch = ts.OutForLunch;
                    model.InFromLunch = ts.InFromLunch;
                    model.TimeOut = ts.TimeOut;
                    model.LeaveId = ts.LeaveId;
                    model.LeaveHours = ts.LeaveHours;
                    model.AdditionalHours = ts.AdditionalHours;
                    model.TotalHoursWorked = ts.TotalHoursWorked;
                    model.Submitted = ts.Submitted;
                    model.AuthorizedBySupervisor = ts.AuthorizedBySupervisor;
                    model.EmpId = ts.EmpId;
                }                
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

                //Slight bit of feedback till error checking can be implimented. If additional hours exist or doesnt with a note or not
                if (model.AdditionalHours.ToString().Trim().Equals("0:00") && !String.IsNullOrEmpty(model.Note))
                {
                    Debug.WriteLine("In Erro 1");
                    string mess = "You created a note but have no additional hours. This may be a mistake.";
                    Session["Message"] = mess;
                }
                if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                {
                    Debug.WriteLine("In Erro 2");
                    string mess = "You have addtional hours. You might want to make a note.";
                    Session["Message"] = mess;
                }
                Debug.WriteLine("AddH : " + model.AdditionalHours + "/ Note: " + model.Note);
                Debug.WriteLine("The message says :" + Session["Message"] + "\\");
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
                    EmpId = model.EmpId,
                    Note = model.Note
                };

                sheet.UpdateTimeSheet(sheet);

                //Get list of TimeSheet objects based on date and employee id and add list to session
                List<TimeSheet> tsheets = sheet.GetTimeSheetByWeek(emp.EmpId, dates);
                Session["TimeSheetData"] = tsheets;
                Request.Form["Date"] = "";
                //Return the TimeSheet view
                Request.Params.Clear();
                return RedirectToAction("Timesheet", "Timesheet");*/

            }
            catch (Exception ex)
            {
                
                Debug.WriteLine(ex);                
                return RedirectToAction("Timesheet", "Timesheet");
            }
        }

        [HttpPost]
        public ActionResult SaveTimeSheet(TimeSheet model)
        {
            try
            {
                Debug.WriteLine("In SaveTimeSheet");
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

                string CurrentDate = model.Date;
                string message;
                for (int i = 0; i < 7; i++)
                {
                    if (tsheets[i].Date.ToString().Trim().Equals(CurrentDate))
                    {
                        if (tsheets[i].AuthorizedBySupervisor.ToString().Trim().Equals("False"))
                        {
                            if (!String.IsNullOrEmpty(model.TimeIn)) { tsheets[i].TimeIn = model.TimeIn; }
                            if (!String.IsNullOrEmpty(model.OutForLunch)) { tsheets[i].OutForLunch = model.OutForLunch; }
                            if (!String.IsNullOrEmpty(model.InFromLunch)) { tsheets[i].InFromLunch = model.InFromLunch; }
                            if (!String.IsNullOrEmpty(model.TimeOut)) { tsheets[i].TimeOut = model.TimeOut; }
                            if (!String.IsNullOrEmpty(model.LeaveId.ToString())) { tsheets[i].LeaveId = model.LeaveId; }
                            if (!String.IsNullOrEmpty(model.LeaveHours)) { tsheets[i].LeaveHours = model.LeaveHours; }
                            if (!String.IsNullOrEmpty(model.AdditionalHours)) { tsheets[i].AdditionalHours = model.AdditionalHours; }

                            if (model.AdditionalHours.ToString().Trim().Equals("0:00") && !String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 1");
                                string mess = "You created a note but have no additional hours. This may be a mistake.";
                                Session["Message"] = mess;
                            }
                            if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 2");
                                string mess = "You have addtional hours. You might want to make a note.";
                                Session["Message"] = mess;
                            }
                            message = "Timesheet Saved Succesfully";
                            Session["WeeklyMessage"] = message;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                        }
                        else
                        {
                            message = "Timesheet has already been approved. no changes can be made";
                            Session["WeeklyMessage"] = message;
                        }
                    }
                }

                return RedirectToAction("Timesheet", "Timesheet");
                /*
                //Pull the employee object from the session.
                Employee emp = (Employee)Session["Employee"];
                List<string> dates = (List<string>)Session["Dates"];
                Session["Message"] = "";
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
                Debug.WriteLine("The old note is: " + model.Note);
                Debug.WriteLine("The ID is : " + model.Id);                
                //Slight bit of feedback till error checking can be implimented. If additional hours exist or doesnt with a note or not
                if (model.AdditionalHours.ToString().Trim().Equals("0:00") && !String.IsNullOrEmpty(model.Note))
                {
                    Debug.WriteLine("In Erro 1");
                    string mess = "You created a note but have no additional hours. This may be a mistake.";
                    Session["Message"] = mess;
                }
                if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                {
                    Debug.WriteLine("In Erro 2");
                    string mess = "You have addtional hours. You might want to make a note.";
                    Session["Message"] = mess;
                }
                Debug.WriteLine("AddH : " + model.AdditionalHours + "/ Note: " + model.Note);
                Debug.WriteLine("The message says :" + Session["Message"] + "\\");
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
                    EmpId = model.EmpId,
                    Note = model.Note
                };
                
                sheet.UpdateTimeSheet(sheet);

                //Get list of TimeSheet objects based on date and employee id and add list to session
                List<TimeSheet> tsheets = sheet.GetTimeSheetByWeek(emp.EmpId, dates);
                Session["TimeSheetData"] = tsheets;
                Request.Form["Date"] = "";
                //Return the TimeSheet view
                Request.Params.Clear();
                return RedirectToAction("Timesheet", "Timesheet");*/

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
                Debug.WriteLine("The textarea says : [" + model.Note + "]");
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
                    EmpId = model.EmpId,
                    Note = model.Note
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

        public ActionResult SaveDailyTimeSheet()//for the record timestamp button.
        {
            Debug.WriteLine("In SaveDailyTimeSheet");
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
                    if (tsheets[i].AuthorizedBySupervisor.ToString().Trim().Equals("False"))
                    {
                        string today = DateTime.Now.ToString("HH:mm");

                        if (tsheets[i].TimeIn.ToString().Trim().Equals("0:00"))
                        {
                            tsheets[i].TimeIn = today;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                            message = "1st punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                            Session["DailyMessage"] = message;
                        }
                        else
                        {
                            if (tsheets[i].OutForLunch.ToString().Trim().Equals("0:00"))
                            {
                                tsheets[i].OutForLunch = today;
                                tsheets[i].UpdateTimeSheet(tsheets[i]);
                                message = "2nd punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                Session["DailyMessage"] = message;
                            }
                            else
                            {
                                if (tsheets[i].InFromLunch.ToString().Trim().Equals("0:00"))
                                {
                                    tsheets[i].InFromLunch = today;
                                    tsheets[i].UpdateTimeSheet(tsheets[i]);
                                    message = "3rd punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                    Session["DailyMessage"] = message;
                                }
                                else
                                {
                                    if (tsheets[i].TimeOut.ToString().Trim().Equals("0:00"))
                                    {
                                        tsheets[i].TimeOut = today;
                                        tsheets[i].UpdateTimeSheet(tsheets[i]);
                                        message = "4th punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                        Session["DailyMessage"] = message;
                                    }
                                    else
                                    {
                                        message = "All 4 punches have been used, please use additional hours for more time worked.";
                                        Session["DailyMessage"] = message;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Timesheet has already been approved. no changes can be made";
                        Session["DailyMessage"] = message;
                    }

                }

            }

            return RedirectToAction("DailyTimesheet", "Timesheet");

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
                    if (tsheets[i].AuthorizedBySupervisor.ToString().Trim().Equals("False"))
                    {
                        string today = DateTime.Now.ToString("HH:mm");

                        if (tsheets[i].TimeIn.ToString().Trim().Equals("0:00"))
                        {
                            tsheets[i].TimeIn = today;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                            message = "1st punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                            Session["QuickTimeStamp"] = message;
                        }
                        else
                        {
                            if (tsheets[i].OutForLunch.ToString().Trim().Equals("0:00"))
                            {
                                tsheets[i].OutForLunch = today;
                                tsheets[i].UpdateTimeSheet(tsheets[i]);
                                message = "2nd punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                Session["QuickTimeStamp"] = message;
                            }
                            else
                            {
                                if (tsheets[i].InFromLunch.ToString().Trim().Equals("0:00"))
                                {
                                    tsheets[i].InFromLunch = today;
                                    tsheets[i].UpdateTimeSheet(tsheets[i]);
                                    message = "3rd punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                    Session["QuickTimeStamp"] = message;
                                }
                                else
                                {
                                    if (tsheets[i].TimeOut.ToString().Trim().Equals("0:00"))
                                    {
                                        tsheets[i].TimeOut = today;
                                        tsheets[i].UpdateTimeSheet(tsheets[i]);
                                        message = "4th punch has been added at: " + DateTime.Now.ToString("h:mm tt");
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
                    else
                    {
                        message = "Timesheet has already been approved. no changes can be made";
                        Session["QuickTimeStamp"] = message;
                    }

                }

            }

            return RedirectToAction("Index", "Employees");
        }
       
    }
}