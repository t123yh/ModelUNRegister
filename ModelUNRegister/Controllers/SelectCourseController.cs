using Microsoft.AspNet.Identity.Owin;
using ModelUNRegister.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ModelUNRegister.Utilities;
using System.Net;

namespace ModelUNRegister.Controllers
{
    [Authorize]
    public class SelectCourseController : Controller
    {
        private ApplicationUserManager _userManager;

        private ApplicationDbContext _db = new ApplicationDbContext();

        public ApplicationDbContext DbContext
        {
            get
            {
                return _db;
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
        public async Task<ActionResult> Index(bool? success)
        {
            var courses = await DbContext.Courses.ToArrayAsync();
            var selected = (await UserManager.FindByIdAsync(User.Identity.Name)).Courses.Select(c => c.Id);
            return View(new CourseSelectionViewModel() { Courses = courses, SelectedCourses = selected });
        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(CourseSelectionViewModel model)
        {
            if (model.SelectedCourses == null)
            {
                model.SelectedCourses = new Guid[0];
            }
            if (model.SelectedCourses.Count() > 4)
            {
                ModelState.AddModelError("SelectedCourses", "你最多只能选择 4 门课程。");
                model.Courses = await DbContext.Courses.ToArrayAsync();
                return View(model);
            }
            else
            {
                var user = await DbContext.Users.Where(u => u.UserName == User.Identity.Name).FirstAsync();
                user.Courses.Clear();
                foreach (var courseId in model.SelectedCourses)
                {
                    var courseToAdd = DbContext.Courses.First(c => c.Id == courseId);
                    user.Courses.Add(courseToAdd);
                }
                await DbContext.SaveChangesAsync();
                Session["message"] = "已保存你的设置。";
                return RedirectToAction("Index");
            }

        }

        public async Task<ActionResult> CourseDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = await DbContext.Courses.FindAsync(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return PartialView(course);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}