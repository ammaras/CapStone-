﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Controllers;
using TaskLog2ndGen.Models;
using System.Linq;
using TaskLog2ndGen.ViewModels;
using System.Collections.Generic;

namespace TaskLog2ndGen.Tests.Controllers
{
    [TestClass]
    public class TasksControllerTest
    {
        private GB_Tasklogtracker_D1Context db;

        public TasksControllerTest()
        {
            db = new GB_Tasklogtracker_D1Context();
        }

        [TestMethod]
        public void ViewTasksSucceed()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            Task<ActionResult> actionResult = controller.Index(String.Empty, String.Empty);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public void SearchTasksSucceed()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            Task<ActionResult> actionResult = controller.Index("Not Assigned", String.Empty);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [TestMethod]
        public void ViewTasksByAssignmentSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();

            // Act
            Task<ActionResult> actionResult = controller.TasksByAssignment();
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("TasksByAssignment", viewResult.ViewName);
        }

        [TestMethod]
        public void ViewTasksByTeamSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();

            // Act
            Task<ActionResult> actionResult = controller.TasksByTeam();
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("TasksByTeam", viewResult.ViewName);
        }

        [TestMethod]
        public void ViewTasksByTimeSpentSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();

            // Act
            Task<ActionResult> actionResult = controller.TasksByTimeSpent();
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("TasksByTimeSpent", viewResult.ViewName);
        }

        [TestMethod]
        public void ViewTasksByStatusSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();

            // Act
            Task<ActionResult> actionResult = controller.TasksByStatus();
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("TasksByStatus", viewResult.ViewName);
        }

        [TestMethod]
        public void CreateTaskSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();
            TaskReferenceViewModel taskReferenceViewModel = new TaskReferenceViewModel()
            {
                primaryContact = 1,
                secondaryContact = 1,
                dateLogged = DateTime.Now,
                dateSubmmited = DateTime.Now,
                serviceTeam = 1,
                serviceGroup = 1,
                platform = "SQL",
                urgency = "Medium",
                businessUnit = 1,
                environment = "Non-Production",
                category = "Support",
                application = 1,
                references = new List<Reference>() { new Reference { referenceNo = "CH0099999", referenceType = "SNOW" } },
                title = "DBA support needed",
                description = "Apply the security",
                highLevelEstimate = "days",
                links = "http://localhost:58403/",
                taskStatus = "Not Assigned"
            };

            // Act
            Task<ActionResult> actionResult = controller.Create(taskReferenceViewModel);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void CreateTaskFail()
        {
            // Arrange
            TasksController controller = new TasksController();
            TaskReferenceViewModel taskReferenceViewModel = new TaskReferenceViewModel()
            {
                primaryContact = 1,
                secondaryContact = 1,
                dateLogged = DateTime.Now,
                dateSubmmited = DateTime.Now,
                serviceTeam = 1,
                serviceGroup = 1,
                platform = "SQL",
                urgency = "Medium",
                businessUnit = 1,
                environment = "Non-Production",
                category = "Support",
                application = 1,
                references = new List<Reference>() { new Reference { referenceNo = "CH0088888", referenceType = "SNOW" } },
                title = null,
                description = "Apply the security",
                highLevelEstimate = "days",
                links = "http://localhost:58403/",
                taskStatus = "Not Assigned"
            };

            // Act & Assert
            Task<ActionResult> actionResult = controller.Create(taskReferenceViewModel);
            Assert.ThrowsException<AggregateException>(() => actionResult.Wait());
        }

        [TestMethod]
        public void ViewTaskSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();
            Models.Task task = db.Tasks.AsNoTracking().ToList().Last();

            // Act
            Task<ActionResult> actionResult = controller.Details(task.taskId);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("Details", viewResult.ViewName);
        }

        [TestMethod]
        public void EditTaskSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();
            Models.Task task = db.Tasks.AsNoTracking().ToList().Last();
            TaskReferenceViewModel taskReferenceViewModel = new TaskReferenceViewModel()
            {
                taskId = task.taskId,
                primaryContact = task.primaryContact,
                secondaryContact = task.secondaryContact,
                dateLogged = task.dateLogged,
                dateSubmmited = task.dateSubmmited,
                serviceTeam = task.serviceTeam,
                serviceGroup = task.serviceGroup,
                platform = task.platform,
                urgency = task.urgency,
                businessUnit = task.businessUnit,
                environment = task.environment,
                category = task.category,
                application = task.application,
                title = "DBA - Managed Services support needed",
                description = task.description,
                highLevelEstimate = task.highLevelEstimate,
                links = task.links,
                taskStatus = task.taskStatus
            };
            foreach (var item in task.TaskReferences)
            {
                taskReferenceViewModel.references.Add(item.Reference1);
            }

            // Act
            Task<ActionResult> actionResult = controller.Edit(taskReferenceViewModel);

            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void EditTaskFail()
        {
            // Arrange
            TasksController controller = new TasksController();
            Models.Task task = db.Tasks.AsNoTracking().ToList().Last();
            TaskReferenceViewModel taskReferenceViewModel = new TaskReferenceViewModel()
            {
                taskId = task.taskId,
                primaryContact = task.primaryContact,
                secondaryContact = task.secondaryContact,
                dateLogged = task.dateLogged,
                dateSubmmited = task.dateSubmmited,
                serviceTeam = task.serviceTeam,
                serviceGroup = task.serviceGroup,
                platform = task.platform,
                urgency = task.urgency,
                businessUnit = task.businessUnit,
                environment = task.environment,
                category = task.category,
                application = task.application,
                title = null,
                description = task.description,
                highLevelEstimate = task.highLevelEstimate,
                links = task.links,
                taskStatus = task.taskStatus
            };
            foreach (var item in task.TaskReferences)
            {
                taskReferenceViewModel.references.Add(item.Reference1);
            }

            // Act & Assert
            Task<ActionResult> actionResult = controller.Edit(taskReferenceViewModel);
            Assert.ThrowsException<AggregateException>(() => actionResult.Wait());
        }

        [TestMethod]
        public void CancelTaskSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();
            Models.Task task = db.Tasks.AsNoTracking().ToList().Last();

            // Act
            Task<ActionResult> actionResult = controller.CancelConfirmed(task.taskId);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void DeleteTaskSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();
            Models.Task task = db.Tasks.AsNoTracking().ToList().Last();

            // Act
            Task<ActionResult> actionResult = controller.DeleteConfirmed(task.taskId);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void AcknowledgeTaskSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();
            Models.Task task = db.Tasks.AsNoTracking().ToList().Last();

            // Act
            Task<ActionResult> actionResult = controller.AcknowledgeConfirmed(task.taskId);
            actionResult.Wait();
            RedirectToRouteResult redirectToRouteResult = actionResult.Result as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void RequestClarificationSucceed()
        {
            // Arrange
            TasksController controller = new TasksController();
            Models.Task task = db.Tasks.AsNoTracking().ToList().Last();
            ClarificationViewModel clarificationViewModel = new ClarificationViewModel()
            {
                taskId = task.taskId,
                to = task.Employee.email,
                cc = task.Employee1.email,
                from = "pjackson@gmail.com",
                subject = "Need Clarification",
                body = "For Task Id: 1"
            };

            // Act
            ActionResult actionResult = controller.RequestClarification(clarificationViewModel);

            RedirectToRouteResult redirectToRouteResult = actionResult as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", redirectToRouteResult.RouteValues["action"]);
        }

        [TestMethod]
        public void ViewTaskSummarySucceed()
        {
            // Arrange
            TasksController controller = new TasksController();
            Models.Task task = db.Tasks.AsNoTracking().ToList().Last();

            // Act
            Task<ActionResult> actionResult = controller.TaskSummary(task.taskId);
            actionResult.Wait();
            ViewResult viewResult = actionResult.Result as ViewResult;

            // Assert
            Assert.AreEqual("TaskSummary", viewResult.ViewName);
        }
    }
}