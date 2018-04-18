using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.Controllers
{
    public class EmployeesController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();

        // GET: Employees
        public async Task<ActionResult> Index()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode == "Employee")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            var employees = db.Employees.Include(e => e.Account).Include(e => e.Team1);
            return View("Index", await employees.ToListAsync());
        }

        // GET: Employees/Details/5
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
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View("Details", employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            ViewBag.employeeId = new SelectList(db.Accounts, "employeeId", "userName");
            ViewBag.team = new SelectList(db.Teams, "teamId", "name");
            return View();
        }

        // POST: Employees/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "employeeId,team,lastName,firstName,email,description,middleName,phone,extension")] Employee employee)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            employee.lastChanged = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                await db.SaveChangesAsync();
                Account account = new Account()
                {
                    employeeId = employee.employeeId,
                    username = (employee.firstName.ElementAt(0) + employee.lastName).ToLower(),
                    password = (employee.firstName.ElementAt(0) + employee.lastName).ToLower(),
                    roleCode = "Employee"
                };
                db.Accounts.Add(account);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.employeeId = new SelectList(db.Accounts, "employeeId", "userName", employee.employeeId);
            ViewBag.team = new SelectList(db.Teams, "teamId", "name", employee.team);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.employeeId = new SelectList(db.Accounts, "employeeId", "userName", employee.employeeId);
            ViewBag.team = new SelectList(db.Teams, "teamId", "name", employee.team);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "employeeId,team,lastName,firstName,email,description,middleName,phone,extension")] Employee employee)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            employee.lastChanged = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = employee.employeeId });
            }
            ViewBag.employeeId = new SelectList(db.Accounts, "employeeId", "userName", employee.employeeId);
            ViewBag.team = new SelectList(db.Teams, "teamId", "name", employee.team);
            return View(employee);
        }

        // GET: Employees/Delete/5
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
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
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
            Employee employee = await db.Employees.FindAsync(id);
            List<Models.Task> tasks = await db.Tasks.Where(t => t.primaryContact == employee.employeeId || t.secondaryContact == employee.employeeId).ToListAsync();
            foreach (var task in tasks)
            {
                List<TaskReference> taskReferences = await db.TaskReferences.Where(tr => tr.task == task.taskId).ToListAsync();
                foreach (var taskReference in taskReferences)
                {
                    db.TaskReferences.Remove(taskReference);
                }
                List<TaskAudit> taskAudits = await db.TaskAudits.Where(ta => ta.task == task.taskId).ToListAsync();
                foreach (var taskAudit in taskAudits)
                {
                    db.TaskAudits.Remove(taskAudit);
                }
                List<Worksheet> worksheets = await db.Worksheets.Where(ws => ws.task == task.taskId).ToListAsync();
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
            }
            List<Worksheet> worksheets2 = await db.Worksheets.Where(ws => ws.employee == employee.employeeId).ToListAsync();
            foreach (var worksheet in worksheets2)
            {
                List<WorksheetAudit> worksheetAudits = await db.WorksheetAudits.Where(wa => wa.worksheet == worksheet.worksheetId).ToListAsync();
                foreach (var worksheetAudit in worksheetAudits)
                {
                    db.WorksheetAudits.Remove(worksheetAudit);
                }
                db.Worksheets.Remove(worksheet);
            }
            if (employee.Account != null)
            {
                db.Accounts.Remove(employee.Account);
            }
            db.Employees.Remove(employee);
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