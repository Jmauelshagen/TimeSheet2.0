using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Models;

namespace Timesheet.Controllers
{
    public class TimesheetController : Controller
    {
        // GET: Timesheet
        public ActionResult Timesheet()
        {
            return View();
        }

        public ActionResult GetTimeSheet()
        {
            Employee emp = (Employee)Session["Employee"];
            List<string> dates = GetDates();
            Session["Dates"] = dates;

            using (LoginDatabaseEntities1 db = new LoginDatabaseEntities1())
            {
                var sheets = from tsheets in db.TimeSheets
                             where tsheets.EmpId == emp.EmpId && tsheets.WeekEnding == dates.ElementAt(0)
                             select tsheets;

                if (sheets.Count() == 0)
                {
                    Session["WeekEndDate"] = GetDates();
                }
            }

            return View();
        }

        //This method determines the curent date and then derives the dates for each day of the week
        public List<string> GetDates()
        {
            List<string> dates = null;
            string endOfWeek = "";
            string sunDate = "";
            string monDate = "";
            string tueDate = "";
            string wedDate = "";
            string thrDate = "";
            string friDate = "";
            string satDate = "";
            int hoy = int.Parse(DateTime.Now.DayOfWeek.ToString());

            switch (hoy)
            {
                case 1: //Sunday
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

                case 2: //Monday
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

                case 3: //Tuesday
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

                case 4: //Wednesday
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

                case 5: //Thursday
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

                case 6: //Friday
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

                case 7: //Saturday
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
    }
}