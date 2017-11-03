using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Helpers;
using Timesheet.Controllers;
using TimesheetReports.Controllers;
using System.Web.Mvc;

namespace TimesheetTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            //Arrange
            TimesheetController controller = new TimesheetController();
            //Act
            ViewResult result = controller.Timesheet() as ViewResult;
            //Assert
            Assert.IsNotNull(result);
        }
    }
}
