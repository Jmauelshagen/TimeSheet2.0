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
    public class ReportsController : Controller
    {
        //Establish db connection
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();
        // GET: Reports
        //Forwards user to Supervisor screen after getting employee names and week ending dates
        //to be used in the menu/form
        public ActionResult Reports()
        {
            var sup = (Employee)Session["Supervisor"];
            Session["EmployeeList"] = GetEmployeeNames(sup.Banner_ID);
            Session["WeekendingList"] = GetWeekEndingDateList();

            return View();
        }

        //Obtains a list of week ending dates and adds them to a select list object
        //to be used in the UI as a menu
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

        //Obtains the time sheet data corresponding to the selected employee name and week ending date
        //Redirects users back to the supervisor screen after putting time sheet info into the session object
        [HttpPost]
        public ActionResult ReportData(TimeSheet model)
        {
            Debug.WriteLine("Name : " + model.Name + " and Weekending : " + model.WeekEnding + " ]]");
            TimeSheet timeSheet = new TimeSheet();
            if (model.Name == null || model.WeekEnding == null)
            {
                string message = "***Please select the employee name and Weekend date***";
                Session["Message"] = message;
                Session.Remove("TimeSheetData");
                return RedirectToAction("Reports", "Reports");
            }
            else
            {
                var name = model.Name.Trim();
                var wED = model.WeekEnding.Trim();
                List<TimeSheet> reportList = timeSheet.GetTimeSheetByIdAndDate(Convert.ToInt16(name), wED);
                if(reportList.Count != 0)
                {
                    Session["TimeSheetData"] = reportList;
                }
                else
                {
                    Session.Remove("TimeSheetData");
                }
                Employee ep = new Employee().GetEmployee(Convert.ToInt16(name));
                Session["Employee"] = ep;
                Session.Remove("Message");

                return RedirectToAction("Reports", "Reports");

            }

        }

        //Updates the AuthorizedBySupervisor column to True for each day of the week that is approved
        //Redirects back to the Supervisor screen with a success message
        [HttpPost]
        public ActionResult Approve()
        {
            List<TimeSheet> list = (List<TimeSheet>)Session["TimeSheetData"];
            foreach (TimeSheet sheet in list)
            {
                sheet.AuthorizedBySupervisor = "True";
                sheet.UpdateTimeSheet(sheet);
            }
            string message = "Time sheet is approved.";
            Session["Message"] = message;
            return RedirectToAction("Reports", "Reports");
        }

        public async Task<ActionResult> Deny() //receives form
        {
            List<TimeSheet> list = (List<TimeSheet>)Session["TimeSheetData"];
            foreach (TimeSheet sheet in list)
            {
                sheet.Submitted = "False";
                sheet.AuthorizedBySupervisor = "False";
                sheet.UpdateTimeSheet(sheet);
            }
            string message = "Time sheet is denied. Email has been sent to the employee to have corrections made.";
            Session["Message"] = message;
            Employee emp = (Employee)Session["Employee"];
            var name = emp.First_Name + " " + emp.Last_Name;
            var subject = "Your timesheet was denied.";
            var email = (string)emp.Email_Address;
            var messages = "Dear " + emp.First_Name + ", Your timesheet ending in: " + list[0].WeekEnding + " has been denied by your supervisor. please review and resubmit the Timesheet.";
            Debug.WriteLine("Check 1");
            var x = await SendEmail(name, subject, email, messages);
            if (x == "sent")
            {
                Debug.WriteLine("Check 4");
                ViewData["esent"] = "Your Message Has Been Sent";
                Debug.WriteLine("Message Was sent");
            }
            return RedirectToAction("Reports", "Reports");
        }

        //SendEmail method
        private async Task<string> SendEmail(string name, string subject, string email, string messages)
        {
            MailMessage message = new MailMessage(); //initializes new instance of mailmessage class 
            var sup = (Employee)Session["Supervisor"];
            Debug.WriteLine("HR email: " + sup.Email_Address);
            message.To.Add(new MailAddress(email)); //initializes new instance of mailaddress class
            message.From = new MailAddress(sup.Email_Address);
            message.Subject = subject;
            message.Body = messages;
            message.IsBodyHtml = true;
            using (SmtpClient smtp = new SmtpClient())
            {
                Email em = new Email();
                em = em.GetEmail(sup.Email_Address);
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
    }
}