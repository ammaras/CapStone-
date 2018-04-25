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
    /// <summary>
    /// Handles get and post http requests for CRUD operations on team table
    /// </summary>
    public class TeamsController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();

        /// <summary>
        /// Handles get request and retrieves all teams
        /// </summary>
        /// <returns>View displaying all teams</returns>
        public async Task<ActionResult> Index()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            return View("Index", await db.Teams.ToListAsync());
        }

        /// <summary>
        /// Handles get request and retrieves all employees for a team
        /// </summary>
        /// <param name="id">Id of team for wich to retrieve all employees</param>
        /// <returns>View displaying retrieved employees for a team</returns>
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
            List<Employee> employees = await db.Employees.Where(e => e.Team1.teamId == id).ToListAsync();
            if (employees == null)
            {
                return HttpNotFound();
            }
            ViewBag.teamId = id;
            return View("Details", employees);
        }

        /// <summary>
        /// Handles get request and sets up form to create team
        /// </summary>
        /// <returns>View displaying form to create team</returns>
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
            return View("Create");
        }

        /// <summary>
        /// Handles post request and creates team
        /// </summary>
        /// <param name="team">Team to create</param>
        /// <returns>Redirection to TeamsController.Details action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "teamId,name")] Team team)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            if (ModelState.IsValid)
            {
                db.Teams.Add(team);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = team.teamId });
            }
            return View(team);
        }

        /// <summary>
        /// Handles get request and sets up form to edit team
        /// </summary>
        /// <param name="id">Id of team to edit</param>
        /// <returns>View displaying form to edit team</returns>
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
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        /// <summary>
        /// Handles post request and edits team
        /// </summary>
        /// <param name="team">Team to edit</param>
        /// <returns>Redirection to TeamsController.Details action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "teamId,name")] Team team)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = team.teamId });
            }
            return View(team);
        }

        /// <summary>
        /// Handles get request and asks for confirmation to delete team
        /// </summary>
        /// <param name="id">Id of team to delete</param>
        /// <returns>View displaying team to delete and asking for confirmation</returns>
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
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        /// <summary>
        /// Handles post request and deletes team
        /// </summary>
        /// <param name="id">Id of team to delete</param>
        /// <returns>Redirection to TeamsController.Index action</returns>
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
            Team team = await db.Teams.FindAsync(id);
            List<Employee> employees = await db.Employees.Where(e => e.team == team.teamId).ToListAsync();
            foreach (var employee in employees)
            {
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
            }
            db.Teams.Remove(team);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles get request and sets up form to add an employee to team
        /// </summary>
        /// <param name="id">Id of team to which to add an employee</param>
        /// <returns>View displaying form to add an employee to team</returns>
        public async Task<ActionResult> AddEmployee(int? id)
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
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            List<Employee> employees = await db.Employees.ToListAsync();
            employees.RemoveAll(e => team.Employees.Contains(e));
            ViewBag.employees = new SelectList(employees.OrderBy(e => e.lastName), "employeeId", "fullName");
            return View(team);
        }

        /// <summary>
        /// Handles post request and adds an employee to team
        /// </summary>
        /// <param name="teamId">Id of team to which to add an employee</param>
        /// <param name="employees">Id of employee to add to team</param>
        /// <returns>Redirection to TeamsController.Details action</returns>
        [HttpPost, ActionName("AddEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEmployeeConfirmed(int? teamId, int? employees)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            if (teamId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (employees == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(employees);
            if (employee == null)
            {
                return HttpNotFound();
            }
            employee.team = (int)teamId;
            employee.lastChanged = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = teamId });
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