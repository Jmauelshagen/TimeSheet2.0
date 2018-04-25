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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Email
    {
        /**Instance Variables**/
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();

        public string Password { get; set; }
        public string Email_Address { get; set; }

        /**Default Constructor**/
        public Email()
        {
            Email_Address = "";
            Password = "";
        }

        /** All args Constructor**/
        public Email(string email, string pass)
        {
            Email_Address = email;
            Password = pass;
        }

        /**Method that returns a email given a string**/
        public Email GetEmail(string em)
        {
            var e = from emails in db.Emails
                    where emails.Email_Address == em
                    select emails;
            Email email = (Email)e.FirstOrDefault();
            return email;
        }
    }
}
