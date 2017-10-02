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
        public ActionResult Authorize(Emp empModel, Sup supModel, HR hrModel)
        {
            using (LoginDatabaseEntities db = new LoginDatabaseEntities())
            {
                var empDetails = db.Emps.Where(x => x.UserName == empModel.UserName && x.Password == empModel.Password)
                    .FirstOrDefault();
                var supDetails = db.Sups.Where(x => x.UserName == supModel.UserName && x.Password == supModel.Password)
                    .FirstOrDefault();
                var hrDetails = db.HRs.Where(x => x.UserName == hrModel.UserName && x.Password == hrModel.Password)
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

                    if (supDetails == null)
                    {
                        supModel.LoginErrorMessage = "wrong UserName or Password";
                    }
                    else
                    {
                        Session["supId"] = supDetails.SupId;
                        return RedirectToAction("Index", "Supervisor");
                    }

                    if (hrDetails == null)
                    {
                        hrModel.LoginErrorMessage = "wrong UserName or Password";
                    }
                    else
                    {
                        Session["hrId"] = hrDetails.HRId;
                        return RedirectToAction("Index", "HR");
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