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
                //Query for employee id that matches the username and password submitted from the login form
                //Then grab it from the empId container
                var empId = from login in db.Logins
                            where login.Username == model.Username && login.Password == model.Password
                            select login.EmpId;
                var employeeId = empId.FirstOrDefault();

                //Logic to verify that the login information given matches data in the Logins database
                //If no match is found, the empId will be zero (0)
                if (empId.FirstOrDefault() == 0)
                {
                    string error = "Invalid user.";
                    Session["Error"] = error;
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    //If a match is found, query for the matching record on the Employee table
                    //and instantiate an Employee object and add it to the session
                    IEnumerable<Employee> emp = from employee in db.Employees
                                                where employee.EmpId == employeeId
                                                select employee;
                    Session["Employee"] = emp.FirstOrDefault();

                    //Get the role id from the Employee object
                    int role = emp.FirstOrDefault().RoleId;

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
            //default return statement
            return RedirectToAction("Index", "Login");
        }

        //method for handling the logout link
        public ActionResult LogOut()
        {
           Session.Abandon();
            return RedirectToAction("Index","Login");
        }
   
        

        
    }
}