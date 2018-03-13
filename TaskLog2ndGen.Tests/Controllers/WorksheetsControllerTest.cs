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
    public class WorksheetsControllerTest
    {
        private GB_Tasklogtracker_D1Context db;

        public WorksheetsControllerTest()
        {
            db = new GB_Tasklogtracker_D1Context();
        }

        [TestMethod]
        public void ViewWorksheetsSucceed()
        {
            // Arrange
            WorksheetsController controller = new WorksheetsController();

            // Act
            Task<ActionResult> actionResult = controller.Index(1);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public void CreateWorksheetSucceed()
        {
            // Arrange
            WorksheetsController controller = new WorksheetsController();
            Worksheet worksheet = new Worksheet
            {
                employee = 1,
                task = 1,
                notes = "Created worksheet",
                timeSpent = 1.75M,
                overtime = false,
                onCall = false,
                links = "http://localhost:58403/"
            };

            // Act
            Task<ActionResult> actionResult = controller.Create(worksheet);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void CreateWorksheetFail()
        {
            // Arrange
            WorksheetsController controller = new WorksheetsController();
            Worksheet worksheet = new Worksheet
            {
                employee = 1,
                task = 1,
                notes = null,
                timeSpent = 1.75M,
                overtime = false,
                onCall = false,
                links = "http://localhost:58403/"
            };

            // Act & Assert
            Task<ActionResult> actionResult = controller.Create(worksheet);
            Assert.ThrowsException<AggregateException>(() => actionResult.Wait());
        }

        [TestMethod]
        public void ViewWorksheetSucceed()
        {
            // Arrange
            WorksheetsController controller = new WorksheetsController();
            Worksheet worksheet = db.Worksheets.AsNoTracking().ToList().Last();

            // Act
            Task<ActionResult> actionResult = controller.Details(worksheet.worksheetId);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Details", viewResult.ViewName);
        }

        [TestMethod]
        public void EditWorksheetSucceed()
        {
            // Arrange
            WorksheetsController controller = new WorksheetsController();
            Worksheet worksheet = db.Worksheets.AsNoTracking().ToList().Last();
            worksheet.notes = "Updated worksheet";

            // Act
            Task<ActionResult> actionResult = controller.Edit(worksheet);

            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void EditWorksheetFail()
        {
            // Arrange
            WorksheetsController controller = new WorksheetsController();
            Worksheet worksheet = db.Worksheets.AsNoTracking().ToList().Last();
            worksheet.notes = null;

            // Act & Assert
            Task<ActionResult> actionResult = controller.Edit(worksheet);
            Assert.ThrowsException<AggregateException>(() => actionResult.Wait());
        }

        [TestMethod]
        public void DeleteWorksheetSucceed()
        {
            // Arrange
            WorksheetsController controller = new WorksheetsController();
            Worksheet worksheet = db.Worksheets.AsNoTracking().ToList().Last();

            // Act
            Task<ActionResult> actionResult = controller.DeleteConfirmed(worksheet.worksheetId);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}