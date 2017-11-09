using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timesheet.Models
{
    public class PaySummary
    {
        //Instance Variables
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();

        //class parameters
        public string EmpName { get; set; }
        public string SuperName { get; set; }
        public string TotalHours { get; set; }
        public string OverTimeHours { get; set; }
        public string TimeSheetStatus { get; set; }

        //constructors
        //no-arg constructor
        public PaySummary()
        {
            this.EmpName = "";
            this.SuperName = "";
            this.TotalHours = "";
            this.OverTimeHours = "";
            this.TimeSheetStatus = "";
        }

        //all-arg constructor
        public PaySummary(string empName, string superName, string totalHrs, string overHrs, string status)
        {
            this.EmpName = empName;
            this.SuperName = superName;
            this.TotalHours = totalHrs;
            this.OverTimeHours = overHrs;
            this.TimeSheetStatus = status;
        }

        //Method to return pay summary data for employees
        public List<int> GetEmpIdsByWeekEndDate(string date)
        {
            //get set of employee ids by week ending date
            var eIds = (from sheets in db.TimeSheets
                        where sheets.WeekEnding == date
                        select sheets.EmpId).Distinct().OrderBy(EmpId => EmpId);

            List<int> empIds = new List<int>();
            //add employee ids to list
            foreach(int id in eIds)
            {
                empIds.Add(id);
            }

            return empIds;
        }

    }
}