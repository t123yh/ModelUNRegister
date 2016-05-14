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

namespace ModelUNRegister.Controllers
{
    public class EnrollListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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

            EnrollRequest req = await db.EnrollRequests.FindAsync(id);

            if (req == null)
            {
                return HttpNotFound();
            }

            ViewBag.QuestionCount = await db.Questions.CountAsync();

            return View(new EnrollListItem() { Request = req, AnswerCount = req.User.Answers.Count() });
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