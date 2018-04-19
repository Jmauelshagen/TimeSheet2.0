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
        public int Banner_ID { get; set; }
        public string SuperName { get; set; }
        public string TotalHours { get; set; }
        public string TotalAbsent { get; set; }
        public string OverTimeHours { get; set; }
        public string OverTimeHoursFLSA { get; set; }
        public string TimeSheetStatus { get; set; }

        //constructors
        //no-arg constructor
        public PaySummary()
        {
            this.EmpName = "";
            this.Banner_ID = 0;
            this.SuperName = "";
            this.TotalHours = "";
            this.TotalAbsent = "";
            this.OverTimeHours = "";
            this.OverTimeHoursFLSA = "";
            this.TimeSheetStatus = "";
        }

        //all-arg constructor
        public PaySummary(string empName, int Banner_ID, string superName, string totalHrs, string totalabs, string overHrs, string overFLSA, string status)
        {
            this.EmpName = empName;
            this.Banner_ID = Banner_ID;
            this.SuperName = superName;
            this.TotalHours = totalHrs;
            this.TotalAbsent = totalabs;
            this.OverTimeHours = overHrs;
            this.OverTimeHoursFLSA = overFLSA;
            this.TimeSheetStatus = status;
        }

        //constructor by Employee Id and WeekEnding date
        public PaySummary(int Banner_ID, string wED)
        {
            //Code to calculate total hours worked in a week and time sheet status
            string status = "Unknown";
            string totalWorked = "";
            string totalabsent = "";
            string totalHours = "";
            string overTime = "0";
            int missedpunch = 0;
            int Error = 0;
            int hour = 0;
            int hours = 0;
            int minute = 0;
            int minutes = 0;
            var tsheets = (from sheets in db.TimeSheets
                           where sheets.Banner_ID == Banner_ID && sheets.WeekEnding == wED
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
                else if (!String.IsNullOrEmpty(hoursWorked.Trim()) && !hoursWorked.Equals("Error"))
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
                Debug.WriteLine("Submit status:" + sheet.Submitted);
                Debug.WriteLine("Autorized status: " + sheet.AuthorizedBySupervisor);
                if (sheet.Submitted.Trim().Equals("True") && sheet.AuthorizedBySupervisor.Trim().Equals("True"))
                {
                    Debug.WriteLine("In Authorized");
                    status = "Approved";
                }
                else if(sheet.Submitted.Trim().Equals("True") && sheet.AuthorizedBySupervisor.Trim().Equals("False"))
                {
                    Debug.WriteLine("In submitted");
                    status = "Pending Approval";
                }
                else
                {
                    status = "Not Submitted";
                }
            }
            this.TimeSheetStatus = status;
            this.TotalHours = totalHours;

            //Calculate Total Absent Hours
            foreach (TimeSheet sheet in tsheets)
            {
                string absHours = "";
                if (sheet.LeaveHours.Equals("0:00"))
                {

                }
                else if (!String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                {
                    absHours = sheet.LeaveHours;
                    Debug.WriteLine("Leave hour : " + sheet.LeaveHours);
                    hour += Convert.ToInt16(absHours.Split(':')[0]);
                    minute += Convert.ToInt16(absHours.Split(':')[1]);
                    while (minute >= 60)
                    {
                        minute = minute - 60;
                        hour = hour + 1;
                    }

                    totalHours = hour + ":" + minute;
                    totalabsent = totalHours;

                    Debug.WriteLine("Running absents: " + totalHours);
                    Debug.WriteLine("Running absents by the hours: " + hours);
                }
                else
                {
                    Error = Error + 1;
                }
            }
            this.TotalAbsent = totalabsent;

            /**Calculates total overtime that was made**/
            if (hours >= 40)
            {
                overTime = (hours - 40).ToString() + ":" + minutes;
                int overtimeHours = 0;
                int overtimeMinutes = ((hours - 40) * 60 + minutes) + (((hours - 40) * 60 + minutes) / 2);
                while (overtimeMinutes >= 60)
                {
                    overtimeHours = overtimeHours + 1;
                    overtimeMinutes = overtimeMinutes - 60;
                }
                if (overtimeMinutes >= 53)
                {
                    overTime = (overtimeHours + 1) + ":00";
                }
                if (overtimeMinutes >= 38 && overtimeMinutes <= 52)
                {
                    overTime = overtimeHours + ":45";
                }
                if (overtimeMinutes >= 23 && overtimeMinutes <= 37)
                {
                    overTime = overtimeHours + ":30";
                }
                if (overtimeMinutes <= 22 && overtimeMinutes >= 8)
                {
                    overTime = overtimeHours + ":15";
                }
                if (overtimeMinutes <= 7)
                {
                    overTime = overtimeHours + ":00";

                }
            }
            this.OverTimeHoursFLSA = overTime;

            //Calculate overtime hours
            if(hours >= 40)
            {
                overTime = (hours - 40).ToString() +":"+ minutes;
            }
            this.OverTimeHours = overTime;

            //Code to get the employee name and Supervisor name by employee id
            var fname = (from emps in db.Employees
                         where emps.Banner_ID == Banner_ID
                         select emps.First_Name).FirstOrDefault();
            var lname = (from emps in db.Employees
                         where emps.Banner_ID == Banner_ID
                         select emps.Last_Name).FirstOrDefault();
            this.EmpName = fname + " " + lname;
            this.Banner_ID = Banner_ID;

            var sId = (from emps in db.Employees
                       where emps.Banner_ID == Banner_ID
                       select emps.Supervisor).FirstOrDefault();
            int sIdn = Convert.ToInt16(sId.Trim().ToString());
            var sfname = (from emps in db.Employees
                          where emps.Banner_ID == sIdn
                          select emps.First_Name).FirstOrDefault();
            var slname = (from emps in db.Employees
                          where emps.Banner_ID == sIdn
                          select emps.Last_Name).FirstOrDefault();
            this.SuperName = sfname + " " + slname;

        }

        public PaySummary(int empId, string wED, string t)
        {
            //Code to calculate total hours worked in a week and time sheet status
            string status = "Unknown";
            string totalWorked = "";
            string totalabsent = "";
            string totalHours = "";
            int missedpunch = 0;
            int Error = 0;
            int hour = 0;
            int hours = 0;
            int minute = 0;
            int minutes = 0;
            var tsheets = (from sheets in db.TimeSheets
                           where sheets.Banner_ID == empId && sheets.WeekEnding == wED && sheets.AuthorizedBySupervisor.Equals(t)
                           select sheets);

            foreach (TimeSheet sheet in tsheets)
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
                Debug.WriteLine("Submit status:" + sheet.Submitted);
                Debug.WriteLine("Autorized status: " + sheet.AuthorizedBySupervisor);
                if (sheet.Submitted.Trim().Equals("True") && sheet.AuthorizedBySupervisor.Trim().Equals("True"))
                {
                    Debug.WriteLine("In Authorized");
                    status = "Authorized";
                }
                else if (sheet.Submitted.Trim().Equals("True") && sheet.AuthorizedBySupervisor.Trim().Equals("False"))
                {
                    Debug.WriteLine("In submitted");
                    status = "Submitted";
                }
                else
                {
                    status = "Not Submitted";
                }
            }
            this.TimeSheetStatus = status;
            this.TotalHours = totalHours.ToString();

            //Calculate Total Absent Hours
            foreach (TimeSheet sheet in tsheets)
            {
                string absHours = "";
                if (sheet.LeaveHours.Equals("0:00"))
                {

                }
                else if (!String.IsNullOrEmpty(sheet.LeaveHours))
                {
                    absHours = sheet.LeaveHours;
                    Debug.WriteLine("Leave hour : " + sheet.LeaveHours);
                    hour += Convert.ToInt16(absHours.Split(':')[0]);
                    minute += Convert.ToInt16(absHours.Split(':')[1]);
                    while (minute >= 60)
                    {
                        minute = minute - 60;
                        hour = hour + 1;
                    }

                    totalHours = hour + ":" + minute;
                    totalabsent = totalHours;

                    Debug.WriteLine("Running absents: " + totalHours);
                    Debug.WriteLine("Running absents by the hours: " + hours);
                }
                else
                {
                    Error = Error + 1;
                }
            }
            this.TotalAbsent = totalabsent;

            //Calculate overtime hours
            string overTime = "";
            if (hours >= 40)
            {
                overTime = (hours - 40).ToString() + ":" + minutes;
            }
            this.OverTimeHours = overTime;

            //Code to get the employee name and Supervisor name by employee id
            var fname = (from emps in db.Employees
                         where emps.Banner_ID == empId
                         select emps.First_Name).FirstOrDefault();
            var lname = (from emps in db.Employees
                         where emps.Banner_ID == empId
                         select emps.Last_Name).FirstOrDefault();
            this.EmpName = fname + " " + lname;
            this.Banner_ID = empId;

            var sId = (from emps in db.Employees
                       where emps.Banner_ID == empId
                       select emps.Supervisor).FirstOrDefault();
            int sIdn = Convert.ToInt16(sId.Trim().ToString());
            var sfname = (from emps in db.Employees
                          where emps.Banner_ID == sIdn
                          select emps.First_Name).FirstOrDefault();
            var slname = (from emps in db.Employees
                          where emps.Banner_ID == sIdn
                          select emps.Last_Name).FirstOrDefault();
            this.SuperName = sfname + " " + slname;

        }        

        //Method to return pay summary data for employees
        public List<int> GetBanner_IDsByWeekEndDate(string date)
        {
            //get set of employee ids by week ending date
            var eIds = (from sheets in db.TimeSheets
                        where sheets.WeekEnding == date
                        select sheets.Banner_ID).Distinct().OrderBy(Banner_ID => Banner_ID);

            List<int> Banner_IDs = new List<int>();
            //add employee ids to list
            foreach(int id in eIds)
            {
                Banner_IDs.Add(id);
            }

            return Banner_IDs;
        }
    }
}