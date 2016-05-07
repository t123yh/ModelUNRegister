using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ModelUNRegister.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class SystemManagementController : Controller
    {
        // GET: SystemManagement
        public ActionResult Index()
        {
            return View();
        }
    }
}