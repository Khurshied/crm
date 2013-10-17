using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HLGranite.Mvc.Models;

namespace HLGranite.Mvc.Controllers
{
    public class ActivityController : Controller
    {
        private hlgraniteEntities db = new hlgraniteEntities();

        //
        // GET: /Activity/

        public ActionResult Index()
        {
            var activities = db.Activities.Include(a => a.Status).Include(a => a.User).Include(a => a.WorkItem);
            return View(activities.ToList());
        }

        //
        // GET: /Activity/

        public ActionResult Parent(int id)
        {
            var activities = db.Activities.Include(a => a.Status).Include(a => a.User).Include(a => a.WorkItem).Where(a => a.WorkItemId.Equals(id));
            return View(activities.ToList());
        }

        //
        // GET: /Activity/Details/5

        public ActionResult Details(long id = 0)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        //
        // GET: /Activity/Create

        public ActionResult Create()
        {
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName");
            ViewBag.WorkItemId = new SelectList(db.WorkItems, "Id", "Id");
            return View();
        }

        //
        // POST: /Activity/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", activity.StatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", activity.UserId);
            ViewBag.WorkItemId = new SelectList(db.WorkItems, "Id", "Id", activity.WorkItemId);
            return View(activity);
        }

        //
        // GET: /Activity/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", activity.StatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", activity.UserId);
            ViewBag.WorkItemId = new SelectList(db.WorkItems, "Id", "Id", activity.WorkItemId);
            return View(activity);
        }

        //
        // POST: /Activity/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.Statuses, "Id", "Name", activity.StatusId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", activity.UserId);
            ViewBag.WorkItemId = new SelectList(db.WorkItems, "Id", "Id", activity.WorkItemId);
            return View(activity);
        }

        //
        // GET: /Activity/Delete/5

        public ActionResult Delete(long id = 0)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        //
        // POST: /Activity/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}