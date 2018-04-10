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
    public class ReportsController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();

        // GET: Reports
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            ViewBag.Today = DateTime.Today;
            return View();
        }

        // POST: Reports/Admin
        public async Task<ActionResult> Admin(DateTime? startDate1, DateTime? endDate1)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode != "Admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            if (startDate1 == null)
            {
                TempData["startDate1Error"] = "Start date is required.";
                return RedirectToAction("");
            }
            if (endDate1 == null)
            {
                TempData["endDate1Error"] = "End date is required.";
                return RedirectToAction("");
            }
            if (startDate1 > endDate1)
            {
                TempData["startDate1Error"] = "Start date cannot be more recent than end date.";
                return RedirectToAction("");
            }
            ViewBag.startDate = startDate1;
            ViewBag.endDate = endDate1;
            ViewBag.createdBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).Employee.fullName : "Jackson, Peter";
            var employees = await db.Employees.Where(e => e.lastChanged >= startDate1 && e.lastChanged <= endDate1).ToListAsync();
            return View("Admin", employees);
        }

        // POST: Reports/EmployeesTime
        public async Task<ActionResult> EmployeesTime(DateTime? startDate2, DateTime? endDate2)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (System.Web.HttpContext.Current != null && (Session["account"] as Account).roleCode == "Employee")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            if (startDate2 == null)
            {
                TempData["startDate2Error"] = "Start date is required.";
                return RedirectToAction("");
            }
            if (endDate2 == null)
            {
                TempData["endDate2Error"] = "End date is required.";
                return RedirectToAction("");
            }
            if (startDate2 > endDate2)
            {
                TempData["startDate2Error"] = "Start date cannot be more recent than end date.";
                return RedirectToAction("");
            }
            ViewBag.startDate = startDate2;
            ViewBag.endDate = endDate2;
            ViewBag.createdBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).Employee.fullName : "Jackson, Peter";
            var employees = await db.Employees.OrderByDescending(e => e.lastName).ToListAsync();
            var tasks = await db.Tasks.OrderByDescending(t => t.dateSubmmited).ToListAsync();
            var employeesTimes = new List<EmployeesTimeViewModel>();
            foreach (var employee in employees)
            {
                var employeesTime = new EmployeesTimeViewModel()
                {
                    employee = employee
                };
                var employeeTimes = new List<EmployeeTimeViewModel>();
                foreach (var task in tasks)
                {
                    decimal regularTime = 0;
                    decimal overTime = 0;
                    var employeeTime = new EmployeeTimeViewModel();
                    var worksheets = task.Worksheets.Where(ws => ws.employee == (System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1) && ws.dateAssigned >= startDate2 && ws.dateAssigned <= endDate2);
                    for (int i = 0; i < worksheets.Count(); i++)
                    {
                        if (i == 0)
                        {
                            employeeTime.task = task;
                        }
                        if (worksheets.ElementAt(i).overtime)
                        {
                            overTime += worksheets.ElementAt(i).timeSpent;
                        }
                        else
                        {
                            regularTime += worksheets.ElementAt(i).timeSpent;
                        }
                        if (i == worksheets.Count() - 1)
                        {
                            employeeTime.regularTime = regularTime;
                            employeeTime.overTime = overTime;
                            employeeTimes.Add(employeeTime);
                        }
                    }
                }
                employeesTime.employeeTimes = employeeTimes;
                employeesTimes.Add(employeesTime);
            }
            return View("EmployeesTime", employeesTimes);
        }

        // POST: Reports/EmployeeTime"
        public async Task<ActionResult> EmployeeTime(DateTime? startDate3, DateTime? endDate3)
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return RedirectToAction("", "Login");
            }
            if (startDate3 == null)
            {
                TempData["startDate3Error"] = "Start date is required.";
                return RedirectToAction("");
            }
            if (endDate3 == null)
            {
                TempData["endDate3Error"] = "End date is required.";
                return RedirectToAction("");
            }
            if (startDate3 > endDate3)
            {
                TempData["startDate3Error"] = "Start date cannot be more recent than end date.";
                return RedirectToAction("");
            }
            ViewBag.startDate = startDate3;
            ViewBag.endDate = endDate3;
            ViewBag.createdBy = System.Web.HttpContext.Current != null ? (Session["account"] as Account).Employee.fullName : "Jackson, Peter";
            var tasks = await db.Tasks.OrderByDescending(t => t.dateSubmmited).ToListAsync();
            var employeeTimes = new List<EmployeeTimeViewModel>();
            foreach (var task in tasks)
            {
                decimal regularTime = 0;
                decimal overTime = 0;
                var employeeTime = new EmployeeTimeViewModel();
                var worksheets = task.Worksheets.Where(ws => ws.employee == (System.Web.HttpContext.Current != null ? (Session["account"] as Account).employeeId : 1) && ws.dateAssigned >= startDate3 && ws.dateAssigned <= endDate3);
                for (int i = 0; i < worksheets.Count(); i++)
                {
                    if (i == 0)
                    {
                        employeeTime.task = task;
                    }
                    if (worksheets.ElementAt(i).overtime)
                    {
                        overTime += worksheets.ElementAt(i).timeSpent;
                    }
                    else
                    {
                        regularTime += worksheets.ElementAt(i).timeSpent;
                    }
                    if (i == worksheets.Count() - 1)
                    {
                        employeeTime.regularTime = regularTime;
                        employeeTime.overTime = overTime;
                        employeeTimes.Add(employeeTime);
                    }
                }
            }
            return View("EmployeeTime", employeeTimes);
        }
    }
}