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
    using System.Web.Mvc;

    public partial class TimeSheet
    {
        //Instance Variables
        LoginDatabaseEntities1 db = new LoginDatabaseEntities1();

        //Class properties
        public int Id { get; set; }
        public string WeekEnding { get; set; }
        public string Date { get; set; }
        public string TimeIn { get; set; }
        public string OutForLunch { get; set; }
        public string InFromLunch { get; set; }
        public string TimeOut { get; set; }
        public Nullable<int> LeaveId { get; set; }
        public string LeaveHours { get; set; }
        public string AdditionalHours { get; set; }
        public string TotalHoursWorked { get; set; }
        public string Submitted { get; set; }
        public string AuthorizedBySupervisor { get; set; }        
        public int Banner_ID { get; set; }
        public IEnumerable<SelectListItem> WeekEndingDates { get; set; }
        public IEnumerable<SelectListItem> EmpNames { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }


        //Constructors
        //no-args constructor
        public TimeSheet()
        {
            Id = 0;
            WeekEnding = "";
            Date = "";
            TimeIn = "";
            OutForLunch = "";
            InFromLunch = "";
            TimeOut = "";
            LeaveId = 0;
            LeaveHours = "";
            AdditionalHours = "";
            TotalHoursWorked = "";
            Submitted = "False";
            AuthorizedBySupervisor = "False";
            Banner_ID = 0;
            Note = "";
        }

        //all-args constructor
        public TimeSheet(int id, string wEnd, string date, string inT, string outL, string inL, string outT,
            int leaveId, string leaveHrs, string addlHrs, string tlHrs, string sub, string auth, int banner_ID, string n)
        {
            Id = id;
            WeekEnding = wEnd;
            Date = date;
            TimeIn = inT;
            OutForLunch = outL;
            InFromLunch = inL;
            TimeOut = outT;
            LeaveId = leaveId;
            LeaveHours = leaveHrs;
            AdditionalHours = addlHrs;
            TotalHoursWorked = tlHrs;
            Submitted = sub;
            AuthorizedBySupervisor = auth;
            Banner_ID = banner_ID;
            Note = n;
        }

        //Method to get list of Timesheet objects by employee id and week ending date
        //Queries the TimeSheet table by employee id and WeekEnding date and returns List collection of Timesheet objects
        public List<TimeSheet> GetTimeSheetByWeek(int Banner_ID, List<string> dates)
        {
            List<TimeSheet> timesheets = new List<TimeSheet>();
            string wEnd = dates[0].Trim();
            var sheets = from tsheets in db.TimeSheets
                         where tsheets.Banner_ID == Banner_ID && tsheets.WeekEnding == wEnd
                         orderby tsheets.Id ascending
                         select tsheets;
            var count = sheets.Count();
            Debug.WriteLine("TimeSheet count is: " + count.ToString() + "********************************************************************************************************************************");
            if (count == 0)
            {
                for (int i = 1; i < 8; i++)
                {
                    TimeSheet sheet = new TimeSheet
                    {
                        Id = this.GetMaxTimeSheetId() + 1,
                        WeekEnding = dates[0].Trim(),
                        Date = dates[i].Trim(),
                        TimeIn = "",
                        OutForLunch = "",
                        InFromLunch = "",
                        TimeOut = "",
                        LeaveId = 0,
                        LeaveHours = "",
                        AdditionalHours = "",
                        TotalHoursWorked = "",
                        Submitted = "False",
                        AuthorizedBySupervisor = "False",
                        Banner_ID = Banner_ID,
                        Note = ""
                    };
                    this.InsertTimeSheet(sheet);
                    timesheets.Add(sheet);
                }
            }
            else
            {
                foreach (TimeSheet sheet in sheets)
                {
                    timesheets.Add(sheet);
                }
            }
            return timesheets;
        }

        public List<TimeSheet> GetApprovedTimesheets(string emp)
        {
            List<TimeSheet> timesheets = new List<TimeSheet>();
            int iemp = Convert.ToInt32(emp);
            Debug.WriteLine("Iemp value is : " + iemp + " ]");
            var sheets = from tsheets in db.TimeSheets
                         where tsheets.AuthorizedBySupervisor.Equals("True") && tsheets.Banner_ID == iemp
                         group tsheets by tsheets.WeekEnding into weekgroup
                         orderby weekgroup.Key ascending
                         select weekgroup;
            foreach (var weekgroup in sheets)
            {
                Debug.WriteLine("Key is: [0]" + weekgroup.Key + "}}");
                foreach (TimeSheet sheet in weekgroup)
                {
                    
                }               
            }
            return timesheets;
        }

        //Method to retrieve list of TimeSheet objects by employee name and week ending date
        public List<TimeSheet> GetTimeSheetByNameAndDate(string name, string wED)
        {
            Debug.WriteLine("Name value is: " + name);
            List<TimeSheet> timesheets = new List<TimeSheet>();
            string[] splitNames = name.Split(' ');
            string fName = splitNames[0].Trim();
            string lName = splitNames[1].Trim();
            //Find the employee id based on the name passed in to the method
            var Banner_ID = (from emps in db.Employees
                         where emps.First_Name == fName && emps.Last_Name == lName
                         select emps.Banner_ID).FirstOrDefault();
            //Select the TimeSheet objects based on the employee id and week ending date
            var sheets = from tsheets in db.TimeSheets
                         where tsheets.Banner_ID == Banner_ID && tsheets.WeekEnding == wED
                         orderby tsheets.Id ascending
                         select tsheets;

            foreach (TimeSheet sheet in sheets)
            {
                timesheets.Add(sheet);
            }

            return timesheets;
        }

        //Method to retrieve list of TimeSheet objects by employee ID and week ending date
        public List<TimeSheet> GetTimeSheetByIdAndDate(int id, string wED)
        {
            Debug.WriteLine("ID value is: " + id);
            List<TimeSheet> timesheets = new List<TimeSheet>();
            //Select the TimeSheet objects based on the employee id and week ending date
            var sheets = from tsheets in db.TimeSheets
                         where tsheets.Banner_ID == id && tsheets.WeekEnding == wED
                         orderby tsheets.Id ascending
                         select tsheets;

            foreach (TimeSheet sheet in sheets)
            {
                timesheets.Add(sheet);
            }

            return timesheets;
        }

        //Method to get the max id from the TimeSheet data table
        public int GetMaxTimeSheetId()
        {
            var ids = from tsheets in db.TimeSheets
                      orderby tsheets.Id descending
                      select tsheets.Id;
            int maxId = ids.FirstOrDefault();
            return maxId;
        }

        //Method to insert TimeSheet data into the TimeSheet data table
        public void InsertTimeSheet(TimeSheet sheet)
        {
            WeeklyReport weeklyReport = new WeeklyReport();

            db.TimeSheets.Add(sheet);
            db.SaveChanges();

            weeklyReport.getWeeklyReport(sheet.Banner_ID, sheet.WeekEnding.Trim());
        }

        //Method to update TimeSheet data in the TimeSheet data table
        public void UpdateTimeSheet(TimeSheet sheet)
        {
            WeeklyReport weeklyReport = new WeeklyReport();

            Debug.WriteLine("in database save 1");
            Debug.WriteLine("******************************************************************************************************** "+sheet.LeaveId);
            Debug.WriteLine("With sheet id: " + sheet.Id + "]");

            string timeIn = "";
            string outForLunch = "";
            string inFromLunch = "";
            string timeOut = "";

            if (!String.IsNullOrEmpty(sheet.TimeIn.Trim()))
            {
                timeIn = sheet.TimeIn;
            }
            else { timeIn = ""; }
            if (!String.IsNullOrEmpty(sheet.OutForLunch.Trim()))
            {
                outForLunch = sheet.OutForLunch;
            }
            else { outForLunch = ""; }
            if (!String.IsNullOrEmpty(sheet.InFromLunch.Trim()))
            {
                inFromLunch = sheet.InFromLunch;
            }
            else { inFromLunch = ""; }
            if (!String.IsNullOrEmpty(sheet.TimeOut.Trim()))
            {
                timeOut = sheet.TimeOut;
            }
            else { timeOut = ""; }

            TimeSheet tsheet = (from tsheets in db.TimeSheets
                                where tsheets.Id == sheet.Id
                                select tsheets).Single();
            Debug.WriteLine("The sheet is: " + sheet.Note + "]");
            tsheet.Id = sheet.Id;
            tsheet.WeekEnding = sheet.WeekEnding;
            tsheet.Date = sheet.Date;
            tsheet.TimeIn = timeIn;
            tsheet.OutForLunch = outForLunch;
            tsheet.InFromLunch = inFromLunch;
            tsheet.TimeOut = timeOut;
            tsheet.LeaveId = sheet.LeaveId;
            tsheet.LeaveHours = sheet.LeaveHours;
            tsheet.AdditionalHours = sheet.AdditionalHours;
            tsheet.TotalHoursWorked = tsheet.CalculateTotalHoursWorked(sheet);
            tsheet.Submitted = sheet.Submitted;
            tsheet.AuthorizedBySupervisor = sheet.AuthorizedBySupervisor;
            tsheet.Banner_ID = sheet.Banner_ID;
            tsheet.Note = sheet.Note;
            Debug.WriteLine("The tsheet is :" + tsheet.Note + "]");

            db.SaveChanges();

            weeklyReport.getWeeklyReport(sheet.Banner_ID, sheet.WeekEnding.Trim());

        }

        /**Method to calculate total hours worked daily by taking the time in/out
         * and the lunch time out and returned from lunch then rounds to nearest 15th minute mark
         * **/
        public string CalculateTotalHoursWorked(TimeSheet sheet)
        {
            try
            {
                if (!String.IsNullOrEmpty(sheet.TimeIn.Trim()) && !String.IsNullOrEmpty(sheet.OutForLunch.Trim()) && String.IsNullOrEmpty(sheet.InFromLunch.Trim()) && String.IsNullOrEmpty(sheet.TimeOut.Trim()))
                {
                    Debug.WriteLine("Calculating the first 2 punches");
                    DateTime tIn = RoundToNearest(DateTime.Parse(sheet.TimeIn), TimeSpan.FromMinutes(15)); ;
                    DateTime lOut = RoundToNearest(DateTime.Parse(sheet.OutForLunch), TimeSpan.FromMinutes(15));
                    //used to view the incoming values
                    Debug.WriteLine("Clocked in at " + tIn + " in 2 Punches");
                    Debug.WriteLine("Clocked out for lunch at " + lOut + " in 2 Punches");
                    string totalHours;
                    if (tIn > lOut)
                    {
                        totalHours = "Error";
                    }
                    else
                    {
                        int leaveHour = 0;
                        int leaveMinute = 0;
                        int addHour = 0;
                        int addMinute = 0;

                        /*Once the verification it the LeaveHours and AdditionalHours are added we can unblock the following code!*/
                        if (!String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                        {
                            string leaveTime = sheet.LeaveHours.ToString().Trim();
                            leaveHour = Convert.ToInt16(leaveTime.Split(':')[0]);
                             leaveMinute = Convert.ToInt16(leaveTime.Split(':')[1]);
                        }

                        if (!String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                        {
                            string AdditionalHours = sheet.AdditionalHours.ToString().Trim();
                            addHour = Convert.ToInt16(AdditionalHours.Split(':')[0]);
                            addMinute = Convert.ToInt16(AdditionalHours.Split(':')[1]);
                        }

                        TimeSpan hoursWorked = lOut.Subtract(tIn);
                        int hour = Convert.ToInt16(Math.Truncate(hoursWorked.TotalHours + leaveHour + addHour)); // + leaveHour + addHour;
                        int minute = Convert.ToInt16(hoursWorked.Minutes + leaveMinute + addMinute); // + leaveMinute + addMinute;
                        totalHours = hour.ToString() + ":" + minute.ToString();
                        Debug.WriteLine(hoursWorked + "************* " + hoursWorked.TotalHours + "************************* " + hour + " ************* " + minute + " leave time " + leaveHour + " add time " + AdditionalHours);                        
                    }
                    Debug.WriteLine("TotalHours from the first 2 punches :"+ totalHours);
                    return totalHours;
                }

                else if (!String.IsNullOrEmpty(sheet.TimeIn.Trim()) && !String.IsNullOrEmpty(sheet.OutForLunch.Trim()) && !String.IsNullOrEmpty(sheet.InFromLunch.Trim()) && !String.IsNullOrEmpty(sheet.TimeOut.Trim()))
                {
                    Debug.WriteLine("Calculating all times");
                    DateTime tIn = RoundToNearest(DateTime.Parse(sheet.TimeIn), TimeSpan.FromMinutes(15)); ;
                    DateTime lOut = RoundToNearest(DateTime.Parse(sheet.OutForLunch), TimeSpan.FromMinutes(15));
                    DateTime lIn = RoundToNearest(DateTime.Parse(sheet.InFromLunch), TimeSpan.FromMinutes(15));
                    DateTime tOut = RoundToNearest(DateTime.Parse(sheet.TimeOut), TimeSpan.FromMinutes(15));
                    //used to view the incoming values
                    Debug.WriteLine("Clocked in at " + tIn + " in 4 Punches");
                    Debug.WriteLine("Clocked out for lunch at " + lOut + " in 4 Punches");
                    Debug.WriteLine("Clocked in from lunch at " + lIn + " in 4 Punches");
                    Debug.WriteLine("Clocked out at " + tOut + " in 4 Punches");
                    string totalHours;
                    if (tIn > lOut || lOut > lIn || lIn > tOut)
                    {
                        totalHours = "Error";
                    }
                    else
                    {
                        int leaveHour = 0;
                        int leaveMinute = 0;
                        int addHour = 0;
                        int addMinute = 0;

                        /*Once the verification it the LeaveHours and AdditionalHours are added we can unblock the following code!*/
                        if (!String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                        {
                            string leaveTime = sheet.LeaveHours.ToString().Trim();
                            leaveHour = Convert.ToInt16(leaveTime.Split(':')[0]);
                            leaveMinute = Convert.ToInt16(leaveTime.Split(':')[1]);
                        }

                        if (!String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                        {
                            string AdditionalHours = sheet.AdditionalHours.ToString().Trim();
                            addHour = Convert.ToInt16(AdditionalHours.Split(':')[0]);
                            addMinute = Convert.ToInt16(AdditionalHours.Split(':')[1]);
                        }

                        TimeSpan hoursWorked = tOut.Subtract(tIn).Subtract(lIn.Subtract(lOut));
                        int hour = Convert.ToInt16(Math.Truncate(hoursWorked.TotalHours + leaveHour + addHour)); // + leaveHour + addHour;
                        int minute = Convert.ToInt16(hoursWorked.Minutes + leaveMinute + addMinute); // + leaveMinute + addMinute;
                        Debug.WriteLine(hoursWorked + "************* " +hoursWorked.TotalHours + "************************* " + hour + " ************* " + minute + " leave time " + LeaveHours + " add time " + AdditionalHours);
                        totalHours = hour.ToString() + ":" + minute.ToString();
                    }
                    return totalHours;
                }

                else if (!String.IsNullOrEmpty(sheet.AdditionalHours.Trim()) && String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                {
                    Debug.WriteLine("if only additional hours are worked..");
                    string totalHours;
                    totalHours = sheet.AdditionalHours.ToString().Trim();
                    return totalHours;
                }

                else if (!String.IsNullOrEmpty(sheet.LeaveHours.Trim()) && String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                {
                    Debug.WriteLine("if only leave hours are worked..");
                    string totalHours;
                    totalHours = sheet.LeaveHours.ToString().Trim();
                    return totalHours;
                }
                else if (!String.IsNullOrEmpty(sheet.LeaveHours.Trim()) && !String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                {
                    Debug.WriteLine("if both additionalHours and LeaveHours are filled in.");
                    int leaveHour = 0;
                    int leaveMinute = 0;
                    int addHour = 0;
                    int addMinute = 0;

                    string leaveTime = sheet.LeaveHours.ToString().Trim();
                    leaveHour = Convert.ToInt16(leaveTime.Split(':')[0]);
                    leaveMinute = Convert.ToInt16(leaveTime.Split(':')[1]);
 
                    string AdditionalHours = sheet.AdditionalHours.ToString().Trim();
                    addHour = Convert.ToInt16(AdditionalHours.Split(':')[0]);
                    addMinute = Convert.ToInt16(AdditionalHours.Split(':')[1]);

                    int hour = Convert.ToInt16(leaveHour + addHour);
                    int minute = Convert.ToInt16(leaveMinute + addMinute);
                    string totalHours = hour.ToString() + ":" + minute.ToString();
                    return totalHours;
                }

                else if (String.IsNullOrEmpty(sheet.TimeIn.Trim()) && String.IsNullOrEmpty(sheet.OutForLunch.Trim()) && String.IsNullOrEmpty(sheet.InFromLunch.Trim()) && String.IsNullOrEmpty(sheet.TimeOut.Trim()) && String.IsNullOrEmpty(sheet.AdditionalHours.Trim()) && String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                {
                    Debug.WriteLine("Skipping over empty day not filled out yet.");
                    string totalHours;
                    totalHours = "NoTime";
                    return totalHours;
                }

                else
                {
                    Debug.WriteLine("Sending 'Missing Punch' hours for the day because punches are missing. only gets called for 1 punch and 3 punches 000000000000000000000000000");
                    string totalHours;
                    totalHours = "Missing Punch";
                    return totalHours;
                }

            }
            catch (ArgumentException ae)
            {
                Debug.WriteLine(ae);
                return "";
            }
        }

        public string CalculateWorkedHours(TimeSheet sheet)
        {
            try
            {
                if (!String.IsNullOrEmpty(sheet.TimeIn.Trim()) && !String.IsNullOrEmpty(sheet.OutForLunch.Trim()) && String.IsNullOrEmpty(sheet.InFromLunch.Trim()) && String.IsNullOrEmpty(sheet.TimeOut.Trim()))
                {
                    Debug.WriteLine("Calculating the first 2 punches");
                    DateTime tIn = RoundToNearest(DateTime.Parse(sheet.TimeIn), TimeSpan.FromMinutes(15)); ;
                    DateTime lOut = RoundToNearest(DateTime.Parse(sheet.OutForLunch), TimeSpan.FromMinutes(15));
                    //used to view the incoming values
                    Debug.WriteLine("Clocked in at " + tIn + " in 2 Punches");
                    Debug.WriteLine("Clocked out for lunch at " + lOut + " in 2 Punches");
                    string totalHours;
                    if (tIn > lOut)
                    {
                        totalHours = "Error";
                    }
                    else
                    {
                        int addHour = 0;
                        int addMinute = 0;

                        /*Once the verification it the LeaveHours and AdditionalHours are added we can unblock the following code!*/
                        if (!String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                        {
                            string AdditionalHours = sheet.AdditionalHours.ToString().Trim();
                            addHour = Convert.ToInt16(AdditionalHours.Split(':')[0]);
                            addMinute = Convert.ToInt16(AdditionalHours.Split(':')[1]);
                        }

                        TimeSpan hoursWorked = lOut.Subtract(tIn);
                        int hour = Convert.ToInt16(Math.Truncate(hoursWorked.TotalHours + addHour)); // + leaveHour + addHour;
                        int minute = Convert.ToInt16(hoursWorked.Minutes + addMinute); // + leaveMinute + addMinute;
                        totalHours = hour.ToString() + ":" + minute.ToString();
                        Debug.WriteLine(hoursWorked + "************* " + hoursWorked.TotalHours + "************************* " + hour + " ************* " + minute + " add time " + AdditionalHours);
                    }
                    Debug.WriteLine("TotalHours from the first 2 punches :" + totalHours);
                    return totalHours;
                }

                else if (!String.IsNullOrEmpty(sheet.TimeIn.Trim()) && !String.IsNullOrEmpty(sheet.OutForLunch.Trim()) && !String.IsNullOrEmpty(sheet.InFromLunch.Trim()) && !String.IsNullOrEmpty(sheet.TimeOut.Trim()))
                {
                    Debug.WriteLine("Calculating all times");
                    DateTime tIn = RoundToNearest(DateTime.Parse(sheet.TimeIn), TimeSpan.FromMinutes(15)); ;
                    DateTime lOut = RoundToNearest(DateTime.Parse(sheet.OutForLunch), TimeSpan.FromMinutes(15));
                    DateTime lIn = RoundToNearest(DateTime.Parse(sheet.InFromLunch), TimeSpan.FromMinutes(15));
                    DateTime tOut = RoundToNearest(DateTime.Parse(sheet.TimeOut), TimeSpan.FromMinutes(15));
                    //used to view the incoming values
                    Debug.WriteLine("Clocked in at " + tIn + " in 4 Punches");
                    Debug.WriteLine("Clocked out for lunch at " + lOut + " in 4 Punches");
                    Debug.WriteLine("Clocked in from lunch at " + lIn + " in 4 Punches");
                    Debug.WriteLine("Clocked out at " + tOut + " in 4 Punches");
                    string totalHours;
                    if (tIn > lOut || lOut > lIn || lIn > tOut)
                    {
                        totalHours = "Error";
                    }
                    else
                    {
                        int addHour = 0;
                        int addMinute = 0;

                        /*Once the verification it the LeaveHours and AdditionalHours are added we can unblock the following code!*/
                        if (!String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                        {
                            string AdditionalHours = sheet.AdditionalHours.ToString().Trim();
                            addHour = Convert.ToInt16(AdditionalHours.Split(':')[0]);
                            addMinute = Convert.ToInt16(AdditionalHours.Split(':')[1]);
                        }

                        TimeSpan hoursWorked = tOut.Subtract(tIn).Subtract(lIn.Subtract(lOut));
                        int hour = Convert.ToInt16(Math.Truncate(hoursWorked.TotalHours + addHour)); // + leaveHour + addHour;
                        int minute = Convert.ToInt16(hoursWorked.Minutes + addMinute); // + leaveMinute + addMinute;
                        Debug.WriteLine(hoursWorked + "************* " + hoursWorked.TotalHours + "************************* " + hour + " ************* " + minute + " add time " + AdditionalHours);
                        totalHours = hour.ToString() + ":" + minute.ToString();
                    }
                    return totalHours;
                }

                else if (String.IsNullOrEmpty(sheet.TimeIn.Trim()) && String.IsNullOrEmpty(sheet.OutForLunch.Trim()) && String.IsNullOrEmpty(sheet.InFromLunch.Trim()) && String.IsNullOrEmpty(sheet.TimeOut.Trim()) && String.IsNullOrEmpty(sheet.AdditionalHours.Trim()) && String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                {
                    Debug.WriteLine("Skipping over empty day not filled out yet.");
                    string totalHours;
                    totalHours = "NoTime";
                    return totalHours;
                }

                else if (!String.IsNullOrEmpty(sheet.AdditionalHours.Trim()))
                {
                    Debug.WriteLine("if only additional hours are worked..");
                    string totalHours;
                    totalHours = sheet.AdditionalHours.ToString().Trim();
                    return totalHours;
                }

                else if (!String.IsNullOrEmpty(sheet.LeaveHours.Trim()))
                {
                    Debug.WriteLine("if only additional hours are worked..");
                    string totalHours;
                    totalHours = "NoTime";
                    return totalHours;
                }

                else
                {
                    Debug.WriteLine("Sending 'Missing Punch' hours for the day because punches are missing. only gets called for 1 punch and 3 punches 999999999999");
                    string totalHours;
                    totalHours = "Missing Punch";
                    return totalHours;
                }

            }
            catch (ArgumentException ae)
            {
                Debug.WriteLine(ae);
                return "";
            }
        }

        /**This method determines the current date and then derives the dates for each day of the week **/
        public List<string> GetDates()
        {
            List<string> dates = new List<string>();
            string endOfWeek = "";
            string sunDate = "";
            string monDate = "";
            string tueDate = "";
            string wedDate = "";
            string thrDate = "";
            string friDate = "";
            string satDate = "";
            int hoy = (int)DateTime.Now.DayOfWeek;

            switch (hoy)
            {
                case 0: //Sunday
                    {
                        endOfWeek = DateTime.Now.AddDays(6).ToShortDateString();
                        dates.Add(endOfWeek);
                        sunDate = DateTime.Now.ToShortDateString();
                        dates.Add(sunDate);
                        monDate = DateTime.Now.AddDays(1).ToShortDateString();
                        dates.Add(monDate);
                        tueDate = DateTime.Now.AddDays(2).ToShortDateString();
                        dates.Add(tueDate);
                        wedDate = DateTime.Now.AddDays(3).ToShortDateString();
                        dates.Add(wedDate);
                        thrDate = DateTime.Now.AddDays(4).ToShortDateString();
                        dates.Add(thrDate);
                        friDate = DateTime.Now.AddDays(5).ToShortDateString();
                        dates.Add(friDate);
                        satDate = DateTime.Now.AddDays(6).ToShortDateString();
                        dates.Add(satDate);

                        break;
                    }

                case 1: //Monday
                    {
                        endOfWeek = DateTime.Now.AddDays(5).ToShortDateString();
                        dates.Add(endOfWeek);
                        sunDate = DateTime.Now.AddDays(-1).ToShortDateString();
                        dates.Add(sunDate);
                        monDate = DateTime.Now.ToShortDateString();
                        dates.Add(monDate);
                        tueDate = DateTime.Now.AddDays(1).ToShortDateString();
                        dates.Add(tueDate);
                        wedDate = DateTime.Now.AddDays(2).ToShortDateString();
                        dates.Add(wedDate);
                        thrDate = DateTime.Now.AddDays(3).ToShortDateString();
                        dates.Add(thrDate);
                        friDate = DateTime.Now.AddDays(4).ToShortDateString();
                        dates.Add(friDate);
                        satDate = DateTime.Now.AddDays(5).ToShortDateString();
                        dates.Add(satDate);

                        break;
                    }

                case 2: //Tuesday
                    {
                        endOfWeek = DateTime.Now.AddDays(4).ToShortDateString();
                        dates.Add(endOfWeek);
                        sunDate = DateTime.Now.AddDays(-2).ToShortDateString();
                        dates.Add(sunDate);
                        monDate = DateTime.Now.AddDays(-1).ToShortDateString();
                        dates.Add(monDate);
                        tueDate = DateTime.Now.ToShortDateString();
                        dates.Add(tueDate);
                        wedDate = DateTime.Now.AddDays(1).ToShortDateString();
                        dates.Add(wedDate);
                        thrDate = DateTime.Now.AddDays(2).ToShortDateString();
                        dates.Add(thrDate);
                        friDate = DateTime.Now.AddDays(3).ToShortDateString();
                        dates.Add(friDate);
                        satDate = DateTime.Now.AddDays(4).ToShortDateString();
                        dates.Add(satDate);

                        break;
                    }

                case 3: //Wednesday
                    {
                        endOfWeek = DateTime.Now.AddDays(3).ToShortDateString();
                        dates.Add(endOfWeek);
                        sunDate = DateTime.Now.AddDays(-3).ToShortDateString();
                        dates.Add(sunDate);
                        monDate = DateTime.Now.AddDays(-2).ToShortDateString();
                        dates.Add(monDate);
                        tueDate = DateTime.Now.AddDays(-1).ToShortDateString();
                        dates.Add(tueDate);
                        wedDate = DateTime.Now.ToShortDateString();
                        dates.Add(wedDate);
                        thrDate = DateTime.Now.AddDays(1).ToShortDateString();
                        dates.Add(thrDate);
                        friDate = DateTime.Now.AddDays(2).ToShortDateString();
                        dates.Add(friDate);
                        satDate = DateTime.Now.AddDays(3).ToShortDateString();
                        dates.Add(satDate);

                        break;
                    }

                case 4: //Thursday
                    {
                        endOfWeek = DateTime.Now.AddDays(2).ToShortDateString();
                        dates.Add(endOfWeek);
                        sunDate = DateTime.Now.AddDays(-4).ToShortDateString();
                        dates.Add(sunDate);
                        monDate = DateTime.Now.AddDays(-3).ToShortDateString();
                        dates.Add(monDate);
                        tueDate = DateTime.Now.AddDays(-2).ToShortDateString();
                        dates.Add(tueDate);
                        wedDate = DateTime.Now.AddDays(-1).ToShortDateString();
                        dates.Add(wedDate);
                        thrDate = DateTime.Now.ToShortDateString();
                        dates.Add(thrDate);
                        friDate = DateTime.Now.AddDays(1).ToShortDateString();
                        dates.Add(friDate);
                        satDate = DateTime.Now.AddDays(2).ToShortDateString();
                        dates.Add(satDate);

                        break;
                    }

                case 5: //Friday
                    {
                        endOfWeek = DateTime.Now.AddDays(1).ToShortDateString();
                        dates.Add(endOfWeek);
                        sunDate = DateTime.Now.AddDays(-5).ToShortDateString();
                        dates.Add(sunDate);
                        monDate = DateTime.Now.AddDays(-4).ToShortDateString();
                        dates.Add(monDate);
                        tueDate = DateTime.Now.AddDays(-3).ToShortDateString();
                        dates.Add(tueDate);
                        wedDate = DateTime.Now.AddDays(-2).ToShortDateString();
                        dates.Add(wedDate);
                        thrDate = DateTime.Now.AddDays(-1).ToShortDateString();
                        dates.Add(thrDate);
                        friDate = DateTime.Now.ToShortDateString();
                        dates.Add(friDate);
                        satDate = DateTime.Now.AddDays(1).ToShortDateString();
                        dates.Add(satDate);

                        break;
                    }

                case 6: //Saturday
                    {
                        endOfWeek = DateTime.Now.ToShortDateString();
                        dates.Add(endOfWeek);
                        sunDate = DateTime.Now.AddDays(-6).ToShortDateString();
                        dates.Add(sunDate);
                        monDate = DateTime.Now.AddDays(-5).ToShortDateString();
                        dates.Add(monDate);
                        tueDate = DateTime.Now.AddDays(-4).ToShortDateString();
                        dates.Add(tueDate);
                        wedDate = DateTime.Now.AddDays(-3).ToShortDateString();
                        dates.Add(wedDate);
                        thrDate = DateTime.Now.AddDays(-2).ToShortDateString();
                        dates.Add(thrDate);
                        friDate = DateTime.Now.AddDays(-1).ToShortDateString();
                        dates.Add(friDate);
                        satDate = DateTime.Now.ToShortDateString();
                        dates.Add(satDate);

                        break;
                    }
            }
            return dates;
        }

        //Method to retrieve a date List object by employee id and weekday
        public List<string> GetDates(int id, string wED)
        {
            Debug.WriteLine("ID value is: " + id);
            List<string> dates = new List<string>();
            //Select the TimeSheet objects based on the employee id and week ending date
            var sheets = from tsheets in db.TimeSheets
                         where tsheets.Banner_ID == id && tsheets.WeekEnding == wED
                         orderby tsheets.Id ascending
                         select tsheets;

            foreach (TimeSheet sheet in sheets)
            {
                Debug.WriteLine("******************************* DATE SAVED TO LIST *****************************: " + sheet.Date);
                dates.Add(sheet.Date);
            }
            return dates;
        }

        //Queries the TimeSheet table and obtains a list of distinct week ending dates that exist on the table
        public List<string> GetWeekEndingDateList()
        {
            var wED = (from sheets in db.TimeSheets
                       select sheets.WeekEnding).Distinct().OrderBy(WeekEnding => WeekEnding);

            List<string> weekEndDates = new List<string>();
            foreach (string date in wED)
            {
                weekEndDates.Add(date);
            }
            return weekEndDates;
        }

        public List<string> GetWeekEndingDateList(int id)
        {
            var wED = (from sheets in db.TimeSheets
                       where sheets.Banner_ID == id
                       select sheets.WeekEnding).Distinct().OrderBy(WeekEnding => WeekEnding);

            List<string> weekEndDates = new List<string>();
            foreach (string date in wED)
            {
                weekEndDates.Add(date);
            }
            return weekEndDates;
        }

        public List<string> GetApprovedWeekendsList(int id)
        {
            var wED = (from sheets in db.TimeSheets
                       where sheets.Banner_ID == id && sheets.AuthorizedBySupervisor.Equals("True")
                       select sheets.WeekEnding).Distinct().OrderBy(WeekEnding => WeekEnding);
            if(wED == null)
            {
                return null;

            }
            List<string> weekEndDates = new List<string>();
            foreach (string date in wED)
            {
                weekEndDates.Add(date);                
            }
            return weekEndDates;
        }

        //Obtains the fist and list names for a distinct list of employee ids that exist on the
        //TimeSheet db table
        public List<string> GetEmployeeNames(int sid)
        {
            List<string> names = new List<string>();
            var Id = (from sheets in db.TimeSheets
                      select sheets.Banner_ID).Distinct();
            foreach (int id in Id)
            {
                var fname = (from emps in db.Employees
                             where emps.Banner_ID == id && emps.Supervisor == sid.ToString().Trim()
                             select emps.First_Name).FirstOrDefault();
                var lname = (from emps in db.Employees
                             where emps.Banner_ID == id
                             select emps.Last_Name).FirstOrDefault();
                if (fname != null && lname != null)
                {
                    string fullname = fname.Trim() + " " + lname.Trim() + ", " + id;
                    names.Add(fullname);
                }

            }
            return names;
        }
        /** Method Rounds to the nearest 15 minutes and returns a DateTime variable **/
        public DateTime RoundToNearest(DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;
            var offset = roundUp ? d.Ticks : 0;
            return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        }
    }
}


