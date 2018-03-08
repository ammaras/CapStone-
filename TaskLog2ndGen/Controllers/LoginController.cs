using System;
using System.Data;
using System.Data.Entity;
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
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
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
                    Account accnt = await db.Accounts.Where(a => a.username == account.username).SingleOrDefaultAsync();
                    if (accnt == null)
                    {
                        ModelState.AddModelError("password", "Username or password are invalid.");
                        return View("Index", account);
                    }
                    if (accnt.password != account.password)
                    {
                        ModelState.AddModelError("password", "Username or password are invalid.");
                    }
                    else
                    {
                        if (System.Web.HttpContext.Current != null)
                        {
                            Session["account"] = accnt;
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    return View("Index", account);
                }
                catch (Exception err)
                {
                    TempData["errMsg"] = "An unexpected error has occurred. Please try again later.";
                }
            }
            return View("Index", account);
        }

    }
}