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
            //Logic to verify that the login information given matches data in the Logins database
            //Instantiate an empty login object, then call the ValidateLogin method with the username and
            //password information from the UI.  If the method returns false, then the user is redirected to the login
            //screen and an error message is displayed.
            Login log = new Login();
            if (!log.ValidateLogin(model.Username, model.Password))
            {
                string error = "Invalid user.";
                Session["Error"] = error;
                return RedirectToAction("Index", "Login");
            }
            else
            //Instantiate a login object based on the username and password.  Get the employee id from the login object,
            //and create an employee object and put that object into the session
            {
                Login login = log.GetLogin(model.Username, model.Password);
                Employee emp = new Employee();
                Employee employee = emp.GetEmployee(login.EmpId);
                Session["Employee"] = employee;

                //Get the role id from the Employee object
                int role = employee.RoleId;

                //Redirect the user to the correct dashboard screen based upon the role id
                // 1 = Employee; 2 = Supervisor; 3 = HR
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

        //method for handling the logout link
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }




    }
}