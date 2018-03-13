using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Models;
using TaskLog2ndGen.ViewModels;
using System.Linq;

namespace TaskLog2ndGen.Controllers
{
    public class TasksController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();
        private const string CANCELLED_TASK_STATUS = "Cancelled";
        private const string NOT_ASSIGNED_TASK_STATUS = "Not Assigned";
        private const string COMM_TASK_AUDIT_TYPE = "Communication";
        private const string FIELD_CHANGE_TASK_AUDIT_TYPE = "Field Changes";
        private const string DOC_CREATED_NOTE = "Document Created";
        private const string DOC_UPDATED_NOTE = "Document Updated";
        private const string STATUS_UPDATED_NOTE = "Status changed from {0} to {1}";

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
                    ////////////////////////// FIX ME //////////////////////////
                    tasks = tasks.OrderByDescending(t => t.dateSubmmited);
                    ////////////////////////// FIX ME //////////////////////////
                    break;
                case "assignment_desc":
                    ////////////////////////// FIX ME //////////////////////////
                    tasks = tasks.OrderByDescending(t => t.dateSubmmited);
                    ////////////////////////// FIX ME //////////////////////////
                    break;
                default:
                    tasks = tasks.OrderByDescending(t => t.dateSubmmited);
                    break;
            }
            return View("Index", await tasks.ToListAsync());
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
        public async Task<ActionResult> Create([Bind(Include = "primaryContact,secondaryContact,dateLogged,serviceTeam,serviceGroup,platform,urgency,businessUnit,environment,category,application,references,title,description,highLevelEstimate,links")] TaskViewModel taskViewModel)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            taskViewModel.dateSubmmited = DateTime.Now;
            foreach (var item in taskViewModel.references)
            {
                if (String.IsNullOrEmpty(item.referenceNo))
                {
                    ModelState.AddModelError("references[" + taskViewModel.references.IndexOf(item) + "].referenceNo", "Reference number is required.");
                }
            }
            if (ModelState.IsValid)
            {
                Models.Task task = new Models.Task
                {
                    primaryContact = taskViewModel.primaryContact,
                    secondaryContact = taskViewModel.secondaryContact,
                    dateLogged = taskViewModel.dateLogged,
                    dateSubmmited = taskViewModel.dateSubmmited,
                    serviceTeam = taskViewModel.serviceTeam,
                    serviceGroup = taskViewModel.serviceGroup,
                    platform = taskViewModel.platform,
                    urgency = taskViewModel.urgency,
                    businessUnit = taskViewModel.businessUnit,
                    environment = taskViewModel.environment,
                    category = taskViewModel.category,
                    application = taskViewModel.application,
                    title = taskViewModel.title,
                    description = taskViewModel.description,
                    highLevelEstimate = taskViewModel.highLevelEstimate,
                    links = taskViewModel.links,
                    taskStatus = NOT_ASSIGNED_TASK_STATUS
                };
                db.Tasks.Add(task);
                await db.SaveChangesAsync();
                foreach (var item in taskViewModel.references)
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
            ViewBag.application = new SelectList(db.Applications, "applicationId", "name", taskViewModel.application);
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description", taskViewModel.businessUnit);
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode", taskViewModel.category);
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "fullName", taskViewModel.secondaryContact);
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "fullName", taskViewModel.primaryContact);
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode", taskViewModel.environment);
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode", taskViewModel.highLevelEstimate);
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode", taskViewModel.platform);
            ViewBag.refTypes = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode");
            ViewBag.ReferenceTypes = db.ReferenceTypes.ToList();
            ViewBag.serviceGroup = new SelectList(db.Groups, "groupId", "name", taskViewModel.serviceGroup);
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode", taskViewModel.urgency);
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name", taskViewModel.serviceTeam);
            ViewBag.taskStatus = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode", taskViewModel.taskStatus);
            ViewBag.dateLogged = taskViewModel.dateLogged;
            return View(taskViewModel);
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
            TaskViewModel taskViewModel = new TaskViewModel
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
                taskViewModel.references.Add(item.Reference1);
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
            return View(taskViewModel);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "taskId,primaryContact,secondaryContact,serviceTeam,serviceGroup,platform,urgency,businessUnit,environment,category,application,references,title,description,highLevelEstimate,links")] TaskViewModel taskViewModel)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            foreach (var item in taskViewModel.references)
            {
                if (String.IsNullOrEmpty(item.referenceNo))
                {
                    ModelState.AddModelError("references[" + taskViewModel.references.IndexOf(item) + "].referenceNo", "Reference number is required.");
                }
                else if (item.referenceNo.Length > 50)
                {
                    ModelState.AddModelError("references[" + taskViewModel.references.IndexOf(item) + "].referenceNo", "Reference number cannot have more than 50 characters..");
                }
            }
            if (ModelState.IsValid)
            {
                Models.Task task = await db.Tasks.FindAsync(taskViewModel.taskId);
                task.primaryContact = taskViewModel.primaryContact;
                task.secondaryContact = taskViewModel.secondaryContact;
                task.serviceTeam = taskViewModel.serviceTeam;
                task.serviceGroup = taskViewModel.serviceGroup;
                task.platform = taskViewModel.platform;
                task.urgency = taskViewModel.urgency;
                task.businessUnit = taskViewModel.businessUnit;
                task.environment = taskViewModel.environment;
                task.category = taskViewModel.category;
                task.application = taskViewModel.application;
                task.title = taskViewModel.title;
                task.description = taskViewModel.description;
                task.highLevelEstimate = taskViewModel.highLevelEstimate;
                task.links = taskViewModel.links;
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
                List<TaskReference> taskReferences = await db.TaskReferences.Where(tr => tr.task == task.taskId).ToListAsync();
                foreach (var item in taskReferences)
                {
                    db.TaskReferences.Remove(item);
                }
                await db.SaveChangesAsync();
                foreach (var item in taskViewModel.references)
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
            ViewBag.application = new SelectList(db.Applications, "applicationId", "name", taskViewModel.application);
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description", taskViewModel.businessUnit);
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode", taskViewModel.category);
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "fullName", taskViewModel.secondaryContact);
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "fullName", taskViewModel.primaryContact);
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode", taskViewModel.environment);
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode", taskViewModel.highLevelEstimate);
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode", taskViewModel.platform);
            ViewBag.refTypes = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode");
            ViewBag.ReferenceTypes = db.ReferenceTypes.ToList();
            ViewBag.serviceGroup = new SelectList(db.Groups, "groupId", "name", taskViewModel.serviceGroup);
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode", taskViewModel.urgency);
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name", taskViewModel.serviceTeam);
            ViewBag.taskStatus = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode", taskViewModel.taskStatus);
            return View(taskViewModel);
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