using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Controllers;
using TaskLog2ndGen.Models;
using System.Linq;

namespace TaskLog2ndGen.Tests.Controllers
{
    [TestClass]
    public class EmployeesControllerTest
    {
        private GB_Tasklogtracker_D1Context db;

        public EmployeesControllerTest()
        {
            db = new GB_Tasklogtracker_D1Context();
        }

        [TestMethod]
        public void ViewEmployeesSucceed()
        {
            // Arrange
            EmployeesController controller = new EmployeesController();

            // Act
            Task<ActionResult> actionResult = controller.Index();
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public void ViewEmployeeSucceed()
        {
            // Arrange
            EmployeesController controller = new EmployeesController();
            Employee employee = new Employee()
            {
                team = 1,
                lastName = "Jackson",
                firstName = "Peter",
                email = "pjackson@gmail.com",
                description = "Developer",
                lastChanged = DateTime.Now,
                middleName = "Daniel",
                phone = "2267923774",
                extension = "123456"
            };
            db.Employees.Add(employee);
            db.SaveChanges();

            // Act
            Task<ActionResult> actionResult = controller.Details(employee.employeeId);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Details", viewResult.ViewName);
        }

        [TestMethod]
        public void EditEmployeeSucceed()
        {
            // Arrange
            EmployeesController controller = new EmployeesController();
            Employee employee = db.Employees.AsNoTracking().ToList().Last();
            employee.firstName = "Joe";

            // Act
            Task<ActionResult> actionResult = controller.Edit(employee);

            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void EditEmployeeFail()
        {
            // Arrange
            EmployeesController controller = new EmployeesController();
            Employee employee = db.Employees.AsNoTracking().ToList().Last();
            employee.firstName = null;

            // Act & Assert
            Task<ActionResult> actionResult = controller.Edit(employee);
            Assert.ThrowsException<AggregateException>(() => actionResult.Wait());
        }

        [TestMethod]
        public void DeleteEmployeeSucceed()
        {
            // Arrange
            EmployeesController controller = new EmployeesController();
            Employee employee = db.Employees.AsNoTracking().ToList().Last();

            // Act
            Task<ActionResult> actionResult = controller.DeleteConfirmed(employee.employeeId);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}