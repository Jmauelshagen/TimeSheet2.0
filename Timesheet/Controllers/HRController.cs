﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
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

            IEnumerable<SelectListItem> WeekendingList = GetWeekEndingDateList();
            Session["WeekendingList"] = WeekendingList;
            return View();
        }

        public ActionResult Overview(TimeSheet model)
        {            
            return View();
        }

        //This controller obtains a list of pay summary objects for the week selected in the UI
        public ActionResult GetPayData(TimeSheet model)
        {
            List<WeeklyReport> paySumList = new List<WeeklyReport>();
            WeeklyReport paySum = new WeeklyReport();       
            var wED = model.WeekEnding;
            List<int> empIds = paySum.GetBanner_IDsByWeekEndDate(wED);

            foreach (int empId in empIds)
            {
                paySumList.Add( paySum.getWeeklyReport(empId,wED));
            }
           
            Session["Weekend"] = wED;
            Session["PaySummaryList"] = paySumList;

            return RedirectToAction("Index", "HR");
        }

        [HttpPost]
        public ActionResult GetOverview(TimeSheet model)
        {
            Debug.WriteLine("In GetOverview");
            Debug.WriteLine(model.Banner_ID);
            //Remove the TimeSheet variable from the session if it exists
            if (Session["TimeSheetData"] != null)
            {
                Session.Remove("TimeSheetData");
            }
            //Pull the employee object from the session.
            Employee emp = new Employee();
            emp = emp.GetEmployee((int)model.Banner_ID);
            Session["Employee"] = emp;
            Session["Message2"] = "";
            //Instantiate a TimeSheet object
            TimeSheet tsheet = new TimeSheet();

            //Get list of TimeSheet objects based on date and employee id and add list to session  
            string wed = (string)Session["Weekend"];
            List<TimeSheet> tsheets = model.GetTimeSheetByIdAndDate(emp.Banner_ID, wed);
            Session["TimeSheetData"] = tsheets;

            IEnumerable<SelectListItem> dateList = GetListOfDays(emp.Banner_ID, wed);
            Session["dateList"] = dateList;

            //Get list of dates for the selected weekend to create overview         
            List<string> dates = GetDaysInTimeSheet(emp.Banner_ID, wed);
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

        // Email function controller
        public async Task<ActionResult> Email(FormCollection form) //receives form
        {
            Employee emp = (Employee)Session["Employee"];
            var name = emp.First_Name + " " + emp.Last_Name;
            var subject = form["empsub"];
            var email = (string)emp.Email_Address.Trim();
            var messages = form["smessage"];
            var x = await SendEmail(name, subject, email, messages);
            if (x == "sent")
                ViewData["esent"] = "Your Message Has Been Sent";
            Debug.WriteLine("Message Was sent");
            return RedirectToAction("Index", "HR");
        }

        //SendEmail method
        private async Task<string> SendEmail(string name, string subject, string email, string messages)
        {
            MailMessage message = new MailMessage(); //initializes new instance of mailmessage class 
            var hr = (Employee)Session["HR"];
            Debug.WriteLine("HR email: " + hr.Email_Address);
            message.To.Add(new MailAddress(email)); //initializes new instance of mailaddress class
            message.From = new MailAddress(hr.Email_Address);
            message.Subject = subject;
            message.Body = messages;
            message.IsBodyHtml = false;
            using (SmtpClient smtp = new SmtpClient())
            {
                Email em = new Email();
                em = em.GetEmail(hr.Email_Address);
                Debug.WriteLine(em.Email_Address + " , " + em.Password);
                var credential = new System.Net.NetworkCredential //credentials check
                {
                    UserName = em.Email_Address.Trim(),  // replace with sender's email id 
                    Password = em.Password.Trim()  // replace with password 
                };
                smtp.Credentials = credential;
                //smtp.Host = "smtp-mail.outlook.com";
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
                return "sent";
            }
        }
        //end of email controller   

        [HttpPost]
        public async Task<ActionResult> SaveTimeSheet(TimeSheet model)
        {
            //Get list of TimeSheet objects based on date and employee id and add list to session
            List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];
            Session["Message2"] = "";

            string CurrentDate = model.Date.Trim();
            string message;
            Debug.WriteLine(CurrentDate);

            for (int i = 0; i < 7; i++)
            {
                if (tsheets[i].Date.ToString().Trim().Equals(CurrentDate))
                {
                    Employee emp = (Employee)Session["Employee"];
                    Employee hr = (Employee)Session["HR"];
                    string name = emp.First_Name + " " + emp.Last_Name;
                    string subject = "Changes Made";
                    string email = emp.Email_Address.Trim();
                    string messages = "Dear " + emp.First_Name.Trim() + "," + Environment.NewLine + "Changes have been made to " + CurrentDate + " Timesheet, Please review changes and call HR if you have any questions." + Environment.NewLine +
                        "Old Timesheet data -";

                    if (!String.IsNullOrEmpty(tsheets[i].TimeIn.Trim())) { messages = messages + " Time In: "+DateTime.Parse(tsheets[i].TimeIn.Trim()).ToString(@"hh\:mm tt"); }
                    else { messages = messages + " Time In: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].OutForLunch.Trim())) { messages = messages + " Time Out: " + DateTime.Parse(tsheets[i].OutForLunch.Trim()).ToString(@"hh\:mm tt"); }
                    else { messages = messages + " Time Out: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].InFromLunch.Trim())) { messages = messages + " TIme In: " + DateTime.Parse(tsheets[i].InFromLunch.Trim()).ToString(@"hh\:mm tt"); }
                    else { messages = messages + " TIme In: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].TimeOut.Trim())) { messages = messages +" Time Out: " + DateTime.Parse(tsheets[i].TimeOut.Trim()).ToString(@"hh\:mm tt"); }
                    else { messages = messages + " Time Out: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].LeaveId.ToString().Trim())) { messages = messages + " Leave ID: " + tsheets[i].LeaveId.ToString().Trim(); }
                    else { messages = messages + " Leave ID: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].LeaveHours.Trim())) { messages = messages + " Leave Hours: " + tsheets[i].LeaveHours.Trim(); }
                    else { messages = messages + " Leave Hours: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].AdditionalHours.Trim())) { messages = messages + " Additional Hours: " + tsheets[i].AdditionalHours.Trim(); }
                    else { messages = messages + " Additional Hours: None"; }

                    //checks the model and makes changes
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

                    model.AdditionalHours = tsheets[i].AdditionalHours;
                    if (String.IsNullOrEmpty(model.AdditionalHours.Trim()) && !String.IsNullOrEmpty(model.Note.Trim()))
                    {
                        Debug.WriteLine("In Erro 1");
                        string mess = "You created a note but have no additional hours. This may be a mistake.";
                        Session["Message2"] = mess;
                    }
                    else if (!String.IsNullOrEmpty(model.AdditionalHours.Trim()) && String.IsNullOrEmpty(model.Note.Trim()))
                    {
                        Debug.WriteLine("In Erro 2");
                        string mess = "You have addtional hours. You might want to make a note.";
                        Session["Message2"] = mess;
                    }
                    else
                    {
                        Debug.WriteLine("AddH : " + model.AdditionalHours + " Note: " + model.Note);
                        Debug.WriteLine("The message says :" + Session["Message2"]);
                        message = "Timesheet Saved Succesfully";
                        Session["Message2"] = "";
                        Session["Message"] = message;
                        tsheets[i].UpdateTimeSheet(tsheets[i]);
                    }
                    messages = messages + Environment.NewLine + "New Timesheet data -";

                    if (!String.IsNullOrEmpty(tsheets[i].TimeIn.Trim())) { messages = messages + " Time In: " + DateTime.Parse(tsheets[i].TimeIn.Trim()).ToString(@"hh\:mm tt"); }
                    else { messages = messages + " Time In: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].OutForLunch.Trim())) { messages = messages + " Time Out: " + DateTime.Parse(tsheets[i].OutForLunch.Trim()).ToString(@"hh\:mm tt"); }
                    else { messages = messages + " Time Out: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].InFromLunch.Trim())) { messages = messages + " TIme In: " + DateTime.Parse(tsheets[i].InFromLunch.Trim()).ToString(@"hh\:mm tt"); }
                    else { messages = messages + " TIme In: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].TimeOut.Trim())) { messages = messages + " Time Out: " + DateTime.Parse(tsheets[i].TimeOut.Trim()).ToString(@"hh\:mm tt"); }
                    else { messages = messages + " Time Out: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].LeaveId.ToString().Trim())) { messages = messages + " Leave ID: " + tsheets[i].LeaveId.ToString().Trim(); }
                    else { messages = messages + " Leave ID: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].LeaveHours.Trim())) { messages = messages + " Leave Hours: " + tsheets[i].LeaveHours.Trim(); }
                    else { messages = messages + " Leave Hours: None"; }

                    if (!String.IsNullOrEmpty(tsheets[i].AdditionalHours.Trim())) { messages = messages + " Additional Hours: " + tsheets[i].AdditionalHours.Trim(); }
                    else { messages = messages + " Additional Hours: None"; }

                    messages = messages + Environment.NewLine + Environment.NewLine + "Thanks," + Environment.NewLine + hr.First_Name.Trim() + " " + hr.Last_Name.Trim();
                    var x = await SendEmail(name, subject, email, messages);
                    if (x == "sent")
                        ViewData["esent"] = "Your Message Has Been Sent";
                    Debug.WriteLine("Message Was sent");
                }
            }

            return RedirectToAction("Overview", "HR");
        }
        public async Task<ActionResult> SaveTimeNote(TimeSheet model)
        {
            try
            {
                Debug.WriteLine("In SaveTimeNote");
                List<TimeSheet> tsheets = (List<TimeSheet>)Session["TimeSheetData"];

                Session["Message2"] = "";
                string CurrentDate = model.Date.Trim();
                string message;

                Employee emp = (Employee)Session["Employee"];
                Employee hr = (Employee)Session["HR"];
                string name = emp.First_Name + " " + emp.Last_Name;
                string subject = "Note Added/Deleted";
                string email = emp.Email_Address.Trim();
                string messages = "Dear " + emp.First_Name.Trim() + "," + Environment.NewLine + "A note has been Added/Deleted from " + CurrentDate + " Timesheet, Please review changes and call HR if you have any questions." + Environment.NewLine +
                    "Old Timesheet data -";

                for (int i = 0; i < 7; i++)
                {

                    if (tsheets[i].Date.ToString().Trim().Equals(CurrentDate))
                    {
                        if (!String.IsNullOrEmpty(tsheets[i].Note.Trim())) { messages = messages + " Note: " + tsheets[i].Note.Trim(); }
                        else { messages = messages + " Note: None"; }

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
                        else if (!String.IsNullOrEmpty(model.AdditionalHours) && String.IsNullOrEmpty(model.Note))
                        {
                            Debug.WriteLine("In Erro 2");
                            string mess = "You have addtional hours. You might want to make a note.";
                            Session["Message2"] = mess;
                        }
                        else
                        {
                            Debug.WriteLine("AddH : " + model.AdditionalHours + " Note: " + model.Note);
                            Debug.WriteLine("The message says :" + Session["Message2"]);
                            message = "Note Saved Succesfully";
                            Session["Message2"] = "";
                            Session["Message"] = message;
                            tsheets[i].UpdateTimeSheet(tsheets[i]);
                        }
                        messages = messages + Environment.NewLine + "New Timesheet data -";

                        if (!String.IsNullOrEmpty(tsheets[i].Note.Trim())) { messages = messages + " Note In: " + tsheets[i].Note.Trim(); }
                        else { messages = messages + " Note: None"; }

                        messages = messages + Environment.NewLine + Environment.NewLine + "Thanks," + Environment.NewLine + hr.First_Name.Trim() + " " + hr.Last_Name.Trim();
                        var x = await SendEmail(name, subject, email, messages);
                        if (x == "sent")
                            ViewData["esent"] = "Your Message Has Been Sent";
                        Debug.WriteLine("Message Was sent");
                    }
                }
                return RedirectToAction("Overview", "HR");
            }
            catch (Exception ex)
            {
                Session["Message"] = "";
                Debug.WriteLine(ex);
                return RedirectToAction("Overview", "HR");
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
    }
}