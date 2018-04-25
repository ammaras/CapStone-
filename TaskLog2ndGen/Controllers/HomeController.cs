using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Models;
using System.Linq;
using System.Collections.Generic;
using TaskLog2ndGen.ViewModels;

namespace TaskLog2ndGen.Controllers
{
    /// <summary>
    /// Handles get http requests for retrieving all tasks
    /// </summary>
    public class HomeController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();

        /// <summary>
        /// Handles get requests, and retrieves, filters and sorts all tasks
        /// </summary>
        /// <param name="searchCriterion">Criterion to filter all tasks</param>
        /// <param name="sortCriterion">Criterion to sort all tasks</param>
        /// <returns>View displaying all tasks, filtered or sorted</returns>
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
                ViewBag.timeSpentSortCriterion = "timeSpent";
            }
            else
            {
                ViewBag.statusSortCriterion = sortCriterion == "status" ? "status_desc" : "status";
                ViewBag.teamSortCriterion = sortCriterion == "team" ? "team_desc" : "team";
                ViewBag.assignmentSortCriterion = sortCriterion == "assignment" ? "assignment_desc" : "assignment";
                ViewBag.timeSpentSortCriterion = sortCriterion == "timeSpent" ? "timeSpent_desc" : "timeSpent";
            }
            var tasks = db.Tasks.Include(t => t.Application1).Include(t => t.BusinessUnit1).Include(t => t.Category1).Include(t => t.Employee).Include(t => t.Employee1).Include(t => t.Environment1).Include(t => t.Group).Include(t => t.HighLevelEstimate1).Include(t => t.Platform1).Include(t => t.TaskStatu).Include(t => t.Urgency1).Include(t => t.Team);
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
            if (!String.IsNullOrEmpty(searchCriterion))
            {
                searchCriterion = searchCriterion.ToLower();
                taskViewModels = taskViewModels.Where(t => t.task.Employee.firstName.ToLower().Contains(searchCriterion)
                                    || t.task.Employee.lastName.ToLower().Contains(searchCriterion)
                                    || t.task.Employee1.firstName.ToLower().Contains(searchCriterion)
                                    || t.task.Employee.lastName.ToLower().Contains(searchCriterion)
                                    || t.task.Team.name.ToLower().Contains(searchCriterion)
                                    || t.task.Group.name.ToLower().Contains(searchCriterion)
                                    || t.task.platform.ToLower().Contains(searchCriterion)
                                    || t.task.urgency.ToLower().Contains(searchCriterion)
                                    || t.task.BusinessUnit1.description.ToLower().Contains(searchCriterion)
                                    || t.task.environment.ToLower().Contains(searchCriterion)
                                    || t.task.category.ToLower().Contains(searchCriterion)
                                    || t.task.Application1.name.ToLower().Contains(searchCriterion)
                                    || t.task.title.ToLower().Contains(searchCriterion)
                                    || t.task.description.ToLower().Contains(searchCriterion)
                                    || t.task.highLevelEstimate.ToLower().Contains(searchCriterion)
                                    || t.task.links.ToLower().Contains(searchCriterion)
                                    || t.task.taskStatus.ToLower().Contains(searchCriterion)
                                    || t.totalTimeSpent.ToString().ToLower().Contains(searchCriterion)
                                    || t.assignedEmployees.ToLower().Contains(searchCriterion)).ToList();
            }
            switch (sortCriterion)
            {
                case "status":
                    taskViewModels = taskViewModels.OrderBy(t => t.task.taskStatus).ThenByDescending(t => t.task.dateSubmmited).ToList();
                    break;
                case "status_desc":
                    taskViewModels = taskViewModels.OrderByDescending(t => t.task.taskStatus).ThenByDescending(t => t.task.dateSubmmited).ToList();
                    break;
                case "team":
                    taskViewModels = taskViewModels.OrderBy(t => t.task.serviceTeam).ThenByDescending(t => t.task.dateSubmmited).ToList();
                    break;
                case "team_desc":
                    taskViewModels = taskViewModels.OrderByDescending(t => t.task.serviceTeam).ThenByDescending(t => t.task.dateSubmmited).ToList();
                    break;
                case "assignment":
                    taskViewModels = taskViewModels.OrderBy(t => t.assignedEmployees).ThenByDescending(t => t.task.dateSubmmited).ToList();
                    break;
                case "assignment_desc":
                    taskViewModels = taskViewModels.OrderByDescending(t => t.assignedEmployees).ThenByDescending(t => t.task.dateSubmmited).ToList();
                    break;
                case "timeSpent":
                    taskViewModels = taskViewModels.OrderBy(t => t.totalTimeSpent).ThenByDescending(t => t.task.dateSubmmited).ToList();
                    break;
                case "timeSpent_desc":
                    taskViewModels = taskViewModels.OrderByDescending(t => t.totalTimeSpent).ThenByDescending(t => t.task.dateSubmmited).ToList();
                    break;
                default:
                    taskViewModels = taskViewModels.OrderByDescending(t => t.task.dateSubmmited).ToList();
                    break;
            }
            return View("Index", taskViewModels);
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