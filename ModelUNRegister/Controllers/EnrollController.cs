using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelUNRegister.Models;
using ModelUNRegister.Utilities;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace ModelUNRegister.Controllers
{
    public class EnrollController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
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

        // GET: Enroll/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Enroll/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,School,Gender,Grade,Email,PhoneNumber,QQNumber")] EnrollViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser olduser;
                if ((olduser = await UserManager.FindByEmailAsync(viewModel.Email)) != null)
                {
                    if (!olduser.EmailConfirmed)
                    {
                        if (DateTime.Now - olduser.EnrollRequest.SubmissionTime > TimeSpan.FromSeconds(AppSettings.DuplicateEmailResetTime))
                        {
                            await UserManager.DeleteAsync(olduser);
                        }
                        else
                        {
                            return View("../Shared/Message", new MessageViewModel()
                            {
                                Title = "电子邮件地址重复",
                                Message = $"该电子邮件地址已被使用，但尚未被验证。如果这是你的电子邮件地址，你可以在 {(int)((AppSettings.DuplicateEmailResetTime - (DateTime.Now - olduser.EnrollRequest.SubmissionTime).TotalSeconds) / 60)} 分钟后重新提交报名请求。",
                                Theme = BootstrapTheme.Warning
                            });
                        }
                    }
                    else
                    {
                        return View("../Shared/Message", new MessageViewModel()
                        {
                            Title = "电子邮件地址重复",
                            Message = $"该电子邮件地址已被使用，并且已被验证。如果希望修改报名信息，你可以在首页选择“修改报名信息”选项，并按页面提示操作。",
                            Theme = BootstrapTheme.Warning
                        });
                    }
                }
                EnrollRequest enrollRequest = new EnrollRequest();
                enrollRequest.RequestId = Guid.NewGuid();
                enrollRequest.SubmissionTime = DateTime.Now;
                enrollRequest.IPAddress = Request.UserHostAddress;
                enrollRequest.Gender = viewModel.Gender;
                enrollRequest.Grade = viewModel.Grade;
                enrollRequest.QQNumber = viewModel.QQNumber;
                enrollRequest.School = viewModel.School;

                var user = new ApplicationUser();
                user.UserName = user.Id;
                user.ActualName = viewModel.Name;
                user.Email = viewModel.Email;
                user.PhoneNumber = viewModel.PhoneNumber;
                user.EnrollRequest = enrollRequest;

                var result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return Content(result.Errors.Aggregate("", (n, p) => n + " " + p));
                }

                EmailVerificationViewModel emailModel = new EmailVerificationViewModel()
                {
                    Name = user.ActualName,
                    EmailConfirmationLink = Url.Action("EmailConfirmation", "Enroll", new
                    {
                        UserId = user.Id,
                        Token = (await UserManager.GenerateEmailConfirmationTokenAsync(user.Id))
                    }, Request.Url.Scheme),
                    Questions = db.Questions.OrderBy(q => q.Index).ToList()
                };
                await UserManager.SendEmailAsync(user.Id, "元峰会 - 报名确认", EmailHelper.RenderPartialToString(this, "EmailConfirmationEmail", emailModel));

                return View("../Shared/Message", new MessageViewModel()
                {
                    Title = "已发送验证邮件",
                    Message = $"验证邮件已发至你的电子邮箱，请按邮件内的提示操作。",
                    Theme = BootstrapTheme.Success
                });
            }

            return View(viewModel);
        }

        public ActionResult Success()
        {
            return View();
        }

        public async Task<ActionResult> EmailConfirmation(string userId, string token)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            if (user.EmailConfirmed)
            {
                await SignInManager.SignInAsync(user, true, true);
                return Content("You've already signed in.");
            }
            return View(EnrollViewModel.CreateFromUser(user));
        }

        [HttpPost, ActionName("EmailConfirmation"), ValidateAntiForgeryToken]
        public async Task<ActionResult> EmailConfirmationPost(string userId, string token)
        {
            await UserManager.ConfirmEmailAsync(userId, token);
            return Content("To be implemented.");
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
