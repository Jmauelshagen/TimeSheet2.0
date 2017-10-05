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
        public ActionResult Authorize(Login model)
        {
            using (LoginDatabaseEntities1 db = new LoginDatabaseEntities1())
            {

                var empId = from login in db.Logins
                            where login.Username == model.Username && login.Password == model.Password
                            select login.EmpId;
                var employeeId = empId.FirstOrDefault();


                if (empId.FirstOrDefault() == 0)
                {
                    model.LoginErrorMessage = "Invalid user.";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    IEnumerable<Employee> emp = from employee in db.Employees
                                                where employee.EmpId == employeeId
                                                select employee;
                    Session["Employee"] = emp.FirstOrDefault();

                    int role = emp.FirstOrDefault().RoleId;

                    switch (role)
                    {
                        case 1:
                            {
                                return RedirectToAction("Index", "Employees");
                                break;
                            }
                        case 2:
                            {
                                return RedirectToAction("Index", "Supervisor");
                                break;
                            }
                        case 3:
                            {
                                return RedirectToAction("Index", "HR");
                                break;
                            }
                        default:
                            {
                                model.LoginErrorMessage = "An error has occurred.";
                                return RedirectToAction("Index", "Login");
                                break;
                            }
                    }
                            
                        
                }


            }
            return RedirectToAction("Index", "Login");
        }


        public ActionResult LogOut()
        {
           Session.Abandon();
            return RedirectToAction("Index","Login");
        }
   
        

        
    }
}