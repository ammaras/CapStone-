using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Models;
using System.Linq;

namespace TaskLog2ndGen.Controllers
{
    public class HomeController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();

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
    }
}