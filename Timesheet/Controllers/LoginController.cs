using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;

namespace Timesheet.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(Timesheet.Models.Emp empModel)
        {
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                var empDetails = db.Emps.Where(x => x.UserName == empModel.UserName && x.Password == empModel.Password)
                    .FirstOrDefault();
                if (empDetails == null)
                {
                    empModel.LoginErrorMessage = "wrong UserName or Password";
                }
                else
                {
                    Session["empId"] = empDetails.EmpId;
                    return RedirectToAction("Index", "Employees");
                }
                  
            }
            return View("Index",empModel);
        }

        public ActionResult LogOut()
        {
           Session.Abandon();
            return RedirectToAction("Index","Login");
        }
   
        

        
    }
}