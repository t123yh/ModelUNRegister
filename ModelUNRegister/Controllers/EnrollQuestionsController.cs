using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelUNRegister.Models;

namespace ModelUNRegister.Controllers
{
    [Authorize(Roles ="Administrators")]
    public class EnrollQuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EnrollQuestions
        public async Task<ActionResult> Index()
        {
            return View(await db.Questions.OrderBy(item => item.Index).ToListAsync());
        }

        // GET: EnrollQuestions/Create
        public async Task<ActionResult> Create()
        {
            int index;
            if(await db.Questions.CountAsync() == 0)
            {
                index = 1;
            }
            else
            {
                index = await db.Questions.MaxAsync(item => item.Index) + 1;
            }
            return View(new EnrollQuestion() { Index = index });
        }

        // POST: EnrollQuestions/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Index,Title,Description")] EnrollQuestion enrollQuestion)
        {
            if (ModelState.IsValid)
            {
                enrollQuestion.Id = Guid.NewGuid();
                db.Questions.Add(enrollQuestion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(enrollQuestion);
        }

        // GET: EnrollQuestions/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnrollQuestion enrollQuestion = await db.Questions.FindAsync(id);
            if (enrollQuestion == null)
            {
                return HttpNotFound();
            }
            return View(enrollQuestion);
        }

        // POST: EnrollQuestions/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Index")] EnrollQuestion enrollQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollQuestion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(enrollQuestion);
        }

        // GET: EnrollQuestions/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EnrollQuestion enrollQuestion = await db.Questions.FindAsync(id);
            if (enrollQuestion == null)
            {
                return HttpNotFound();
            }
            return View(enrollQuestion);
        }

        // POST: EnrollQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            EnrollQuestion enrollQuestion = await db.Questions.FindAsync(id);
            db.Questions.Remove(enrollQuestion);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
