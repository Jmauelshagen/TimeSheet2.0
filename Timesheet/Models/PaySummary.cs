﻿using System;
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
            string totalHours = "";
            int hour = 0;
            int minute = 0;
            var tsheets = (from sheets in db.TimeSheets
                           where sheets.EmpId == empId && sheets.WeekEnding == wED
                           select sheets);

            foreach(TimeSheet sheet in tsheets)
            {

                string hoursWorked = sheet.CalculateTotalHoursWorked(sheet);
                if (hoursWorked.Equals("NoTime"))
                {

                }
                else if (!String.IsNullOrEmpty(hoursWorked) && !hoursWorked.Equals("Error"))
                {
                    hour += Convert.ToInt16(hoursWorked.Split(':')[0]);
                    minute += Convert.ToInt16(hoursWorked.Split(':')[1]);
                    if (minute >= 60)
                    {
                        minute = minute - 60;
                        hour = hour + 1;
                    }
                    totalHours = hour + ":" + minute;
                    Debug.WriteLine("Running total: " + totalHours);
                    Debug.WriteLine("Running total by the hours: " + hour);
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
            if(hour >= 40)
            {
                overTime = (hour - 40).ToString() +":"+ minute;
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