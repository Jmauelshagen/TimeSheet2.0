using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        //constructor by Employee Id and WeekEnding date
        public PaySummary(int empId, string wED)
        {
            //Code to calculate total hours worked in a week and time sheet status
            string status = "Unknown";
            string totalWorked = "";
            string totalHours = "";
            int missedpunch = 0;
            int Error = 0;
            int hours = 0;
            int minutes = 0;
            var tsheets = (from sheets in db.TimeSheets
                           where sheets.EmpId == empId && sheets.WeekEnding == wED
                           select sheets);

            foreach(TimeSheet sheet in tsheets)
            {
                string hoursWorked = sheet.CalculateWorkedHours(sheet);
                if (hoursWorked.Equals("NoTime"))
                {

                }
                else if (hoursWorked.Equals("Missing Punch"))
                {
                    missedpunch = missedpunch + 1;
                }
                else if (!String.IsNullOrEmpty(hoursWorked) && !hoursWorked.Equals("Error"))
                {
                    hours += Convert.ToInt16(hoursWorked.Split(':')[0]);
                    minutes += Convert.ToInt16(hoursWorked.Split(':')[1]);
                    while (minutes >= 60)
                    {
                        minutes = minutes - 60;
                        hours = hours + 1;
                    }
                    if (minutes == 0)
                    {
                        totalHours = hours + ":00";
                    }
                    else
                    {
                        totalHours = hours + ":" + minutes;
                    }
                    totalWorked = totalHours;

                    Debug.WriteLine("Running Worked total: " + totalWorked);
                    Debug.WriteLine("Running Worked total by the hours: " + hours);
                }
                else
                {
                    Error = Error + 1;
                }

                if (sheet.Submitted.Equals("True") && sheet.AuthorizedBySupervisor.Equals("True"))
                {
                    status = "Authorized";
                }
                else if(sheet.Submitted.Equals("True") && sheet.AuthorizedBySupervisor.Equals("False"))
                {
                    status = "Submitted";
                }
                else
                {
                    status = "Not Submitted";
                }
            }
            this.TimeSheetStatus = status;
            this.TotalHours = totalHours.ToString();

            //Calculate overtime hours
            string overTime = "";
            if(hours >= 40)
            {
                overTime = (hours - 40).ToString() +":"+ minutes;
            }
            this.OverTimeHours = overTime;

            //Code to get the employee name and Supervisor name by employee id
            var fname = (from emps in db.Employees
                         where emps.EmpId == empId
                         select emps.FirstName).FirstOrDefault();
            var lname = (from emps in db.Employees
                         where emps.EmpId == empId
                         select emps.LastName).FirstOrDefault();
            this.EmpName = fname + " " + lname;

            var sId = (from emps in db.Employees
                       where emps.EmpId == empId
                       select emps.Supervisor).FirstOrDefault();
            var sfname = (from emps in db.Employees
                          where emps.EmpId == sId
                          select emps.FirstName).FirstOrDefault();
            var slname = (from emps in db.Employees
                          where emps.EmpId == sId
                          select emps.LastName).FirstOrDefault();
            this.SuperName = sfname + " " + slname;

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