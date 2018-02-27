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

        // GET: Tasks
        public async Task<ActionResult> Index(string criterion)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            List<Models.Task> tasks;
            if (!String.IsNullOrEmpty(criterion))
            {
                criterion = criterion.ToLower();
                tasks = await db.Tasks.Include(t => t.Application1).Include(t => t.BusinessUnit1).Include(t => t.Category1).Include(t => t.Employee).Include(t => t.Employee1).Include(t => t.Environment1).Include(t => t.HighLevelEstimate1).Include(t => t.Platform1).Include(t => t.Reference1).Include(t => t.ServiceGroup1).Include(t => t.Urgency1).Include(t => t.Team).Include(t => t.TaskStatu).Where(t => t.Employee.lastName.ToLower().Contains(criterion) || t.Employee.firstName.Contains(criterion) || t.taskStatusCode.ToLower().Contains(criterion)).ToListAsync();
            }
            else
            {
                tasks = await db.Tasks.Include(t => t.Application1).Include(t => t.BusinessUnit1).Include(t => t.Category1).Include(t => t.Employee).Include(t => t.Employee1).Include(t => t.Environment1).Include(t => t.HighLevelEstimate1).Include(t => t.Platform1).Include(t => t.Reference1).Include(t => t.ServiceGroup1).Include(t => t.Urgency1).Include(t => t.Team).Include(t => t.TaskStatu).ToListAsync();
            }
            return View(tasks);
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
            return View(task);
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
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "lastName");
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "lastName");
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode");
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode");
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode");
            ViewBag.referenceType = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode");
            ViewBag.serviceGroup = new SelectList(db.ServiceGroups, "serviceGroupId", "name");
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode");
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name");
            ViewBag.taskStatusCode = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode");
            ViewBag.dateLogged = DateTime.Now;
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "taskId,primaryContact,secondaryContact,dateLogged,dateSubmmited,serviceTeam,serviceGroup,platform,urgency,businessUnit,environment,category,application,referenceNo,referenceType,title,description,highLevelEstimate,links,taskStatusCode")] TaskViewModel taskViewModel)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            taskViewModel.dateSubmmited = DateTime.Now;

            if (ModelState.IsValid)
            {
                Reference reference = new Reference();
                reference.referenceNo = taskViewModel.referenceNo;
                reference.referenceType = taskViewModel.referenceType;
                db.References.Add(reference);
                await db.SaveChangesAsync();
                Models.Task task = new Models.Task();
                task.primaryContact = taskViewModel.primaryContact;
                task.secondaryContact = taskViewModel.secondaryContact;
                task.dateLogged = taskViewModel.dateLogged;
                task.dateSubmmited = taskViewModel.dateSubmmited;
                task.serviceTeam = taskViewModel.serviceTeam;
                task.serviceGroup = taskViewModel.serviceGroup;
                task.platform = taskViewModel.platform;
                task.urgency = taskViewModel.urgency;
                task.businessUnit = taskViewModel.businessUnit;
                task.environment = taskViewModel.environment;
                task.category = taskViewModel.category;
                task.application = taskViewModel.application;
                task.reference = reference.referenceNo;
                task.title = taskViewModel.title;
                task.description = taskViewModel.description;
                task.highLevelEstimate = taskViewModel.highLevelEstimate;
                task.links = taskViewModel.links;
                task.taskStatusCode = taskViewModel.taskStatusCode;
                db.Tasks.Add(task);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.application = new SelectList(db.Applications, "applicationId", "name", taskViewModel.application);
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description", taskViewModel.businessUnit);
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode", taskViewModel.category);
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "lastName", taskViewModel.secondaryContact);
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "lastName", taskViewModel.primaryContact);
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode", taskViewModel.environment);
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode", taskViewModel.highLevelEstimate);
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode", taskViewModel.platform);
            ViewBag.referenceType = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode", taskViewModel.referenceType);
            ViewBag.serviceGroup = new SelectList(db.ServiceGroups, "serviceGroupId", "name", taskViewModel.serviceGroup);
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode", taskViewModel.urgency);
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name", taskViewModel.serviceTeam);
            ViewBag.taskStatusCode = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode", taskViewModel.taskStatusCode);
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
            TaskViewModel taskViewModel = new TaskViewModel();
            taskViewModel.taskId = task.taskId;
            taskViewModel.primaryContact = task.primaryContact;
            taskViewModel.secondaryContact = task.secondaryContact;
            taskViewModel.dateLogged = task.dateLogged;
            taskViewModel.dateSubmmited = task.dateSubmmited;
            taskViewModel.serviceTeam = task.serviceTeam;
            taskViewModel.serviceGroup = task.serviceGroup;
            taskViewModel.platform = task.platform;
            taskViewModel.urgency = task.urgency;
            taskViewModel.businessUnit = task.businessUnit;
            taskViewModel.environment = task.environment;
            taskViewModel.category = task.category;
            taskViewModel.application = task.application;
            taskViewModel.referenceNo = task.Reference1.referenceNo;
            taskViewModel.referenceType = task.Reference1.referenceType;
            taskViewModel.title = task.title;
            taskViewModel.description = task.description;
            taskViewModel.highLevelEstimate = task.highLevelEstimate;
            taskViewModel.links = task.links;
            taskViewModel.taskStatusCode = task.taskStatusCode;

            ViewBag.application = new SelectList(db.Applications, "applicationId", "name", task.application);
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description", task.businessUnit);
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode", task.category);
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "lastName", task.secondaryContact);
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "lastName", task.primaryContact);
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode", task.environment);
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode", task.highLevelEstimate);
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode", task.platform);
            ViewBag.referenceType = new SelectList(db.ReferenceTypes, "referenceTypeCode", "referenceTypeCode", taskViewModel.referenceType);
            ViewBag.serviceGroup = new SelectList(db.ServiceGroups, "serviceGroupId", "name", task.serviceGroup);
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode", task.urgency);
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name", task.serviceTeam);
            ViewBag.taskStatusCode = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode", task.taskStatusCode);
            return View(taskViewModel);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "taskId,primaryContact,secondaryContact,dateLogged,dateSubmmited,serviceTeam,serviceGroup,platform,urgency,businessUnit,environment,category,application,referenceNo,referenceType,title,description,highLevelEstimate,links,taskStatusCode")] TaskViewModel taskViewModel)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (ModelState.IsValid)
            {
                Reference reference = await db.References.FindAsync(taskViewModel.referenceNo);
                reference.referenceType = taskViewModel.referenceType;
                db.Entry(reference).State = EntityState.Modified;
                await db.SaveChangesAsync();
                Models.Task task = await db.Tasks.FindAsync(taskViewModel.taskId);
                task.primaryContact = taskViewModel.primaryContact;
                task.secondaryContact = taskViewModel.secondaryContact;
                task.dateLogged = taskViewModel.dateLogged;
                task.dateSubmmited = taskViewModel.dateSubmmited;
                task.serviceTeam = taskViewModel.serviceTeam;
                task.serviceGroup = taskViewModel.serviceGroup;
                task.platform = taskViewModel.platform;
                task.urgency = taskViewModel.urgency;
                task.businessUnit = taskViewModel.businessUnit;
                task.environment = taskViewModel.environment;
                task.category = taskViewModel.category;
                task.application = taskViewModel.application;
                task.reference = reference.referenceNo;
                task.title = taskViewModel.title;
                task.description = taskViewModel.description;
                task.highLevelEstimate = taskViewModel.highLevelEstimate;
                task.links = taskViewModel.links;
                task.taskStatusCode = taskViewModel.taskStatusCode;
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.application = new SelectList(db.Applications, "applicationId", "name", taskViewModel.application);
            ViewBag.businessUnit = new SelectList(db.BusinessUnits, "businessUnitId", "description", taskViewModel.businessUnit);
            ViewBag.category = new SelectList(db.Categories, "categoryCode", "categoryCode", taskViewModel.category);
            ViewBag.secondaryContact = new SelectList(db.Employees, "employeeId", "lastName", taskViewModel.secondaryContact);
            ViewBag.primaryContact = new SelectList(db.Employees, "employeeId", "lastName", taskViewModel.primaryContact);
            ViewBag.environment = new SelectList(db.Environments, "environmentCode", "environmentCode", taskViewModel.environment);
            ViewBag.highLevelEstimate = new SelectList(db.HighLevelEstimates, "highLevelEstimateCode", "highLevelEstimateCode", taskViewModel.highLevelEstimate);
            ViewBag.platform = new SelectList(db.Platforms, "platformCode", "platformCode", taskViewModel.platform);
            ViewBag.reference = new SelectList(db.References, "referenceNo", "referenceType", taskViewModel.referenceType);
            ViewBag.serviceGroup = new SelectList(db.ServiceGroups, "serviceGroupId", "name", taskViewModel.serviceGroup);
            ViewBag.urgency = new SelectList(db.Urgencies, "urgencyCode", "urgencyCode", taskViewModel.urgency);
            ViewBag.serviceTeam = new SelectList(db.Teams, "teamId", "name", taskViewModel.serviceTeam);
            ViewBag.taskStatusCode = new SelectList(db.TaskStatus, "taskStatusCode", "taskStatusCode", taskViewModel.taskStatusCode);
            return View(taskViewModel);
        }

        // GET: Tasks/Delete/5
        public async Task<ActionResult> Delete(int? id)
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

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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