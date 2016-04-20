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
                   "元峰会报名确认",
                   "元峰会",
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

        public async Task<ActionResult> Confirm(Guid id)
        {
            var item = await db.EnrollRequests.FirstAsync(o => o.RequestId == id);
            return View(item);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmPost(Guid id)
        {
            //var item = await db.EnrollRequests.FirstAsync(o => o.RequestId == id);
            var item = new EnrollRequest()
            {
                RequestId = id
            };
            item.EmailVerified = true;
            item.EmailVerificationTime = DateTime.Now;
            db.EnrollRequests.Attach(item);
            db.Entry(item).Property(o => o.EmailVerificationTime).IsModified
                = db.Entry(item).Property(o => o.EmailVerified).IsModified
                = true;
            await db.SaveChangesAsync();
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
