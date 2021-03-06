﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Controllers;
using TaskLog2ndGen.ViewModels;

namespace TaskLog2ndGen.Tests.Controllers
{
    [TestClass]
    public class LoginControllerTest
    {
        [TestMethod]
        public void LogInSucceed()
        {
            // Arrange
            LoginController controller = new LoginController();
            AccountViewModel accountViewModel = new AccountViewModel()
            {
                username = "admin",
                password = "admin"
            };

            // Act
            Task<ActionResult> actionResult = controller.Index(accountViewModel);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void LogInFail()
        {
            // Arrange
            LoginController controller = new LoginController();
            AccountViewModel accountViewModel = new AccountViewModel()
            {
                username = "administrator",
                password = "administrator"
            };

            // Act
            Task<ActionResult> actionResult = controller.Index(accountViewModel);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Index", viewResult.ViewName);
        }
    }
}