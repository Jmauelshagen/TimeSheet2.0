using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Timesheet.Controllers
{
    public class HelpController : Controller
    {
        // GET: Help
        public ActionResult HRhelp()
        {
            return View();
        }

        // GET: Help
        public ActionResult Suphelp()
        {
            return View();
        }
        public ActionResult Emphelp()
        {
            return View();
        }
    }
}