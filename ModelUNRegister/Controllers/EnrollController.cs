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
                EnrollRequest enrollRequest = new EnrollRequest();
                enrollRequest.RequestId = Guid.NewGuid();
                enrollRequest.SubmissionTime = DateTime.Now;
                enrollRequest.IPAddress = Request.UserHostAddress;
                enrollRequest.Gender = viewModel.Gender;
                enrollRequest.Grade = viewModel.Grade;
                enrollRequest.QQNumber = viewModel.QQNumber;
                enrollRequest.School = viewModel.School;

                var user = new ApplicationUser();
                user.UserName = viewModel.Name;
                user.Email = viewModel.Email;
                user.PhoneNumber = viewModel.PhoneNumber;
                user.EnrollRequest = enrollRequest;

                await UserManager.CreateAsync(user);

                EmailVerificationViewModel emailModel = new EmailVerificationViewModel()
                {
                    Name = user.UserName,
                    EmailConfirmationLink = Url.Action("EmailConfirmation", "Enroll", new
                    {
                        UserId = user.Id,
                        Token = (await UserManager.GenerateEmailConfirmationTokenAsync(user.Id))
                    }, Request.Url.Scheme)
                };
                await UserManager.SendEmailAsync(user.Id, "元峰会 - 报名确认", EmailHelper.RenderPartialToString(this, "EmailConfirmationEmail", emailModel));

                return RedirectToAction("EmailSent");
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
