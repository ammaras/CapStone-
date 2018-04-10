using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Controllers;

namespace TaskLog2ndGen.Tests.Controllers
{
    [TestClass]
    public class ReportsControllerTest
    {
        [TestMethod]
        public void GenerateAdminReportSucced()
        {
            // Arrange
            ReportsController controller = new ReportsController();

            // Act
            Task<ActionResult> actionResult = controller.Admin(DateTime.Today, DateTime.Today);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Admin", viewResult.ViewName);
        }

        [TestMethod]
        public void GenerateEmployeeTimeReportSucced()
        {
            // Arrange
            ReportsController controller = new ReportsController();

            // Act
            Task<ActionResult> actionResult = controller.EmployeeTime(DateTime.Today, DateTime.Today);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("EmployeeTime", viewResult.ViewName);
        }

        [TestMethod]
        public void GenerateEmployeesTimeReportSucced()
        {
            // Arrange
            ReportsController controller = new ReportsController();

            // Act
            Task<ActionResult> actionResult = controller.EmployeesTime(DateTime.Today, DateTime.Today);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("EmployeesTime", viewResult.ViewName);
        }
    }
}