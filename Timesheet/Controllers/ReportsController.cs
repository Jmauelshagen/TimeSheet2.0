using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;

namespace Reports.Controllers
{
    public class ReportsController : Controller
    {
        public object Emps { get; private set; }

        // GET: Reports,
        public ActionResult Reports()
        {
            LoginDatabaseEntities1 db = new LoginDatabaseEntities1();

            ViewBag.Employees = new SelectList(db.Employees, "EmpId", "FirstName");
            return View();
        }

        [ActionName("Reports")]
        [HttpPost]
        public ActionResult IndexPost(string SubmitButton, int? id)

            {
                LoginDatabaseEntities1 db = new LoginDatabaseEntities1();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                string buttonClicked = SubmitButton;
                if (buttonClicked == "Approve")
                {
                    CurrentApplication currentApplication = db.CurrentApplications.Find(Emps);
                    currentApplication.AppStatus = "APPROVED";
                    db.SaveChanges();
                }
                else if (buttonClicked == "Decline")
                {
                    CurrentApplication currentApplication = db.CurrentApplications.Find(Emps);
                    currentApplication.AppStatus = "UNAPPROVED";
                    db.SaveChanges();
                }
                return RedirectToAction("Reports");
            }

        private class CurrentApplication
        {
            public string AppStatus { get; internal set; }
        }
    }
    
}       
