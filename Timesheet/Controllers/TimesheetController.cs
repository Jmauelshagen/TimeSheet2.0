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
        public ActionResult OldTimesheet()
        {
            Employee emp = (Employee)Session["Employee"];
            Session["WeekList"] = GetWeekEndingDateList(emp.EmpId);
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
        public ActionResult SubmitOldTimesheet(TimeSheet model)
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

            return RedirectToAction("OldTimesheet", "Timesheet");

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
            Session["Message2"] = "";

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
            Session["Message2"] = message;
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
        /**This next seciton returns the stored date selected from the drop down. It then calls a method
        * to retrieves a specific timesheet based on the day and user. Then sets the current model
        * to the timesheet to ensure integrity and allow the note to be updated below**/
        [HttpPost]
        public ActionResult SaveTimeNote(TimeSheet model)
        {      
            try
            {
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
                Session["Message2"] = "";
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
                                if (model.Note.ToString().Equals("None") || model.Note.ToString().Equals("none")) {
                                    tsheets[i].Note = "";
                                    model.Note = "";
                                }
                                else { tsheets[i].Note = model.Note; }
                            }
                            model.AdditionalHours = tsheets[i].AdditionalHours;
                            if (model.AdditionalHours.ToString().Trim().Equals("0:00") && !String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 1");
                                string mess = "You created a note but have no additional hours. This may be a mistake.";
                                Session["Message2"] = mess;
                            }
                            if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 2");
                                string mess = "You have addtional hours. You might want to make a note.";
                                Session["Message2"] = mess;
                            }
                            Debug.WriteLine("AddH : " + model.AdditionalHours + "/ Note: " + model.Note);
                            Debug.WriteLine("The message says :" + Session["Message2"] + "||");
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
                Session["Message2"] = "";
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
                                Session["Message2"] = mess;
                            }
                            if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 2");
                                string mess = "You have addtional hours. You might want to make a note.";
                                Session["Message2"] = mess;
                            }
                            Debug.WriteLine("AddH : " + model.AdditionalHours + "/ Note: " + model.Note);
                            Debug.WriteLine("The message says :" + Session["Message2"] + "||");
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
            Session["Message2"] = "";


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
                        if (!String.IsNullOrEmpty(model.TimeIn)) { tsheets[i].TimeIn = model.TimeIn; }
                        if (!String.IsNullOrEmpty(model.OutForLunch)) { tsheets[i].OutForLunch = model.OutForLunch; }
                        if (!String.IsNullOrEmpty(model.InFromLunch)) { tsheets[i].InFromLunch = model.InFromLunch; }
                        if (!String.IsNullOrEmpty(model.TimeOut)) { tsheets[i].TimeOut = model.TimeOut; }
                        if (!String.IsNullOrEmpty(model.LeaveId.ToString())) { tsheets[i].LeaveId = model.LeaveId; }
                        if (!String.IsNullOrEmpty(model.LeaveHours)) { tsheets[i].LeaveHours = model.LeaveHours; }
                        if (!String.IsNullOrEmpty(model.AdditionalHours)) { tsheets[i].AdditionalHours = model.AdditionalHours; }
                        if (!String.IsNullOrEmpty(model.Note))
                        {
                            if (model.Note.ToString().Trim().Equals("None") || model.Note.ToString().Trim().Equals("none")) {
                                tsheets[i].Note = ""; }
                            else { tsheets[i].Note = model.Note + " - " + model.Date; }
                        }

                        model.Note = tsheets[i].Note;
                        if (model.AdditionalHours.ToString().Trim().Equals("0:00") && !String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 1");
                            string mess = "You created a note but have no additional hours. This may be a mistake.";
                            Session["Message2"] = mess;
                        }
                        if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 2");
                            string mess = "You have addtional hours. You might want to make a note.";
                            Session["Message2"] = mess;
                        }
                        Debug.WriteLine("AddH : " + model.AdditionalHours + "/ Note: " + model.Note);
                        Debug.WriteLine("The message says :" + Session["Message2"] + "||");
                        message = "Timesheet Saved Succesfully";
                        Session["DailyMessage"] = message;
                        tsheets[i].UpdateTimeSheet(tsheets[i]);
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
            Session["Message2"] = "";
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

        /******************************************************************************************************************************************/
        [HttpPost]
        public ActionResult SaveOldTimeNote(TimeSheet model)
        {
            /**This next seciton returns the stored date selected from the drop down. It then calls a method
            * to retrieves a specific timesheet based on the day and user. Then sets the current model
            * to the timesheet to ensure integrity and allow the note to be updated below**/

            //Get list of TimeSheet objects based on date and employee id and add list to session
            List<TimeSheet>  tsheets = (List<TimeSheet>)Session["TimeSheetData"];
            Session["Message2"] = "";

            //string CurrentDate = Request.Form["Date"].ToString().Trim();
            string CurrentDate = model.Date.Trim();
            string message;
            Debug.WriteLine(CurrentDate);

            for (int i = 0; i < 7; i++)
            {
                Debug.WriteLine("Save the Note");

                if (tsheets[i].Date.ToString().Trim().Equals(CurrentDate))
                {

                    if (tsheets[i].AuthorizedBySupervisor.ToString().Trim().Equals("False"))
                    {

                        if (!String.IsNullOrEmpty(model.Note))
                        {
                            if (model.Note.ToString().Equals("None") || model.Note.ToString().Equals("none")) { tsheets[i].Note = ""; }
                            else { tsheets[i].Note = model.Note + " - " + model.Date; }
                        }
                        model.AdditionalHours = tsheets[i].AdditionalHours;
                        if (model.AdditionalHours.ToString().Trim().Equals("0:00") && !String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 1");
                            string mess = "You created a note but have no additional hours. This may be a mistake.";
                            Session["Message2"] = mess;
                        }
                        if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 2");
                            string mess = "You have addtional hours. You might want to make a note.";
                            Session["Message2"] = mess;
                        }
                        message = "Timesheet Saved Succesfully";
                        tsheets[i].UpdateTimeSheet(tsheets[i]);
                        Session["TimeSheetData"] = tsheets;
                        Session["Message"] = message;

                    }
                    else
                    {
                        message = "Timesheet has already been approved. no changes can be made";
                        Session["Message"] = message;
                    }
                }
            }

            return RedirectToAction("OldTimesheet", "Timesheet");
        }

        [HttpPost]
        public ActionResult SaveOldTimeSheet(TimeSheet model)
        {
            //Get list of TimeSheet objects based on date and employee id and add list to session
            List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];
            Session["Message2"] = "";

            //string CurrentDate = Request.Form["Date"].ToString().Trim();
            string CurrentDate = model.Date.Trim();
            string message;
            Debug.WriteLine(CurrentDate);

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
                            Session["Message2"] = mess;
                        }
                        if (!model.AdditionalHours.ToString().Trim().Equals("0:00") && String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 2");
                            string mess = "You have addtional hours. You might want to make a note.";
                            Session["Message2"] = mess;
                        }
                        message = "Timesheet Saved Succesfully";
                        tsheets[i].UpdateTimeSheet(tsheets[i]);
                        Session["TimeSheetData"] = tsheets;
                        Session["Message"] = message;

                    }
                    else
                    {
                        message = "Timesheet has already been approved. no changes can be made";
                        Session["Message"] = message;
                    }
                }
            }

            return RedirectToAction("OldTimesheet", "Timesheet");
        }

        //Obtains the time sheet data corresponding to the selected employee name and week ending date
        //Redirects users back to the supervisor screen after putting time sheet info into the session object
        [HttpPost]
        public ActionResult ReportData(TimeSheet model)
        {
            //Pull the employee object from the session.
            Employee emp = (Employee)Session["Employee"];

            Debug.WriteLine("Name : " + emp.FirstName +" "+emp.LastName + " and Weekending : " + model.WeekEnding + " ]");
            if (Session["Message"] != null)
            {
                Session.Remove("Message");
            }
            if (model.WeekEnding == null)
            {
                string message = "***Please select the employee name and Weekend date***";
                Session["Message"] = message;
                return RedirectToAction("OldTimesheet", "Timesheet");
            }

            string wED = model.WeekEnding.Trim();
            List<TimeSheet> tsheets = model.GetTimeSheetByIdAndDate(emp.EmpId, wED);
            Session["TimeSheetData"] = tsheets;
            IEnumerable<SelectListItem> dateList = GetListOfDays(emp.EmpId, wED);
            Session["dateList"] = dateList;
            List<string> dates = GetDaysInTimeSheet(emp.EmpId, wED);
            Session["dates"] = dates;

            return RedirectToAction("OldTimesheet", "Timesheet");
        }

        private IEnumerable<SelectListItem> GetListOfDays(int id, string wed)
        {
            List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];
            List<SelectListItem> dates = new List<SelectListItem>();
            foreach (string date in tsheets[0].GetDates(id,wed))
            {
                dates.Add(new SelectListItem
                {
                    Value = date,
                    Text = date
                });
            }
            return dates;
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

        //Obtains a list of week ending dates and adds them to a select list object
        //to be used in the UI as a menu
        private IEnumerable<SelectListItem> GetWeekEndingDateList(int id)
        {
            TimeSheet tsheet = new TimeSheet();
            List<SelectListItem> dateList = new List<SelectListItem>();
            foreach (string date in tsheet.GetWeekEndingDateList(id))
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