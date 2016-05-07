using ModelUNRegister.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ModelUNRegister.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class SystemManagementController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SystemManagement
        public async Task<ActionResult> Index()
        {
            ViewBag.RequestCount = await db.EnrollRequests.CountAsync();
            return View();
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