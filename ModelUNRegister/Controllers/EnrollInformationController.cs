using Microsoft.AspNet.Identity;
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
    public class EnrollInformationController : Controller
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

        // GET: Index
        public async Task<ActionResult> Index()
        {
            int questionCount = await db.Questions.CountAsync();
            string uid = User.Identity.GetUserId();
            int answeredCount = await db.Questions.Where(question => db.Answers.Count(ans => ans.User.Id == uid && ans.Question.Id == question.Id) > 0).CountAsync();
            ViewBag.UnansweredQuestion = questionCount - answeredCount;
            return View();
        }

        public async Task<ActionResult> Edit()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user.EnrollRequest != null)
            {
                return View(EnrollViewModel.CreateFromUser(user));
            }
            else
            {
                return View("../Shared/Message", new MessageViewModel()
                {
                    Title = "错误",
                    Message = "当前用户没有报名信息。",
                    Theme = BootstrapTheme.Warning
                });
            }
        }

        public async Task<ActionResult> Edit(EnrollViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                user.ActualName = model.Name;
                user.EnrollRequest.Gender = model.Gender;
                user.EnrollRequest.School = model.School;
                user.EnrollRequest.Grade = model.Grade;
                user.PhoneNumber = model.PhoneNumber;
                user.EnrollRequest.QQNumber = model.QQNumber;

                await UserManager.UpdateAsync(user);

                return Redirect(Url.Action("Index"));
            }
            else
            {
                return View("../Shared/Message", new MessageViewModel()
                {
                    Title = "错误",
                    Message = "模型不正确。",
                    Theme = BootstrapTheme.Danger
                });
            }
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

        [HttpPost]
        public async Task<ActionResult> Answers(AnswersViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                foreach (QuestionAnswerViewModel qa in model.Answers)
                {
                    EnrollQuestionAnswer answer = db.Answers.Where(ans => ans.Question.Id == qa.Question.Id && ans.User.Id == user.Id).FirstOrDefault();
                    // 如果当前问题已经有了回答
                    if (answer != null)
                    {
                        // 如果有回答就修改，如果没有就删掉。
                        if (!string.IsNullOrWhiteSpace(qa.AnswerContent))
                        {
                            answer.AnswerContent = qa.AnswerContent;
                        }
                        else
                        {
                            db.Answers.Remove(answer);
                        }
                    }
                    else
                    {
                        // 如果回答不为空
                        if (!string.IsNullOrWhiteSpace(qa.AnswerContent))
                        {
                            answer = new EnrollQuestionAnswer();
                            answer.Id = Guid.NewGuid();
                            answer.AnswerContent = qa.AnswerContent;
                            answer.Question = db.Questions.Where(d => d.Id == qa.Question.Id).First();
                            answer.User = db.Users.Where(u => u.Id == user.Id).First();
                            db.Answers.Add(answer);
                        }
                    }
                }
                await db.SaveChangesAsync();
                return Redirect(Url.Action("Index"));
            }
            else
            {
                return View("../Shared/Message", new MessageViewModel()
                {
                    Title = "错误",
                    Message = "模型不正确。",
                    Theme = BootstrapTheme.Danger
                });
            }
        }
    }
}