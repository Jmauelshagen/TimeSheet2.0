using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
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
            model.WeekEndingDates = GetWeekEndingDateList();
            return View(model);
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

            Session["PaySummaryList"] = paySumList;
            
            return RedirectToAction("Index", "HR");
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
        public async Task<ActionResult> email(FormCollection form) //receives form
        {
            var name = form["empname"];
            var subject = form["empsub"];
            var email = form["empemail"].Trim();
            var messages = form["smessage"];
            var x = await SendEmail(name, subject, email, messages); 
            if (x == "sent")
                ViewData["esent"] = "Your Message Has Been Sent";
            return RedirectToAction("Index", "HR");
        }
  
        //SendEmail method
        private async Task<string> SendEmail(string name, string subject, string email, string messages)
        {
            MailMessage message = new MailMessage(); //initializes new instance of mailmessage class 
            var emp = (Employee)Session["Employee"]; 
            message.To.Add(new MailAddress("raulochoa413@yahoo.com")); //initializes new instance of mailaddress class
            message.To.Add(new MailAddress(email)); //initializes new instance of mailaddress class
            //message.From = new MailAddress(emp.Email);  
            message.From = new MailAddress("hr.testingctc@gmail.com");
            message.Subject =  subject + ":Message From" + email;
            message.Body = "Name: " + name + "Subject:" + subject +  "\nTo: " + email + "\n" + messages;
            message.IsBodyHtml = true;
            using (SmtpClient smtp = new SmtpClient())
            {
                var credential = new System.Net.NetworkCredential //credentials check
                {
                    UserName = "hr.testingctc@gmail.com",  // replace with sender's email id 
                    Password = "P@s$w0rd"  // replace with password 
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                //smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
                return "sent";
            }
        }
        //end of email controller 

    }
}