//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Timesheet.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Emp
    {
        public int EmpId { get; set; }
        [DisplayName("User Name")]
        [Required(ErrorMessage = "This Field is required")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This Field is required")]
        public string Password { get; set; }

        public String LoginErrorMessage { get; set; }
    }
}
