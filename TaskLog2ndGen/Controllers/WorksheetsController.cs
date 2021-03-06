﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.Controllers
{
    /// <summary>
    /// Handles get and post http requests for CRUD operations on worksheet table
    /// </summary>
    public class WorksheetsController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();
        private const string ASSIGNED_WORKSHEET_STATUS = "Assigned";
        private const string DOC_CREATED_NOTE = "Document Created";
        private const string DOC_UPDATED_NOTE = "Document Updated";
        private const string ASSIGNED_TASK_STATUS = "Assigned";
        private const string COMM_TASK_AUDIT_TYPE = "Communication";
        private const string STATUS_UPDATED_NOTE = "Status changed from {0} to {1}";

        /// <summary>
        /// Handles get request and retrieves all worksheets for a task
        /// </summary>
        /// <param name="id">Id of task for which to retrieve all worksheets</param>
        /// <returns>View displaying all worksheets for a task</returns>
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            var worksheets = db.Worksheets.Where(w => w.task == id).Include(w => w.Employee1).Include(w => w.Task1).Include(w => w.WorksheetStatu);
            if (worksheets == null)
            {
                return HttpNotFound();
            }
            ViewBag.taskStatus = task.taskStatus;
            ViewBag.task = id;
            return View("Index", await worksheets.ToListAsync());
        }

        /// <summary>
        /// Handles get request and retrieves a worksheet
        /// </summary>
        /// <param name="id">Id of worksheet to retrieve</param>
        /// <returns>View displaying retrieved worksheet</returns>
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
            Worksheet worksheet = await db.Worksheets.FindAsync(id);
            if (worksheet == null)
            {
                return HttpNotFound();
            }
            return View("Details", worksheet);
        }

        /// <summary>
        /// Handles get request and sets up form to create worksheet for a task
        /// </summary>
        /// <param name="id">Id of task for which to create worksheet</param>
        /// <returns>View displaying form to create worksheet for a task</returns>
        public async Task<ActionResult> Create(int? id)
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
            ViewBag.employee = new SelectList(db.Employees, "employeeId", "fullName");
            ViewBag.task = id;
            return View();
        }

        /// <summary>
        /// Handles post request and creates worksheet
        /// </summary>
        /// <param name="worksheet">Worksheet to create</param>
        /// <returns>Redirection to WorksheetsController.Details action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "employee,task,notes,timeSpent,overtime,onCall,links")] Worksheet worksheet)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            worksheet.dateAssigned = DateTime.Now;
            worksheet.worksheetStatus = ASSIGNED_WORKSHEET_STATUS;
            if (ModelState.IsValid)
            {
                db.Worksheets.Add(worksheet);
                await db.SaveChangesAsync();
                WorksheetAudit worksheetAudit = new WorksheetAudit
                {
                    worksheet = worksheet.worksheetId,
                    dateLogged = worksheet.dateAssigned,
                    loggedBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1,
                    notes = DOC_CREATED_NOTE,
                    timeSpent = worksheet.timeSpent,
                    worksheetStatus = worksheet.worksheetStatus
                };
                db.WorksheetAudits.Add(worksheetAudit);
                await db.SaveChangesAsync();
                Models.Task task = await db.Tasks.Where(t => t.taskId == worksheet.task).SingleOrDefaultAsync();
                if (task.taskStatus != ASSIGNED_TASK_STATUS)
                {
                    TaskAudit taskAudit = new TaskAudit
                    {
                        task = task.taskId,
                        taskAuditType = COMM_TASK_AUDIT_TYPE,
                        dateLogged = DateTime.Now,
                        loggedBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1,
                        notes = String.Format(STATUS_UPDATED_NOTE, task.taskStatus, ASSIGNED_TASK_STATUS),
                        taskStatus = task.taskStatus
                    };
                    task.taskStatus = ASSIGNED_TASK_STATUS;
                    db.Entry(task).State = EntityState.Modified;
                    db.TaskAudits.Add(taskAudit);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = worksheet.worksheetId });
            }
            ViewBag.employee = new SelectList(db.Employees, "employeeId", "fullName", worksheet.employee);
            ViewBag.task = worksheet.task;
            return View(worksheet);
        }

        /// <summary>
        /// Handles get request and sets up form to edit worksheet
        /// </summary>
        /// <param name="id">Id of worksheet to edit</param>
        /// <returns>View displaying form to edit worksheet</returns>
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
            Worksheet worksheet = await db.Worksheets.FindAsync(id);
            if (worksheet == null)
            {
                return HttpNotFound();
            }
            ViewBag.employee = new SelectList(db.Employees, "employeeId", "fullName", worksheet.employee);
            ViewBag.worksheetStatus = new SelectList(db.WorksheetStatus, "worksheetStatusCode", "worksheetStatusCode", worksheet.worksheetStatus);
            return View(worksheet);
        }

        /// <summary>
        /// Handles post request and edits worksheet
        /// </summary>
        /// <param name="worksheet">Worksheet to edit</param>
        /// <returns>Redirection to WorksheetsController.Details action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "worksheetId,employee,task,worksheetStatus,dateAssigned,notes,timeSpent,overtime,onCall,links")] Worksheet worksheet)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (ModelState.IsValid)
            {
                db.Entry(worksheet).State = EntityState.Modified;
                await db.SaveChangesAsync();
                WorksheetAudit worksheetAudit = new WorksheetAudit
                {
                    worksheet = worksheet.worksheetId,
                    dateLogged = DateTime.Now,
                    loggedBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1,
                    notes = DOC_UPDATED_NOTE,
                    timeSpent = worksheet.timeSpent,
                    worksheetStatus = worksheet.worksheetStatus
                };
                db.WorksheetAudits.Add(worksheetAudit);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = worksheet.worksheetId });
            }
            ViewBag.employee = new SelectList(db.Employees, "employeeId", "lastName", worksheet.employee);
            ViewBag.task = new SelectList(db.Tasks, "taskId", "platform", worksheet.task);
            ViewBag.worksheetStatus = new SelectList(db.WorksheetStatus, "worksheetStatusCode", "worksheetStatusCode", worksheet.worksheetStatus);
            return View(worksheet);
        }

        /// <summary>
        /// Handles get request and asks for confirmation to delete worksheet
        /// </summary>
        /// <param name="id">Id of worksheet to delete</param>
        /// <returns>View displaying worksheet to delete and asking for confirmation</returns>
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
            Worksheet worksheet = await db.Worksheets.FindAsync(id);
            if (worksheet == null)
            {
                return HttpNotFound();
            }
            return View(worksheet);
        }

        /// <summary>
        /// Handles post request and deletes worksheet
        /// </summary>
        /// <param name="id">Id of worksheet to delete</param>
        /// <returns>Redirection to WorksheetsController.Index action</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            List<WorksheetAudit> worksheetAudits = await db.WorksheetAudits.Where(wa => wa.worksheet == id).ToListAsync();
            foreach (var item in worksheetAudits)
            {
                db.WorksheetAudits.Remove(item);
            }
            Worksheet worksheet = await db.Worksheets.FindAsync(id);
            db.Worksheets.Remove(worksheet);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { id = worksheet.task });
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