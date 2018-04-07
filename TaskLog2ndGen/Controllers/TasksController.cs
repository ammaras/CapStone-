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

        // GET: Tasks
        public async Task<ActionResult> Index(string searchCriterion, string sortCriterion)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (String.IsNullOrEmpty(sortCriterion))
            {
                ViewBag.statusSortCriterion = "status";
                ViewBag.teamSortCriterion = "team";
                ViewBag.assignmentSortCriterion = "assignment";
            }
            else
            {
                ViewBag.statusSortCriterion = sortCriterion == "status" ? "status_desc" : "status";
                ViewBag.teamSortCriterion = sortCriterion == "team" ? "team_desc" : "team";
                ViewBag.assignmentSortCriterion = sortCriterion == "assignment" ? "assignment_desc" : "assignment";
            }
            var tasks = db.Tasks.Include(t => t.Application1).Include(t => t.BusinessUnit1).Include(t => t.Category1).Include(t => t.Employee).Include(t => t.Employee1).Include(t => t.Environment1).Include(t => t.Group).Include(t => t.HighLevelEstimate1).Include(t => t.Platform1).Include(t => t.TaskStatu).Include(t => t.Urgency1).Include(t => t.Team);
            if (!String.IsNullOrEmpty(searchCriterion))
            {
                searchCriterion = searchCriterion.ToLower();
                tasks = tasks.Where(t => t.Employee.firstName.Contains(searchCriterion)
                                    || t.Employee.lastName.Contains(searchCriterion)
                                    || t.Employee1.firstName.Contains(searchCriterion)
                                    || t.Employee.lastName.Contains(searchCriterion)
                                    || t.Team.name.Contains(searchCriterion)
                                    || t.Group.name.Contains(searchCriterion)
                                    || t.platform.Contains(searchCriterion)
                                    || t.urgency.Contains(searchCriterion)
                                    || t.BusinessUnit1.description.Contains(searchCriterion)
                                    || t.environment.Contains(searchCriterion)
                                    || t.category.Contains(searchCriterion)
                                    || t.Application1.name.Contains(searchCriterion)
                                    || t.title.Contains(searchCriterion)
                                    || t.description.Contains(searchCriterion)
                                    || t.highLevelEstimate.Contains(searchCriterion)
                                    || t.links.Contains(searchCriterion)
                                    || t.taskStatus.Contains(searchCriterion));
            }
            switch (sortCriterion)
            {
                case "status":
                    tasks = tasks.OrderBy(t => t.taskStatus).ThenByDescending(t => t.dateSubmmited);
                    break;
                case "status_desc":
                    tasks = tasks.OrderByDescending(t => t.taskStatus).ThenByDescending(t => t.dateSubmmited);
                    break;
                case "team":
                    tasks = tasks.OrderBy(t => t.serviceTeam).ThenByDescending(t => t.dateSubmmited);
                    break;
                case "team_desc":
                    tasks = tasks.OrderByDescending(t => t.serviceTeam).ThenByDescending(t => t.dateSubmmited);
                    break;
                case "assignment":
                    tasks = tasks.OrderBy(t => t.Employee.lastName).ThenByDescending(t => t.dateSubmmited); ;
                    break;
                case "assignment_desc":
                    tasks = tasks.OrderByDescending(t => t.Employee.lastName).ThenByDescending(t => t.dateSubmmited); ;
                    break;
                default:
                    tasks = tasks.OrderByDescending(t => t.dateSubmmited);
                    break;
            }
            return View("Index", await tasks.ToListAsync());
        }

        // GET: Tasks
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

        // GET: Tasks
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

        // GET: Tasks
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

        // GET: Tasks
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

        // GET: Tasks/Details/5
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

        // GET: Tasks/Create
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

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Tasks/Edit/5
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

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Tasks/Delete/5
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

        // POST: Tasks/Delete/5
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
            List<TaskReference> taskReferences = await db.TaskReferences.Where(tr => tr.task == task.taskId).ToListAsync();
            foreach (var item in taskReferences)
            {
                db.TaskReferences.Remove(item);
            }
            List<TaskAudit> taskAudits = await db.TaskAudits.Where(ta => ta.task == id).ToListAsync();
            foreach (var item in taskAudits)
            {
                db.TaskAudits.Remove(item);
            }
            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Tasks/Cancel/5
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
            return View(task);
        }

        // POST: Tasks/Cancel/5
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

        // GET: Tasks/Acknowledge/5
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
            return View(task);
        }

        // POST: Tasks/Acknowledge/5
        [HttpPost, ActionName("Acknowledge")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AcknowledgeConfirmed(int id)
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

        // GET: Tasks/RequestClarification/5
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
            ClarificationViewModel clarificationViewModel = new ClarificationViewModel()
            {
                taskId = task.taskId,
                to = task.Employee.email,
                cc = task.Employee1.email,
                from = System.Web.HttpContext.Current != null ? (Session["account"] as Account).Employee.email : null,
            };
            return View(clarificationViewModel);
        }

        // POST: Tasks/RequestClarification/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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