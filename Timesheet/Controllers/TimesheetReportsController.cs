using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Timesheet.Controllers;

namespace TimesheetReports.Controllers
{
    public class TimesheetReportsController : Controller
    {
        // GET: TimesheetReports
        public ActionResult TimesheetReports()
        {
            return View();
        }
    }
    [ActionName (TimesheetReportsController)]
    [HttpPost]
    public ActionResult TimesheetReports(string SubmitButton, int? id)
    {
        String buttonClicked = SubmitButton;
        SubmitButton = "Approve";
        
        if(id == null)
    
    {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        String buttonClicked = SubmitButton;
        if(buttonClicked == “Approve”)
    
    {
            CurrentApplication currentApplication = db.CurrentApplications.Find(id);
            currentApplication = “success”;
            db.SaveChanges();
        }

        else if (buttonClicked == “Decline”)
	{
            CurrentApplication currentApplication = db.CurrentApplication.AppStatus = “fail”;
            db.SaveChanges();
        }
        return (supervisor.SupervisorController);
    }

    internal class supervisor
    {
    }
}
