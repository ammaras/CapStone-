using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Models;
using TaskLog2ndGen.ViewModels;
using System.Linq;
using System.Net.Mail;

namespace TaskLog2ndGen.Controllers
{
    /// <summary>
    /// Handles get and post http requests for CRUD operations on task table
    /// </summary>
    public class TasksController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();
        private const string CANCELLED_TASK_STATUS = "Cancelled";
        private const string NOT_ASSIGNED_TASK_STATUS = "Not Assigned";
        private const string ACKNOWLEDGED_TASK_STATUS = "Acknowledged";
        private const string COMM_TASK_AUDIT_TYPE = "Communication";
        private const string FIELD_CHANGE_TASK_AUDIT_TYPE = "Field Changes";
        private const string DOC_CREATED_NOTE = "Document Created";
        private const string DOC_UPDATED_NOTE = "Document Updated";
        private const string STATUS_UPDATED_NOTE = "Status changed from {0} to {1}";
        private const string MAIL_SERVER = "server.address.com";
        private const string MAIL_SERVER_USERNAME = "username";
        private const string MAIL_SERVER_PASSWORD = "password";
        private const string SYSTEM_NOTIFICATION_MAIL = "tasklog2ndgen@manulife.com";
        private const string SYSTEM_NOTIFICATION_SUBJECT = "Task Id: {0} acknowledgement notification. Do not reply.";
        private const string SYSTEM_NOTIFICATION_BODY = "Task Id: {0} has been acknowledge by {1}.";

