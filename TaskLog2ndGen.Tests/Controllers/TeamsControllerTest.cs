using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Controllers;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.Tests.Controllers
{
    [TestClass]
    public class TeamsControllerTest
    {
        [TestMethod]
        public void ViewTeamsSucceed()
        {
            // Arrange
            TeamsController controller = new TeamsController();

            // Act
            Task<ActionResult> actionResult = controller.Index();
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public void CreateTeamSucceed()
        {
            // Arrange
            TeamsController controller = new TeamsController();
            Team team = new Team()
            {
                name = "ITS - Managed Services"
            };

            // Act
            Task<ActionResult> actionResult = controller.Create(team);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void CreateTeamFail()
        {
            // Arrange
            TeamsController controller = new TeamsController();
            Team team = new Team()
            {
                name = null
            };

            // Act & Assert
            Task<ActionResult> actionResult = controller.Create(team);
            Assert.ThrowsException<AggregateException>(() => actionResult.Wait());
        }

        [TestMethod]
        public void ViewTeamSucceed()
        {
            // Arrange
            TeamsController controller = new TeamsController();

            // Act
            Task<ActionResult> actionResult = controller.Details(1);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Details", viewResult.ViewName);
        }

        [TestMethod]
        public void EditTeamSucceed()
        {
            // Arrange
            TeamsController controller = new TeamsController();
            Team team = new Team()
            {
                teamId = 2,
                name = "CHANGE THIS NAME"
            };

            // Act
            Task<ActionResult> actionResult = controller.Edit(team);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void EditTeamFail()
        {
            // Arrange
            TeamsController controller = new TeamsController();
            Team team = new Team()
            {
                teamId = 2,
                name = null
            };

            // Act & Assert
            Task<ActionResult> actionResult = controller.Edit(team);
            Assert.ThrowsException<AggregateException>(() => actionResult.Wait());
        }

        [TestMethod]
        public void DeleteTeamSucceed()
        {
            // Arrange
            TeamsController controller = new TeamsController();

            // Act
            Task<ActionResult> actionResult = controller.DeleteConfirmed(2);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }
    }
}