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
    [Authorize(Roles = "Administrators")]
    public class HomePageLinksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HomePageLinks
        public async Task<ActionResult> Index()
        {
            return View((await db.HomePageLinks.ToListAsync()).OrderBy(l => l.Index));
        }

        // GET: HomePageLinks/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HomePageLink homePageLink = await db.HomePageLinks.FindAsync(id);
            if (homePageLink == null)
            {
                return HttpNotFound();
            }
            return View(homePageLink);
        }

        // GET: HomePageLinks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomePageLinks/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Index,Name,Description,Icon,Link")] HomePageLink homePageLink)
        {
            if (ModelState.IsValid)
            {
                homePageLink.Id = Guid.NewGuid();
                db.HomePageLinks.Add(homePageLink);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(homePageLink);
        }

        // GET: HomePageLinks/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HomePageLink homePageLink = await db.HomePageLinks.FindAsync(id);
            if (homePageLink == null)
            {
                return HttpNotFound();
            }
            return View(homePageLink);
        }

        // POST: HomePageLinks/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Index,Name,Description,Icon,Link")] HomePageLink homePageLink)
        {
            if (ModelState.IsValid)
            {
                db.Entry(homePageLink).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(homePageLink);
        }

        // GET: HomePageLinks/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HomePageLink homePageLink = await db.HomePageLinks.FindAsync(id);
            if (homePageLink == null)
            {
                return HttpNotFound();
            }
            return View(homePageLink);
        }

        // POST: HomePageLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            HomePageLink homePageLink = await db.HomePageLinks.FindAsync(id);
            db.HomePageLinks.Remove(homePageLink);
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
