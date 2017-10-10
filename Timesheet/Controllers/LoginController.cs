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

        Employee emp1 = new Employee();
       
        [HttpPost]
        public ActionResult Authorize(Login auth, Role role)
        {
            using (LoginDatabaseEntities1 db = new LoginDatabaseEntities1())
            {
                var empDetails = db.Logins.Where(x => x.Username == auth.Username && x.Password == auth.Password)
                    .FirstOrDefault();

                var roledetails = db.Roles.Where(x => x.RoleId == role.RoleId)
                    .FirstOrDefault();


                if (empDetails == null)
                    {
                        auth.LoginErrorMessage = "Wrong UserName or Password";
                    }
                    else
                    {
                        Session["empId"] = empDetails.EmpId;
                        return RedirectToAction("Index", "Employees");
                    }

             

                int caseSwitch = 1;

                switch(caseSwitch)
                {
                    case 1:
                        roledetails.RoleId = 1;
                        Session["empId"] = empDetails.EmpId;
                        RedirectToAction("Index", "Employees");
                        break;
                    case 2:
                        roledetails.RoleId = 2;
                        Session["empId"] = empDetails.EmpId;
                       return RedirectToAction("Index", "Supervisor");
                        break;
                    case 3:
                        roledetails.RoleId = 3;
                        Session["empId"] = empDetails.EmpId;
                        RedirectToAction("Index", "HR");
                        break;
                }

            }
            return View("Index",auth);
        }


        public ActionResult LogOut()
        {
           Session.Abandon();
            return RedirectToAction("Index","Login");
        }
   
        

        
    }
}