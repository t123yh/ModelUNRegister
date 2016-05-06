using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ModelUNRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ModelUNRegister.Controllers
{
    [Authorize]
    public class AnswerController : Controller
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

        // GET: Answer
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Answers()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var answers = db.Questions.OrderBy(q => q.Index).ToArray().Select(question =>
              {
                  EnrollQuestionAnswer answer = db.Answers.Where(ans => ans.User.Id == user.Id && ans.Question.Id == question.Id).FirstOrDefault();
                  return new QuestionAnswerViewModel()
                  {
                      AnswerContent = answer?.AnswerContent,
                      Question = QuestionViewModel.CreateFromQuestion(question)
                  };
              });

            AnswersViewModel model = new AnswersViewModel() { Answers = answers.ToList() };

            return View(model);
        }

    }
}