        /// <summary>
        /// Handles get request and retrieves all tasks by assignment (assigned employees)
        /// </summary>
        /// <returns>View displaying retrieve tasks by assignment (assigned employees)</returns>
        public async Task<ActionResult> TasksByAssignment()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            var employees = await db.Employees.OrderBy(e => e.lastName).ToListAsync();
            List<TaskByAssignmentViewModel> tasksByAssignment = new List<TaskByAssignmentViewModel>();
            foreach (var employee in employees)
            {
                var taskByTeam = new TaskByAssignmentViewModel
                {
                    employee = employee
                };
                var workSheets = await db.Worksheets.Where(ws => ws.employee == employee.employeeId).GroupBy(ws => ws.Task1).Select(grp => grp.FirstOrDefault()).OrderByDescending(ws => ws.Task1.dateSubmmited).ToListAsync();
                foreach (var workSheet in workSheets)
                {
                    taskByTeam.tasks.Add(workSheet.Task1);
                }
                tasksByAssignment.Add(taskByTeam);
            }
            return View("TasksByAssignment", tasksByAssignment);
        }

        /// <summary>
        /// Handles get request and retrieves all tasks by team
        /// </summary>
        /// <returns>View displaying retrieve tasks by team</returns>
        public async Task<ActionResult> TasksByTeam()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            var teams = await db.Teams.OrderBy(t => t.name).ToListAsync();
            List<TaskByTeamViewModel> tasksByTeam = new List<TaskByTeamViewModel>();
            foreach (var team in teams)
            {
                TaskByTeamViewModel taskByTeam = new TaskByTeamViewModel
                {
                    team = team
                };
                var Tasks = await db.Tasks.Where(t => t.serviceTeam == team.teamId).OrderByDescending(t => t.dateSubmmited).ToListAsync();
                foreach (var task in Tasks)
                {
                    taskByTeam.tasks.Add(task);
                }
                tasksByTeam.Add(taskByTeam);
            }
            return View("TasksByTeam", tasksByTeam);
        }

        /// <summary>
        /// Handles get request and retrieves all tasks by status
        /// </summary>
        /// <returns>View displaying retrieve tasks by status</returns>
        public async Task<ActionResult> TasksByStatus()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            var taskStati = await db.TaskStatus.OrderBy(ts => ts.taskStatusCode).ToListAsync();
            List<TaskByStatusViewModel> tasksByStatus = new List<TaskByStatusViewModel>();
            foreach (var taskStatus in taskStati)
            {
                TaskByStatusViewModel taskByStatus = new TaskByStatusViewModel
                {
                    taskStatus = taskStatus
                };
                var Tasks = await db.Tasks.Where(t => t.taskStatus == taskStatus.taskStatusCode).OrderByDescending(t => t.dateSubmmited).ToListAsync();
                foreach (var task in Tasks)
                {
                    taskByStatus.tasks.Add(task);
                }
                tasksByStatus.Add(taskByStatus);
            }
            return View("TasksByStatus", tasksByStatus);
        }

        /// <summary>
        /// Handles get request and retrieves all tasks by time spent
        /// </summary>
        /// <returns>View displaying retrieve tasks by time spent</returns>
        public async Task<ActionResult> TasksByTimeSpent()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            var tasks = await db.Tasks.ToListAsync();
            var taskViewModels = new List<TaskViewModel>();
            foreach (var task in tasks)
            {
                var taskViewModel = new TaskViewModel
                {
                    task = task,
                    totalTimeSpent = 0,
                    assignedEmployees = ""
                };
                var workSheets = await db.Worksheets.Where(ws => ws.task == task.taskId).ToListAsync();
                decimal totalTimeSpent = 0;
                foreach (var workSheet in workSheets)
                {
                    totalTimeSpent += workSheet.timeSpent;
                }
                taskViewModel.totalTimeSpent = totalTimeSpent;
                workSheets = workSheets.GroupBy(ws => ws.Employee1).Select(grp => grp.FirstOrDefault()).ToList();
                foreach (var workSheet in workSheets)
                {
                    if (String.IsNullOrEmpty(taskViewModel.assignedEmployees))
                    {
                        taskViewModel.assignedEmployees += workSheet.Employee1.fullName;
                    }
                    else
                    {
                        taskViewModel.assignedEmployees += "\n" + workSheet.Employee1.fullName;
                    }
                }
                taskViewModels.Add(taskViewModel);
            }
            var tasksByTimeSpent = new List<TaskByTimeSpentViewModel>();
            var totalTimesSpent = taskViewModels.Select(t => t.totalTimeSpent).Distinct();
            foreach (var totalTimeSpent in totalTimesSpent.OrderByDescending(tts => tts))
            {
                var taskByTimeSpent = new TaskByTimeSpentViewModel
                {
                    totalTimeSpent = totalTimeSpent
                };
                foreach (var tempTaskViewModel in taskViewModels.Where(t => t.totalTimeSpent == totalTimeSpent).OrderByDescending(t => t.task.dateSubmmited))
                {
                    taskByTimeSpent.tasks.Add(tempTaskViewModel);
                }
                tasksByTimeSpent.Add(taskByTimeSpent);
            }
            return View("TasksByTimeSpent", tasksByTimeSpent);
        }

        /// <summary>
        /// Handles get request and retrieves a task
        /// </summary>
        /// <param name="id">Id of task to retrieve</param>
        /// <returns>View displaying retrieved task</returns>
        public async Task<ActionResult> Details(int? id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View("Details", task);
        }

        /// <summary>
        /// Handles get request and sets up form to create task
        /// </summary>
        /// <returns>View displaying form to create task</returns>
        public ActionResult Create()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            ViewBag.application = new SelectList(db.Applications, "applicationId", "name");
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description");
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode");
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "fullName");
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "fullName");
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode");
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode");
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode");
            ViewBag.refTypes = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode");
            ViewBag.ReferenceTypes = db.ReferenceTypes.ToList();
            ViewBag.serviceGroup = new SelectList(db.Groups, "groupId", "name");
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode");
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name");
            ViewBag.taskStatus = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode");
            ViewBag.dateLogged = DateTime.Now;
            return View();
        }

        /// <summary>
        /// Handles post request and creates task
        /// </summary>
        /// <param name="taskReferenceViewModel">Task to create</param>
        /// <returns>Redirection to TasksController.Details action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "primaryContact,secondaryContact,dateLogged,serviceTeam,serviceGroup,platform,urgency,businessUnit,environment,category,application,references,title,description,highLevelEstimate,links")] TaskReferenceViewModel taskReferenceViewModel)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            taskReferenceViewModel.dateSubmmited = DateTime.Now;
            foreach (var item in taskReferenceViewModel.references)
            {
                if (String.IsNullOrEmpty(item.referenceNo))
                {
                    ModelState.AddModelError("references[" + taskReferenceViewModel.references.IndexOf(item) + "].referenceNo", "Reference number is required.");
                }
            }
            if (ModelState.IsValid)
            {
                Models.Task task = new Models.Task
                {
                    primaryContact = taskReferenceViewModel.primaryContact,
                    secondaryContact = taskReferenceViewModel.secondaryContact,
                    dateLogged = taskReferenceViewModel.dateLogged,
                    dateSubmmited = taskReferenceViewModel.dateSubmmited,
                    serviceTeam = taskReferenceViewModel.serviceTeam,
                    serviceGroup = taskReferenceViewModel.serviceGroup,
                    platform = taskReferenceViewModel.platform,
                    urgency = taskReferenceViewModel.urgency,
                    businessUnit = taskReferenceViewModel.businessUnit,
                    environment = taskReferenceViewModel.environment,
                    category = taskReferenceViewModel.category,
                    application = taskReferenceViewModel.application,
                    title = taskReferenceViewModel.title,
                    description = taskReferenceViewModel.description,
                    highLevelEstimate = taskReferenceViewModel.highLevelEstimate,
                    links = taskReferenceViewModel.links,
                    taskStatus = NOT_ASSIGNED_TASK_STATUS
                };
                db.Tasks.Add(task);
                await db.SaveChangesAsync();
                foreach (var item in taskReferenceViewModel.references)
                {
                    Reference reference = await db.References.Where(r => r.referenceNo == item.referenceNo && r.referenceType == item.referenceType).SingleOrDefaultAsync();
                    if (reference == null)
                    {
                        reference = new Reference
                        {
                            referenceNo = item.referenceNo,
                            referenceType = item.referenceType
                        };
                        db.References.Add(reference);
                        await db.SaveChangesAsync();
                    }
                    TaskReference taskReference = new TaskReference
                    {
                        task = task.taskId,
                        reference = reference.referenceId
                    };
                    db.TaskReferences.Add(taskReference);
                    await db.SaveChangesAsync();
                }
                TaskAudit taskAudit = new TaskAudit
                {
                    task = task.taskId,
                    taskAuditType = COMM_TASK_AUDIT_TYPE,
                    dateLogged = task.dateLogged,
                    loggedBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1,
                    notes = DOC_CREATED_NOTE,
                    taskStatus = task.taskStatus
                };
                db.TaskAudits.Add(taskAudit);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = task.taskId });
            }
            ViewBag.application = new SelectList(db.Applications, "applicationId", "name", taskReferenceViewModel.application);
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description", taskReferenceViewModel.businessUnit);
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode", taskReferenceViewModel.category);
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "fullName", taskReferenceViewModel.secondaryContact);
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "fullName", taskReferenceViewModel.primaryContact);
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode", taskReferenceViewModel.environment);
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode", taskReferenceViewModel.highLevelEstimate);
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode", taskReferenceViewModel.platform);
            ViewBag.refTypes = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode");
            ViewBag.ReferenceTypes = db.ReferenceTypes.ToList();
            ViewBag.serviceGroup = new SelectList(db.Groups, "groupId", "name", taskReferenceViewModel.serviceGroup);
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode", taskReferenceViewModel.urgency);
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name", taskReferenceViewModel.serviceTeam);
            ViewBag.taskStatus = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode", taskReferenceViewModel.taskStatus);
            ViewBag.dateLogged = taskReferenceViewModel.dateLogged;
            return View(taskReferenceViewModel);
        }

        /// <summary>
        /// Handles get request and sets up form to edit task
        /// </summary>
        /// <param name="id">Id of task to edit</param>
        /// <returns>View displaying form to edit task</returns>
        public async Task<ActionResult> Edit(int? id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            else if (task.taskStatus == CANCELLED_TASK_STATUS)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            TaskReferenceViewModel taskReferenceViewModel = new TaskReferenceViewModel
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
                title = task.title,
                description = task.description,
                highLevelEstimate = task.highLevelEstimate,
                links = task.links,
                taskStatus = task.taskStatus
            };
            foreach (var item in task.TaskReferences)
            {
                taskReferenceViewModel.references.Add(item.Reference1);
            }
            ViewBag.application = new SelectList(db.Applications, "applicationId", "name", task.application);
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description", task.businessUnit);
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode", task.category);
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "fullName", task.secondaryContact);
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "fullName", task.primaryContact);
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode", task.environment);
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode", task.highLevelEstimate);
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode", task.platform);
            ViewBag.refTypes = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode");
            ViewBag.ReferenceTypes = db.ReferenceTypes.ToList();
            ViewBag.serviceGroup = new SelectList(db.Groups, "groupId", "name", task.serviceGroup);
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode", task.urgency);
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name", task.serviceTeam);
            ViewBag.taskStatus = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode", task.taskStatus);
            return View(taskReferenceViewModel);
        }

        /// <summary>
        /// Handles post request and edits task
        /// </summary>
        /// <param name="taskReferenceViewModel">Task to edit</param>
        /// <returns>Redirection to TasksController.Details action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "taskId,primaryContact,secondaryContact,serviceTeam,serviceGroup,platform,urgency,businessUnit,environment,category,application,references,title,description,highLevelEstimate,links")] TaskReferenceViewModel taskReferenceViewModel)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            foreach (var item in taskReferenceViewModel.references)
            {
                if (String.IsNullOrEmpty(item.referenceNo))
                {
                    ModelState.AddModelError("references[" + taskReferenceViewModel.references.IndexOf(item) + "].referenceNo", "Reference number is required.");
                }
                else if (item.referenceNo.Length > 50)
                {
                    ModelState.AddModelError("references[" + taskReferenceViewModel.references.IndexOf(item) + "].referenceNo", "Reference number cannot have more than 50 characters..");
                }
            }
            if (ModelState.IsValid)
            {
                Models.Task task = await db.Tasks.FindAsync(taskReferenceViewModel.taskId);
                task.primaryContact = taskReferenceViewModel.primaryContact;
                task.secondaryContact = taskReferenceViewModel.secondaryContact;
                task.serviceTeam = taskReferenceViewModel.serviceTeam;
                task.serviceGroup = taskReferenceViewModel.serviceGroup;
                task.platform = taskReferenceViewModel.platform;
                task.urgency = taskReferenceViewModel.urgency;
                task.businessUnit = taskReferenceViewModel.businessUnit;
                task.environment = taskReferenceViewModel.environment;
                task.category = taskReferenceViewModel.category;
                task.application = taskReferenceViewModel.application;
                task.title = taskReferenceViewModel.title;
                task.description = taskReferenceViewModel.description;
                task.highLevelEstimate = taskReferenceViewModel.highLevelEstimate;
                task.links = taskReferenceViewModel.links;
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
                List<TaskReference> taskReferences = await db.TaskReferences.Where(tr => tr.task == task.taskId).ToListAsync();
                foreach (var item in taskReferences)
                {
                    db.TaskReferences.Remove(item);
                }
                await db.SaveChangesAsync();
                foreach (var item in taskReferenceViewModel.references)
                {
                    Reference reference = await db.References.Where(r => r.referenceNo == item.referenceNo && r.referenceType == item.referenceType).SingleOrDefaultAsync();
                    if (reference == null)
                    {
                        reference = new Reference
                        {
                            referenceNo = item.referenceNo,
                            referenceType = item.referenceType
                        };
                        db.References.Add(reference);
                        await db.SaveChangesAsync();
                    }
                    TaskReference taskReference = new TaskReference
                    {
                        task = task.taskId,
                        reference = reference.referenceId
                    };
                    db.TaskReferences.Add(taskReference);
                    await db.SaveChangesAsync();
                }
                TaskAudit taskAudit = new TaskAudit
                {
                    task = task.taskId,
                    taskAuditType = FIELD_CHANGE_TASK_AUDIT_TYPE,
                    dateLogged = DateTime.Now,
                    loggedBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1,
                    notes = DOC_UPDATED_NOTE,
                    taskStatus = task.taskStatus
                };
                db.TaskAudits.Add(taskAudit);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = task.taskId });
            }
            ViewBag.application = new SelectList(db.Applications, "applicationId", "name", taskReferenceViewModel.application);
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description", taskReferenceViewModel.businessUnit);
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode", taskReferenceViewModel.category);
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "fullName", taskReferenceViewModel.secondaryContact);
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "fullName", taskReferenceViewModel.primaryContact);
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode", taskReferenceViewModel.environment);
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode", taskReferenceViewModel.highLevelEstimate);
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode", taskReferenceViewModel.platform);
            ViewBag.refTypes = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode");
            ViewBag.ReferenceTypes = db.ReferenceTypes.ToList();
            ViewBag.serviceGroup = new SelectList(db.Groups, "groupId", "name", taskReferenceViewModel.serviceGroup);
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode", taskReferenceViewModel.urgency);
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name", taskReferenceViewModel.serviceTeam);
            ViewBag.taskStatus = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode", taskReferenceViewModel.taskStatus);
            return View(taskReferenceViewModel);
        }

        /// <summary>
        /// Handles get request and asks for confirmation to delete task
        /// </summary>
        /// <param name="id">Id of task to delete</param>
        /// <returns>View displaying task to delete and asking for confirmation</returns>

        public async Task<ActionResult> Delete(int? id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        /// <summary>
        /// Handles post request and deletes task
        /// </summary>
        /// <param name="id">Id of task to delete</param>
        /// <returns>Redirection to TasksController.Index action</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            List<TaskReference> taskReferences = await db.TaskReferences.Where(tr => tr.task == id).ToListAsync();
            foreach (var taskReference in taskReferences)
            {
                db.TaskReferences.Remove(taskReference);
            }
            List<TaskAudit> taskAudits = await db.TaskAudits.Where(ta => ta.task == id).ToListAsync();
            foreach (var taskAudit in taskAudits)
            {
                db.TaskAudits.Remove(taskAudit);
            }
            List<Worksheet> worksheets = await db.Worksheets.Where(ws => ws.task == id).ToListAsync();
            foreach (var worksheet in worksheets)
            {
                List<WorksheetAudit> worksheetAudits = await db.WorksheetAudits.Where(wa => wa.worksheet == worksheet.worksheetId).ToListAsync();
                foreach (var worksheetAudit in worksheetAudits)
                {
                    db.WorksheetAudits.Remove(worksheetAudit);
                }
                db.Worksheets.Remove(worksheet);
            }
            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles get request and asks for confirmation to cancel task
        /// </summary>
        /// <param name="id">Id of task to cancel</param>
        /// <returns>View displaying task to cancel and asking for confirmation</returns>
        public async Task<ActionResult> Cancel(int? id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            else if (task.taskStatus == CANCELLED_TASK_STATUS)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(task);
        }

        /// <summary>
        /// Handles post request and cancels task
        /// </summary>
        /// <param name="id">Id of task to cancel</param>
        /// <returns>Redirection to TasksController.Details action</returns>
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelConfirmed(int id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            TaskAudit taskAudit = new TaskAudit
            {
                task = task.taskId,
                taskAuditType = COMM_TASK_AUDIT_TYPE,
                dateLogged = DateTime.Now,
                loggedBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1,
                notes = String.Format(STATUS_UPDATED_NOTE, task.taskStatus, CANCELLED_TASK_STATUS),
                taskStatus = task.taskStatus
            };
            task.taskStatus = CANCELLED_TASK_STATUS;
            db.Entry(task).State = EntityState.Modified;
            db.TaskAudits.Add(taskAudit);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = task.taskId });
        }

        /// <summary>
        /// Handles get request and asks for confirmation to acknowledge task
        /// </summary>
        /// <param name="id">Id of task to acknowledge</param>
        /// <returns>View displaying task to acknowledge and asking for confirmation</returns>
        public async Task<ActionResult> Acknowledge(int? id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            else if (task.taskStatus == ACKNOWLEDGED_TASK_STATUS)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(task);
        }

        /// <summary>
        /// Handles post request and acknowledges task
        /// </summary>
        /// <param name="id">Id of task to acknowledge</param>
        /// <returns>Redirection to TasksController.Details action</returns>
        [HttpPost, ActionName("Acknowledge")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AcknowledgeConfirmed(int id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task.taskStatus == ACKNOWLEDGED_TASK_STATUS)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            TaskAudit taskAudit = new TaskAudit
            {
                task = task.taskId,
                taskAuditType = COMM_TASK_AUDIT_TYPE,
                dateLogged = DateTime.Now,
                loggedBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1,
                notes = String.Format(STATUS_UPDATED_NOTE, task.taskStatus, ACKNOWLEDGED_TASK_STATUS),
                taskStatus = task.taskStatus
            };
            task.taskStatus = ACKNOWLEDGED_TASK_STATUS;
            db.Entry(task).State = EntityState.Modified;
            db.TaskAudits.Add(taskAudit);
            await db.SaveChangesAsync();
            SmtpClient client = new SmtpClient(MAIL_SERVER)
            {
                Credentials = new NetworkCredential(MAIL_SERVER_USERNAME, MAIL_SERVER_PASSWORD)
            };
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(task.Employee.email));
            if (!String.IsNullOrEmpty(task.Employee1.email))
            {
                mailMessage.To.Add(new MailAddress(task.Employee1.email));
            }
            mailMessage.Sender = new MailAddress(SYSTEM_NOTIFICATION_MAIL);
            mailMessage.Subject = String.Format(SYSTEM_NOTIFICATION_SUBJECT, task.taskId);
            mailMessage.Body = String.Format(SYSTEM_NOTIFICATION_BODY, task.taskId, System.Web.HttpContext.Current != null ? (Session["account"] as Account).Employee.fullName : null);
            //client.Send(mailMessage); /////////////////////////// WILL FAIL, AS NO MAIL SERVER /////////////////////////// 
            return RedirectToAction("Details", new { id = task.taskId });
        }

        /// <summary>
        /// Handles get request and sets up form to request clarification
        /// </summary>
        /// <param name="id">Id of task for which to request clarification</param>
        /// <returns>View displaying form to request clarification</returns>
        public async Task<ActionResult> RequestClarification(int? id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            Account account = await db.Accounts.FindAsync((Session["account"] as Account).employeeId);
            string from = System.Web.HttpContext.Current != null ? account.Employee.email : null;
            ClarificationViewModel clarificationViewModel = new ClarificationViewModel
            {
                taskId = task.taskId,
                to = task.Employee.email,
                cc = task.Employee1.email,
                subject = "Task Id: " + task.taskId + " - " + task.title,
                from = from
            };
            return View(clarificationViewModel);
        }

        /// <summary>
        /// Handles post request and sends clarification request
        /// </summary>
        /// <param name="clarificationViewModel">Clarification request to send</param>
        /// <returns>Redirection to TasksController.Details action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestClarification([Bind(Include = "taskId,to,cc,from,subject,body")] ClarificationViewModel clarificationViewModel)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (ModelState.IsValid)
            {
                SmtpClient client = new SmtpClient(MAIL_SERVER)
                {
                    Credentials = new NetworkCredential(MAIL_SERVER_USERNAME, MAIL_SERVER_PASSWORD)
                };
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(new MailAddress(clarificationViewModel.to));
                if (!String.IsNullOrEmpty(clarificationViewModel.cc))
                {
                    mailMessage.To.Add(new MailAddress(clarificationViewModel.cc));
                }
                mailMessage.Sender = new MailAddress(clarificationViewModel.from);
                mailMessage.Subject = clarificationViewModel.subject;
                mailMessage.Body = clarificationViewModel.body;
                //client.Send(mailMessage); /////////////////////////// WILL FAIL, AS NO MAIL SERVER /////////////////////////// 
                return RedirectToAction("Details", new { id = clarificationViewModel.taskId });
            }
            return View(clarificationViewModel);
        }

        /// <summary>
        /// Handles get request and retrieves a task for which to display summary
        /// </summary>
        /// <param name="id">Id of task to retrieve</param>
        /// <returns>View displaying retrieved task summary</returns>
        public async Task<ActionResult> TaskSummary(int? id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View("TaskSummary", task);
        }

        /// <summary>
        /// Disposes controller at the end of its life cycle
        /// </summary>
        /// <param name="disposing">Flag to dispose database context if true</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}