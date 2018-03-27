using System;
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
            //if (model.isEnabled)
            //{
            //    String result = "True";
            //    foreach(int empID in empIds)
            //    {
            //        paySumList.Add(new PaySummary(empID, wED, result));
            //    }
            //}
            //else
            {
                
                foreach (int empId in empIds)
                {
                    paySumList.Add(new PaySummary(empId, wED));
                }
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
            //message.From = new MailAddress(emp.Email);
            message.Subject = subject;
            message.Body = messages;
            message.IsBodyHtml = false;
            using (SmtpClient smtp = new SmtpClient())
            {
                var credential = new System.Net.NetworkCredential //credentials check
                {
                    UserName = "hr.testingctc@gmail.com",  // replace with sender's email id 
                    Password = "P@s$w0rd"  // replace with password 
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
                    string messages = "Dear " + emp.First_Name.Trim() + "," + Environment.NewLine + "Changes have been made to " + CurrentDate + " Timesheet, Please reivew changes and call HR if you have any questions." + Environment.NewLine +
                        "Old Timesheet data - Time In: " + tsheets[i].TimeIn.Trim() + " Time Out: " + tsheets[i].OutForLunch.Trim() + " Time In: " + tsheets[i].InFromLunch.Trim() + " Time Out: " + tsheets[i].TimeOut.Trim() + " Leave ID: " + tsheets[i].LeaveId + " Leave Hours: " + tsheets[i].LeaveHours.Trim() + " Additional Hours: " + tsheets[i].AdditionalHours.Trim();

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
                    messages = messages + Environment.NewLine + "New Timesheet data - Time In: " + tsheets[i].TimeIn.Trim() + " Time Out: " + tsheets[i].OutForLunch.Trim() + " Time In: " + tsheets[i].InFromLunch.Trim() + " Time Out: " + tsheets[i].TimeOut.Trim() + " Leave ID: " + tsheets[i].LeaveId + " Leave Hours: " + tsheets[i].LeaveHours.Trim() + " Additional Hours: " + tsheets[i].AdditionalHours.Trim() +
                        Environment.NewLine + Environment.NewLine + "Thanks," + Environment.NewLine + hr.First_Name.Trim() + " " + hr.Last_Name.Trim();
                    var x = await SendEmail(name, subject, email, messages);
                    if (x == "sent")
                        ViewData["esent"] = "Your Message Has Been Sent";
                    Debug.WriteLine("Message Was sent");
                }
            }

            return RedirectToAction("Overview", "HR");
        }
    }
}