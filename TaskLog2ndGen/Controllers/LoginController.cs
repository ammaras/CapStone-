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
    /// <summary>
    /// Handles get and post http requests for logging in
    /// </summary>
    public class LoginController : Controller
    {
        private GB_Tasklogtracker_D1Context db = new GB_Tasklogtracker_D1Context();

        /// <summary>
        /// Handles get request and sets up form to log user in
        /// </summary>
        /// <returns>View displaying form to log user in</returns>
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current != null && Session["account"] == null)
            {
                return View();
            }
            return RedirectToAction("", "Home");
        }

        /// <summary>
        /// Handles post request and logs user in
        /// </summary>
        /// <param name="account">Account to log in</param>
        /// <returns>Redirection to HomeController.Index action</returns>
        [HttpPost]
        public async Task<ActionResult> Index([Bind(Include = "userName,password")] AccountViewModel account)
        {
            if (ModelState.IsValid)
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
            return View("Index", account);
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