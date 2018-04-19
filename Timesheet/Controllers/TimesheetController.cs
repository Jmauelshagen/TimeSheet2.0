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
            Employee emp = (Employee)Session["Employee"];
            Session["datelist"] = GetListOfDays();
            return View();
        }
        public ActionResult DailyTimesheet()
        {
            Employee emp = (Employee)Session["Employee"];
            Session["datelist"] = GetListOfDays();
            return View();
        }
        public ActionResult OldTimesheet()
        {
            Employee emp = (Employee)Session["Employee"];
            Session["WeekList"] = GetWeekEndingDateList(emp.Banner_ID);
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
            Session["CurrentMessage"] = message;

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
            Session["OldMessage"] = message;

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
            List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.Banner_ID, dates);
            Session["TimeSheetData"] = tsheets;

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
            List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.Banner_ID, dates);
            Session["TimeSheetData"] = tsheets;

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
                List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.Banner_ID, dates);
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
                                if (model.Note.ToString().Equals("None") || model.Note.ToString().Equals("none"))
                                {
                                    tsheets[i].Note = "";
                                    model.Note = "";
                                }
                                else { tsheets[i].Note = model.Note + " - " + model.Date; }
                            }
                            model.AdditionalHours = tsheets[i].AdditionalHours;
                            if (String.IsNullOrEmpty(model.AdditionalHours) && !String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 1");
                                string mess = "You created a note but have no additional hours. This may be a mistake.";
                                Session["Message2"] = mess;
                            }
                            if (!String.IsNullOrEmpty(model.AdditionalHours) && String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 2");
                                string mess = "You have addtional hours. You might want to make a note.";
                                Session["Message2"] = mess;
                            }
                            Debug.WriteLine("AddH : " + model.AdditionalHours + " Note: " + model.Note);
                            Debug.WriteLine("The message says :" + Session["Message2"]);
                            message = "Note Saved Succesfully";
                            Session["CurrentMessage"] = message;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                        }
                        else
                        {
                            message = "Timesheet has already been approved. no changes can be made";
                            Session["CurrentMessage"] = message;
                        }

                    }
                }
                Session["Message"] = "";
                Session["OldMessage"] = "";
                Session["DailyMessage"] = "";
                return RedirectToAction("Timesheet", "Timesheet");
            }
            catch (Exception ex)
            {
                Session["Message"] = "";
                Session["OldMessage"] = "";
                Session["DailyMessage"] = "";
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
                List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.Banner_ID, dates);
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
                            else { tsheets[i].TimeIn = ""; }

                            if (!String.IsNullOrEmpty(model.OutForLunch)) { tsheets[i].OutForLunch = model.OutForLunch; }
                            else { tsheets[i].OutForLunch = ""; }

                            if (!String.IsNullOrEmpty(model.InFromLunch)) { tsheets[i].InFromLunch = model.InFromLunch; }
                            else { tsheets[i].InFromLunch = ""; }

                            if (!String.IsNullOrEmpty(model.TimeOut)) { tsheets[i].TimeOut = model.TimeOut; }
                            else { tsheets[i].TimeOut = ""; }

                            if (!String.IsNullOrEmpty(model.LeaveId.ToString())) { tsheets[i].LeaveId = model.LeaveId; }
                            else { tsheets[i].LeaveId = 0; }

                            if (!String.IsNullOrEmpty(model.LeaveHours)) { tsheets[i].LeaveHours = model.LeaveHours; }
                            else { tsheets[i].LeaveHours = ""; }

                            if (!String.IsNullOrEmpty(model.AdditionalHours)) { tsheets[i].AdditionalHours = model.AdditionalHours; }
                            else { tsheets[i].AdditionalHours = ""; }

                            if (String.IsNullOrEmpty(model.AdditionalHours) && !String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 1");
                                string mess = "You created a note but have no additional hours. This may be a mistake.";
                                Session["Message2"] = mess;
                            }
                            if (!String.IsNullOrEmpty(model.AdditionalHours) && String.IsNullOrEmpty(model.Note))
                            {
                                Debug.WriteLine("In Erro 2");
                                string mess = "You have addtional hours. You might want to make a note.";
                                Session["Message2"] = mess;
                            }
                            Debug.WriteLine("AddH : " + model.AdditionalHours + "/ Note: " + model.Note);
                            Debug.WriteLine("The message says :" + Session["Message2"] + "||");
                            message = "Timesheet Saved Succesfully";
                            Session["CurrentMessage"] = message;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                        }
                        else
                        {
                            message = "Timesheet has already been approved. no changes can be made";
                            Session["CurrentMessage"] = message;
                        }
                    }
                }
                Session["Message"] = "";
                Session["OldMessage"] = "";
                Session["DailyMessage"] = "";
                return RedirectToAction("Timesheet", "Timesheet");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Session["Message"] = "";
                Session["OldMessage"] = "";
                Session["DailyMessage"] = "";
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
            List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.Banner_ID, dates);
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
                        else { tsheets[i].TimeIn = ""; }

                        if (!String.IsNullOrEmpty(model.OutForLunch)) { tsheets[i].OutForLunch = model.OutForLunch; }
                        else { tsheets[i].OutForLunch = ""; }

                        if (!String.IsNullOrEmpty(model.InFromLunch)) { tsheets[i].InFromLunch = model.InFromLunch; }
                        else { tsheets[i].InFromLunch = ""; }

                        if (!String.IsNullOrEmpty(model.TimeOut)) { tsheets[i].TimeOut = model.TimeOut; }
                        else { tsheets[i].TimeOut = ""; }

                        if (!String.IsNullOrEmpty(model.LeaveId.ToString())) { tsheets[i].LeaveId = model.LeaveId; }
                        else { tsheets[i].LeaveId = 0; }

                        if (!String.IsNullOrEmpty(model.LeaveHours)) { tsheets[i].LeaveHours = model.LeaveHours; }
                        else { tsheets[i].LeaveHours = ""; }

                        if (!String.IsNullOrEmpty(model.AdditionalHours)) { tsheets[i].AdditionalHours = model.AdditionalHours; }
                        else { tsheets[i].AdditionalHours = ""; }

                        if (!String.IsNullOrEmpty(model.Note))
                        {
                            if (model.Note.ToString().Trim().Equals("None") || model.Note.ToString().Trim().Equals("none"))
                            {
                                tsheets[i].Note = "";
                            }
                            else { tsheets[i].Note = model.Note + " - " + model.Date; }
                        }

                        model.Note = tsheets[i].Note;
                        if (String.IsNullOrEmpty(model.AdditionalHours) && !String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 1");
                            string mess = "You created a note but have no additional hours. This may be a mistake.";
                            Session["Message2"] = mess;
                        }
                        if (!String.IsNullOrEmpty(model.AdditionalHours) && String.IsNullOrEmpty(model.Note))
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
            Session["Message"] = "";
            Session["OldMessage"] = "";
            Session["CurrentMessage"] = "";

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
            List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.Banner_ID, dates);
            Session["TimeSheetData"] = tsheets;

            string CurrentDate = DateTime.Now.ToShortDateString();
            string message;
            for (int i = 0; i < 7; i++)
            {
                if (tsheets[i].Date.ToString().Trim().Equals(CurrentDate))
                {
                    if (tsheets[i].AuthorizedBySupervisor.ToString().Trim().Equals("False"))
                    {
                        string time = DateTime.Now.ToString("HH:mm");

                        if (String.IsNullOrEmpty(tsheets[i].TimeIn.Trim()))
                        {
                            tsheets[i].TimeIn = time;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                            message = "1st punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                            Session["DailyMessage"] = message;
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(tsheets[i].OutForLunch.Trim()))
                            {
                                tsheets[i].OutForLunch = time;
                                tsheets[i].UpdateTimeSheet(tsheets[i]);
                                message = "2nd punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                Session["DailyMessage"] = message;
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(tsheets[i].InFromLunch.Trim()))
                                {
                                    tsheets[i].InFromLunch = time;
                                    tsheets[i].UpdateTimeSheet(tsheets[i]);
                                    message = "3rd punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                    Session["DailyMessage"] = message;
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(tsheets[i].TimeOut.Trim()))
                                    {
                                        tsheets[i].TimeOut = time;
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
            Session["Message"] = "";
            Session["OldMessage"] = "";
            Session["CurrentMessage"] = "";

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
            Session["datelist"] = GetListOfDays();
            List<string> dates = tsheet.GetDates();
            Session["Dates"] = dates;

            //Get list of TimeSheet objects based on date and employee id and add list to session
            List<TimeSheet> tsheets = tsheet.GetTimeSheetByWeek(emp.Banner_ID, dates);
            Session["TimeSheetData"] = tsheets;

            string CurrentDate = DateTime.Now.ToShortDateString();
            string message;
            for (int i = 0; i < 7; i++)
            {
                if (tsheets[i].Date.ToString().Trim().Equals(CurrentDate))
                {
                    if (tsheets[i].AuthorizedBySupervisor.ToString().Trim().Equals("False"))
                    {
                        string time = DateTime.Now.ToString("HH:mm"); //gets the current time in military time

                        if (String.IsNullOrEmpty(tsheets[i].TimeIn.Trim()))
                        {
                            tsheets[i].TimeIn = time;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                            message = "1st punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                            Session["Message"] = message;
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(tsheets[i].OutForLunch.Trim()))
                            {
                                tsheets[i].OutForLunch = time;
                                tsheets[i].UpdateTimeSheet(tsheets[i]);
                                message = "2nd punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                Session["Message"] = message;
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(tsheets[i].InFromLunch.Trim()))
                                {
                                    tsheets[i].InFromLunch = time;
                                    tsheets[i].UpdateTimeSheet(tsheets[i]);
                                    message = "3rd punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                    Session["Message"] = message;
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(tsheets[i].TimeOut.Trim()))
                                    {
                                        tsheets[i].TimeOut = time;
                                        tsheets[i].UpdateTimeSheet(tsheets[i]);
                                        message = "4th punch has been added at: " + DateTime.Now.ToString("h:mm tt");
                                        Session["Message"] = message;
                                    }
                                    else
                                    {
                                        message = "All 4 punches have been used, please go to daily timesheet to enter in additional hours.";
                                        Session["Message"] = message;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Timesheet has already been approved. no changes can be made";
                        Session["Message"] = message;
                    }

                }

            }
            Session["OldMessage"] = "";
            Session["DailyMessage"] = "";
            Session["CurrentMessage"] = "";

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
            List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];
            Session["Message2"] = "";

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
                        if (String.IsNullOrEmpty(model.AdditionalHours) && !String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 1");
                            string mess = "You created a note but have no additional hours. This may be a mistake.";
                            Session["Message2"] = mess;
                        }
                        if (!String.IsNullOrEmpty(model.AdditionalHours) && String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 2");
                            string mess = "You have addtional hours. You might want to make a note.";
                            Session["Message2"] = mess;
                        }
                        message = "Note Saved Succesfully";
                        tsheets[i].UpdateTimeSheet(tsheets[i]);
                        Session["TimeSheetData"] = tsheets;
                        Session["OldMessage"] = message;

                    }
                    else
                    {
                        message = "Timesheet has already been approved. no changes can be made";
                        Session["OldMessage"] = message;
                    }
                }
            }
            Session["Message"] = "";
            Session["DailyMessage"] = "";
            Session["CurrentMessage"] = "";

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
                        else { tsheets[i].TimeIn = ""; }

                        if (!String.IsNullOrEmpty(model.OutForLunch)) { tsheets[i].OutForLunch = model.OutForLunch; }
                        else { tsheets[i].OutForLunch = ""; }

                        if (!String.IsNullOrEmpty(model.InFromLunch)) { tsheets[i].InFromLunch = model.InFromLunch; }
                        else { tsheets[i].InFromLunch = ""; }

                        if (!String.IsNullOrEmpty(model.TimeOut)) { tsheets[i].TimeOut = model.TimeOut; }
                        else { tsheets[i].TimeOut = ""; }

                        if (!String.IsNullOrEmpty(model.LeaveId.ToString())) { tsheets[i].LeaveId = model.LeaveId; }
                        else { tsheets[i].LeaveId = 0; }

                        if (!String.IsNullOrEmpty(model.LeaveHours)) { tsheets[i].LeaveHours = model.LeaveHours; }
                        else { tsheets[i].LeaveHours = ""; }

                        if (!String.IsNullOrEmpty(model.AdditionalHours)) { tsheets[i].AdditionalHours = model.AdditionalHours; }
                        else { tsheets[i].AdditionalHours = ""; }


                        if (String.IsNullOrEmpty(model.AdditionalHours) && !String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 1");
                            string mess = "You created a note but have no additional hours. This may be a mistake.";
                            Session["Message2"] = mess;
                        }
                        if (!String.IsNullOrEmpty(model.AdditionalHours) && String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 2");
                            string mess = "You have addtional hours. You might want to make a note.";
                            Session["Message2"] = mess;
                        }
                        message = "Timesheet Saved Succesfully";
                        tsheets[i].UpdateTimeSheet(tsheets[i]);
                        Session["TimeSheetData"] = tsheets;
                        Session["OldMessage"] = message;

                    }
                    else
                    {
                        message = "Timesheet has already been approved. no changes can be made";
                        Session["OldMessage"] = message;
                    }
                }
            }
            Session["Message"] = "";
            Session["DailyMessage"] = "";
            Session["CurrentMessage"] = "";

            return RedirectToAction("OldTimesheet", "Timesheet");
        }

        //Obtains the time sheet data corresponding to the selected employee name and week ending date
        //Redirects users back to the previous timesheet screen after putting time sheet info into the session object
        [HttpPost]
        public ActionResult ReportData(TimeSheet model)
        {
            //Pull the employee object from the session.
            Employee emp = (Employee)Session["Employee"];

            Debug.WriteLine("Name : " + emp.First_Name + " " + emp.Last_Name + " and Weekending : " + model.WeekEnding + " ]");
            if (model.WeekEnding == null)
            {
                string message = "***Please select the Weekend date***";
                Session["OldMessage"] = message;
                return RedirectToAction("OldTimesheet", "Timesheet");
            }
            else
            {

                string wED = model.WeekEnding.Trim();
                List<TimeSheet> tsheets = model.GetTimeSheetByIdAndDate(emp.Banner_ID, wED);
                Session["TimeSheetData"] = tsheets;
                IEnumerable<SelectListItem> dateList = GetListOfDays(emp.Banner_ID, wED);
                Session["dateList"] = dateList;
                List<string> dates = GetDaysInTimeSheet(emp.Banner_ID, wED);
                Session["dates"] = dates;

                Session["Message"] = "";
                Session["DailyMessage"] = "";
                Session["CurrentMessage"] = "";
                Session["OldMessage"] = "";

                return RedirectToAction("OldTimesheet", "Timesheet");
            }

        }

        private IEnumerable<SelectListItem> GetListOfDays(int id, string wed)
        {
            List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];
            List<SelectListItem> dates = new List<SelectListItem>();
            foreach (string date in tsheets[0].GetDates(id, wed))
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