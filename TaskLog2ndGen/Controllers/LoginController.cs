using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskLog2ndGen.Models;
using TaskLog2ndGen.ViewModels;

namespace TaskLog2ndGen.Controllers
{
    public class LoginController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();

        // GET: Login
        public async Task<ActionResult> Index()
        {
            if (Session["account"] == null)
            {
                return View();
            }
            return RedirectToAction("", "Home");
        }

        // POST: Login
        [HttpPost]
        public async Task<ActionResult> Index([Bind(Include = "userName,password")] AccountViewModel account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Account accnt = db.Accounts.Where(a => a.userName == account.userName).SingleOrDefault();
                    if (accnt == null)
                    {
                        ModelState.AddModelError("password", "Username or password are invalid.");
                        return View(account);
                    }
                    if (accnt.password != account.password)
                    {
                        ModelState.AddModelError("password", "Username or password are invalid.");
                    }
                    else
                    {
                        Session["account"] = accnt;
                        return RedirectToAction("", "Home");
                    }
                    return View(account);
                }
                catch (Exception err)
                {
                    TempData["errMsg"] = "An unexpected error has occurred. Please try again later.";
                }
            }
            return View(account);
        }

    }
}