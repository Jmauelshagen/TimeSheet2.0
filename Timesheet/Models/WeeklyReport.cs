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
    using System.Diagnostics;
    using System.Linq;
    using System.Web;

    public partial class WeeklyReport
    {
        /**Instance Variables**/
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();

        /**Properties**/
        public int Id { get; set; }
        public string WeekEnding { get; set; }
        public int Banner_ID { get; set; }
        public string LeaveHours { get; set; }
        public string AdditionalHours { get; set; }
        public string HoursWorked { get; set; }
        public string TotalHoursWorked { get; set; }
        public string Overtime { get; set; }
        public string FLSA_Overtime { get; set; }
        public string SupervisorName { get; set; }
        public string TimesheetStatus { get; set; }
        public string EmployeeName { get; set; }
        public string type { get; set; }

        /**Default constructor**/
        public WeeklyReport()
        {
            Id = 0;
            WeekEnding = "";
            Banner_ID = 0;
            LeaveHours = "";
            AdditionalHours = "";
            HoursWorked = "";
            TotalHoursWorked = "";
            Overtime = "";
            FLSA_Overtime = "";
            SupervisorName = "";
            TimesheetStatus = "";
            EmployeeName = "";
        }

        /**No args Constructor**/
        public WeeklyReport(int id, string we, int bid, string lh, string ah, string hw, string thw, string ot, string fot, string sup, string ts, string en)
        {
            Id = id;
            WeekEnding = we;
            Banner_ID = bid;
            LeaveHours = lh;
            AdditionalHours = ah;
            HoursWorked = hw;
            TotalHoursWorked = thw;
            Overtime = ot;
            FLSA_Overtime = fot;
            SupervisorName = sup;
            TimesheetStatus = ts;
            EmployeeName = en;
        }

        /**Queries the Weekly Report Table for all timesheets for a specific employee then creates
         * a list of weekly timesheets with disticted weekend dates**/
        public WeeklyReport getWeeklyReport(int Banner_ID, string wEnd)
        {
            var wReport = (from wr in db.WeeklyReports
                               where wr.Banner_ID == Banner_ID && wr.WeekEnding == wEnd.Trim()
                               select wr);

            var count = wReport.Count();
            Debug.WriteLine("(((((( THE COUNT IS: )))) " + count);
            if (count == 0)
            {
                WeeklyReport report = new WeeklyReport
                {
                    Id = this.GetMaxWeeklyReportId() + 1,
                    WeekEnding = wEnd.Trim(),
                    Banner_ID = Banner_ID,
                    LeaveHours = "",
                    AdditionalHours = "",
                    HoursWorked = "",
                    TotalHoursWorked = "",
                    Overtime = "",
                    FLSA_Overtime = "",
                    SupervisorName = "",
                    TimesheetStatus = "",
                    EmployeeName = "",
                };
            
                this.InsertWeeklyReport(report);
                report.CalculateWeeklyReport(Banner_ID, wEnd.Trim());
                report.UpdateWeeklyReport(report);
                return report;
            }
            else
            {
                WeeklyReport report = new WeeklyReport();
                report = wReport.First();
                Debug.WriteLine("Report_ID: " + report.Id + " WeekEnding: " + report.WeekEnding + " Banner_ID: " + report.Banner_ID + " leaveHours: " + report.LeaveHours + " AdditionalHours: " + report.AdditionalHours + " HoursWorked: " + report.HoursWorked + " TotalHours: " + report.TotalHoursWorked + " Overtime: " + report.Overtime + " FLSA: " + report.FLSA_Overtime + " Supervisor: " + report.SupervisorName + " Status: " + report.TimesheetStatus + " Employee: " + report.EmployeeName + " poiuy");
                report.CalculateWeeklyReport(Banner_ID, wEnd.Trim());
                report.UpdateWeeklyReport(report);
                return report;
            }

        }

        /**Queries the Weekly Report Table for all timesheets for a specific employee then creates
         * a list of weekly timesheets with total calculations for each distinct weekend date**/
        public void CalculateWeeklyReport(int Banner_ID, string wEnd)
        {
            //Code to calculate total hours worked in a week and time sheet status
            string status = "Unknown";
            string totalWorked = "";
            string totalabsent = "0";
            string totalAdditional = "0";
            string totalHours = "";
            string totalweekly = "";
            string overTime = "0";
            int noTime = 0;
            int missedpunch = 0;
            int Error = 0;
            int hours = 0;
            int minutes = 0;
            var tsheets = (from sheets in db.TimeSheets
                           where sheets.Banner_ID == Banner_ID && sheets.WeekEnding == wEnd
                           select sheets);

            /**Calculates the total hours for the week**/
            noTime = 0;
            missedpunch = 0;
            Error = 0;
            hours = 0;
            minutes = 0;
            foreach (TimeSheet sheet in tsheets)
            {
                string hoursWorked = sheet.CalculateTotalHoursWorked(sheet);
                Debug.WriteLine("Total hours calc is: " + hoursWorked);
                if (!String.IsNullOrEmpty(hoursWorked.Trim()) && !hoursWorked.Equals("Error") && !hoursWorked.Equals("Missing Punch") && !hoursWorked.Equals("NoTime") && !hoursWorked.Equals("0"))
                {
                    hours += Convert.ToInt16(hoursWorked.Split(':')[0]);
                    minutes += Convert.ToInt16(hoursWorked.Split(':')[1]);
                    Debug.WriteLine("Here is the minutes : " + minutes);
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
                    totalweekly = totalHours;

                    Debug.WriteLine("Running total: " + totalHours);
                    Debug.WriteLine("Running total by the hours: " + hours);
                }
                else if (hoursWorked.Equals("Missing Punch"))
                {
                    missedpunch = missedpunch + 1;
                }
                else if (hoursWorked.Equals("Error"))
                {
                    Error = Error + 1;
                }
                else
                {
                    noTime = noTime + 1;
                }
            }

            if (Error >= 1)
            {
                totalweekly = "Error";
            }
            else if (missedpunch >= 1)
            {
                totalweekly = "Missed Punch";
            }
            else if (noTime == 7)
            {
                totalweekly = "NoTime";
            }
            else
            {
                totalweekly = totalHours;
            }
            this.TotalHoursWorked = totalweekly;

            //Calculates the total hours worked
            noTime = 0;
            missedpunch = 0;
            Error = 0;
            hours = 0;
            minutes = 0;
            foreach (TimeSheet sheet in tsheets)
            {
                string hoursWorked = sheet.CalculateWorkedHours(sheet);
                if (!String.IsNullOrEmpty(hoursWorked.Trim()) && !hoursWorked.Equals("Error") && !hoursWorked.Equals("Missing Punch") && !hoursWorked.Equals("NoTime") && !hoursWorked.Equals("0"))
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
                else if (hoursWorked.Equals("Missing Punch"))
                {
                    missedpunch = missedpunch + 1;
                }
                else if (hoursWorked.Equals("Error"))
                {
                    Error = Error + 1;
                }
                else
                {
                    noTime = noTime + 1;
                }

                Debug.WriteLine("Submit status:" + sheet.Submitted);
                Debug.WriteLine("Autorized status: " + sheet.AuthorizedBySupervisor);
                if (sheet.Submitted.Trim().Equals("True") && sheet.AuthorizedBySupervisor.Trim().Equals("True"))
                {
                    Debug.WriteLine("In Authorized");
                    status = "Approved";
                }
                else if (sheet.Submitted.Trim().Equals("True") && sheet.AuthorizedBySupervisor.Trim().Equals("False"))
                {
                    Debug.WriteLine("In submitted");
                    status = "Pending Approval";
                }
                else
                {
                    status = "Not Submitted";
                }
            }

            if (Error >= 1)
            {
                totalWorked = "Error";
            }
            else if (missedpunch >= 1)
            {
                totalWorked = "Missed Punch";
            }
            else if (noTime == 7)
            {
                totalWorked = "NoTime";
            }
            else
            {
                totalWorked = totalHours;
            }
            this.TimesheetStatus = status;
            this.HoursWorked = totalWorked;

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
            this.FLSA_Overtime = overTime;

            //Calculate overtime hours
            if (hours >= 40)
            {
                overTime = (hours - 40).ToString() + ":" + minutes;
            }
            this.Overtime = overTime;

            //Calculate Total Absent Hours
            noTime = 0;
            missedpunch = 0;
            Error = 0;
            hours = 0;
            minutes = 0;
            foreach (TimeSheet sheet in tsheets)
            {
                string absHours = "0";
                if (String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                {

                }
                else if (!String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                {
                    absHours = sheet.LeaveHours;
                    Debug.WriteLine("Leave hour : " + sheet.LeaveHours);
                    hours += Convert.ToInt16(absHours.Split(':')[0]);
                    minutes += Convert.ToInt16(absHours.Split(':')[1]);
                    while (minutes >= 60)
                    {
                        minutes = minutes - 60;
                        hours = hours + 1;
                    }

                    totalHours = hours + ":" + minutes;
                    totalabsent = totalHours;

                    Debug.WriteLine("Running absents: " + totalHours);
                    Debug.WriteLine("Running absents by the hours: " + hours);
                }
                else
                {
                    Error = Error + 1;
                }
            }
            this.LeaveHours = totalabsent;

            //Calculate Total Additional Hours
            noTime = 0;
            missedpunch = 0;
            Error = 0;
            hours = 0;
            minutes = 0;
            foreach (TimeSheet sheet in tsheets)
            {
                string addHours = "0";
                if (String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                {

                }
                else if (!String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                {
                    addHours = sheet.AdditionalHours;
                    Debug.WriteLine("Leave hour : " + sheet.AdditionalHours);
                    hours += Convert.ToInt16(addHours.Split(':')[0]);
                    minutes += Convert.ToInt16(addHours.Split(':')[1]);
                    while (minutes >= 60)
                    {
                        minutes = minutes - 60;
                        hours = hours + 1;
                    }

                    totalHours = hours + ":" + minutes;
                    totalAdditional = totalHours;

                    Debug.WriteLine("Running absents: " + totalHours);
                    Debug.WriteLine("Running absents by the hours: " + hours);
                }
                else
                {
                    Error = Error + 1;
                }
            }
            this.AdditionalHours = totalAdditional;

            //Code to get the employee name and Supervisor name by employee id
            var fname = (from emps in db.Employees
                         where emps.Banner_ID == Banner_ID
                         select emps.First_Name).FirstOrDefault();
            var lname = (from emps in db.Employees
                         where emps.Banner_ID == Banner_ID
                         select emps.Last_Name).FirstOrDefault();
            this.EmployeeName = fname.Trim() + " " + lname.Trim();
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
            this.SupervisorName = sfname.Trim() + " " + slname.Trim();
        }

        /**Inserts a created weekly report into the weekly report table**/
        public void InsertWeeklyReport(WeeklyReport report)
        {
            db.WeeklyReports.Add(report);
            db.SaveChanges();
        }

        /**Updates a specifc Weekly Report with all recieved information**/
        public void UpdateWeeklyReport(WeeklyReport report)
        {
            WeeklyReport wReport = (from wr in db.WeeklyReports
                                where wr.Id == report.Id
                                select wr).Single();
            Debug.WriteLine("Report_ID: " + report.Id + " WeekEnding: " + report.WeekEnding + " Banner_ID: " + report.Banner_ID + " leaveHours: " + report.LeaveHours + " AdditionalHours: " + report.AdditionalHours + " HoursWorked: " + report.HoursWorked + " TotalHours: " + report.TotalHoursWorked + " Overtime: " + report.Overtime + " FLSA: " + report.FLSA_Overtime + " Supervisor: " + report.SupervisorName + " Status: " + report.TimesheetStatus + " Employee: " + report.EmployeeName + " QWERTY");
            wReport.Id = report.Id;
            wReport.WeekEnding = report.WeekEnding.Trim();
            wReport.Banner_ID = report.Banner_ID;
            wReport.LeaveHours = report.LeaveHours;
            wReport.AdditionalHours = report.AdditionalHours;
            wReport.HoursWorked = report.HoursWorked;
            wReport.TotalHoursWorked = report.TotalHoursWorked;
            wReport.Overtime = report.Overtime;
            wReport.FLSA_Overtime = report.FLSA_Overtime;
            wReport.SupervisorName = report.SupervisorName;
            wReport.TimesheetStatus = report.TimesheetStatus;
            wReport.EmployeeName = report.EmployeeName;

            db.SaveChanges();
        }

        /**Method to get the max id from the TimeSheet data table**/
        public int GetMaxWeeklyReportId()
        {
            var ids = from wReports in db.WeeklyReports
                      orderby wReports.Id descending
                      select wReports.Id;
            int maxId = ids.FirstOrDefault();
            return maxId;
        }

        /**Method to return pay summary data for employees**/
        public List<int> GetBanner_IDsByWeekEndDate(string date)
        {
            //get set of employee ids by week ending date
            var eIds = (from sheets in db.TimeSheets
                        where sheets.WeekEnding == date
                        select sheets.Banner_ID).Distinct().OrderBy(Banner_ID => Banner_ID);

            List<int> Banner_IDs = new List<int>();
            //add employee ids to list
            foreach (int id in eIds)
            {
                Banner_IDs.Add(id);
            }

            return Banner_IDs;
        }
    }
}
