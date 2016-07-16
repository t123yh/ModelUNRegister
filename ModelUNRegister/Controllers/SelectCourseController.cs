using Microsoft.AspNet.Identity.Owin;
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
    [Authorize]
    public class SelectCourseController : Controller
    {
        private ApplicationUserManager _userManager;

        private ApplicationDbContext _db;

        public ApplicationDbContext DbContext
        {
            get
            {
                return _db ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: SelectCourse
        public async Task<ActionResult> Index()
        {

            var courses = await DbContext.Courses.ToArrayAsync();
            var selected = (await UserManager.FindByIdAsync(User.Identity.Name)).Courses.Select(c => c.Id);
            return View(new CourseSelectionViewModel() { Courses = courses, SelectedCourses = selected });
        }
    }
}