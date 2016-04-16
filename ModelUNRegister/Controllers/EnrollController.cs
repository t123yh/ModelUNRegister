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

namespace ModelUNRegister.Controllers
{
    public class EnrollController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
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
        public async Task<ActionResult> Create([Bind(Include = "Name,School,Gender,Grade,Email,PhoneNumber,QQNumber")] EnrollRequest enrollRequest)
        {
            if (ModelState.IsValid)
            {
                enrollRequest.RequestId = Guid.NewGuid();
                enrollRequest.SubmissionTime = DateTime.Now;
                enrollRequest.IPAddress = Request.UserHostAddress;
                enrollRequest.EmailVerified = false;
                db.EnrollRequests.Add(enrollRequest);
                db.SaveChanges();
                EmailVerificationViewModel emailModel = new EmailVerificationViewModel()
                {
                    Name = enrollRequest.Name,
                    Id = enrollRequest.RequestId
                };
                await MailHelper.SendAsync(MailHelper.RenderPartialToString(this, "EmailVerification", emailModel),
                   "注册",
                   enrollRequest.Email,
                   AppSettings.SMTPServer,
                   AppSettings.SMTPPort,
                   AppSettings.SMTPSSL,
                   AppSettings.MailAccount,
                   AppSettings.MailPassword);

                return RedirectToAction("Success");
            }

            return View(enrollRequest);
        }

        public ActionResult Success()
        {
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
