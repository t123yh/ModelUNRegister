using Microsoft.AspNet.Identity.Owin;
using ModelUNRegister.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace ModelUNRegister.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class EnrollListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;

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

        public async Task<ActionResult> Index(int? page, string query)
        {
            IQueryable<EnrollRequest> users = from s in db.EnrollRequests
                                              orderby s.SubmissionTime
                                              select s;

            var questionCount = await db.Questions.CountAsync();

            if (query != "*")
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    query = "EmailConfirmed";
                }
                foreach (var condition in query.Split('|'))
                {
                    switch (condition)
                    {
                        case "EmailConfirmed":
                            users = users.Where(r => r.User.EmailConfirmed);
                            break;
                        case "QuestionAnswered":
                            users = users.Where(r => r.User.Answers.Count() == questionCount);
                            break;
                        default:
                            break;
                    }
                }
            }

            ViewBag.QuestionCount = questionCount;

            int pageNum = page.HasValue ? page.Value : 1;

            return View(await users.Select(s => new EnrollListItem() { Request = s, AnswerCount = s.User.Answers.Count() }).ToPagedListAsync(pageNum, 10));
        }

        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(id.Value.ToString());

            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.QuestionCount = await db.Questions.CountAsync();

            return View(new EnrollListItem() { Request = user.EnrollRequest, AnswerCount = user.Answers.Count() });
        }

        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(EnrollViewModel.CreateFromUser(user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            await UserManager.DeleteAsync(await UserManager.FindByIdAsync(id.ToString()));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> GetAnswers(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(id.Value.ToString());

            var answers = user.Answers.Select(ans => new { title = ans.Question.Title, description = ans.Question.Description, answer = ans.AnswerContent });

            if (user == null)
            {
                return HttpNotFound();
            }

            return Json(new { answers = answers }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                if (_userManager != null)
                {
                    _userManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}