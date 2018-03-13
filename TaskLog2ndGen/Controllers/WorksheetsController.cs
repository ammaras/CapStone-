using System;
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
    public class WorksheetsController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();
        private const string ASSIGNED_WORKSHEET_STATUS = "Assigned";
        private const string DOC_CREATED_NOTE = "Document Created";
        private const string DOC_UPDATED_NOTE = "Document Updated";

        // GET: Worksheets
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var worksheets = db.Worksheets.Where(w => w.task == id).Include(w => w.Employee1).Include(w => w.Task1).Include(w => w.WorksheetStatu);
            if (worksheets == null)
            {
                return HttpNotFound();
            }
            ViewBag.task = id;
            return View("Index", await worksheets.ToListAsync());
        }

        // GET: Worksheets/Details/5
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

        // GET: Worksheets/Create
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

        // POST: Worksheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
                return RedirectToAction("Details", new { id = worksheet.worksheetId });
            }
            ViewBag.employee = new SelectList(db.Employees, "employeeId", "fullName", worksheet.employee);
            ViewBag.task = worksheet.task;
            return View(worksheet);
        }

        // GET: Worksheets/Edit/5
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

        // POST: Worksheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Worksheets/Delete/5
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

        // POST: Worksheets/Delete/5
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