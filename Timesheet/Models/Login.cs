//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Timesheet.Models
{
    //another test
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class Login
    {

        //final test
        //Instance variables
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();

        //Class properties
        public int Banner_ID { get; set; }
        [DisplayName("User Name")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "UserName must be between 4 and 20 characters")]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 30 characters")]
        public string Password { get; set; }
        public string LoginErrorMessage { get; set; }

        //Constructors
        //no-arg constructor
        public Login()
        {
            Banner_ID = 0;
            Username = "";
            Password = "";
            LoginErrorMessage = "";
        }

        //all-args constructor
        public Login(int id, string uName, string pWord, string error)
        {
            Banner_ID = id;
            Username = uName;
            Password = pWord;
            LoginErrorMessage = error;
        }

        //Method to validate login information
        //Queries Login table by username and password, returns bool if record is found
        public bool ValidateLogin(string uname, string pword)
        {
            var log = from logins in db.Logins
                      where logins.Username == uname && logins.Password == pword
                      select logins;
            Login login = (Login)log.FirstOrDefault();



            if (login == null)///Fixed and error when logging in, it used to fail when you logged in wrong.

            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Method to get Login object
        //Queries Login table by username and password, returns Login object
        public Login GetLogin(string uname, string pword)
        {
            var log = from logins in db.Logins
                      where logins.Username == uname && logins.Password == pword
                      select logins;
            Login login = (Login)log.FirstOrDefault();
            return login;
        }
    }
}
