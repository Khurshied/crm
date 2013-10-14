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
    public class NisanController : Controller
    {
        private hlgraniteEntities db = new hlgraniteEntities();

        //
        // GET: /Nisan/

        public ActionResult Index()
        {
            short closeStatus = db.Statuses.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID).OrderByDescending(s => s.Id).First().Id;
            var nisans = db.Nisans.Include(n => n.Stock).Include(n => n.SoldTo).Where(n => n.StatusId < closeStatus);
            return View(nisans.ToList());
        }

        //
        // GET: /Nisan/Details/5

        public ActionResult Details(int id = 0)
        {
            Nisan nisan = db.Nisans.Find(id);
            if (nisan == null)
            {
                return HttpNotFound();
            }
            return View(nisan);
        }

        //
        // GET: /Nisan/Create

        public ActionResult Create()
        {
            Nisan nisan = db.Nisans.Create();
            nisan.StatusId = db.Statuses.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID).First().Id;

            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID), "Id", "Name");
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == UserController.STAFF_TYPE_ID), "Id", "DisplayName");
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == UserController.AGENT_TYPE_ID), "Id", "DisplayName");
            return View(nisan);
        }

        //
        // POST: /Nisan/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Nisan nisan)
        {
            if (ModelState.IsValid)
            {
                WorkItem workItem = db.WorkItems.Create();
                db.WorkItems.Add(workItem);

                //short nextStatus = db.Statuses.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID && s.Id != nisan.StatusId).First().Id;
                //nisan.StatusId = nextStatus;
                nisan.WorkItemId = workItem.Id;
                db.Nisans.Add(nisan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == UserController.STAFF_TYPE_ID), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == UserController.AGENT_TYPE_ID), "Id", "DisplayName", nisan.SoldToId);
            return View(nisan);
        }

        //
        // GET: /Nisan/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Nisan nisan = db.Nisans.Find(id);
            if (nisan == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == UserController.STAFF_TYPE_ID), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == UserController.AGENT_TYPE_ID), "Id", "DisplayName", nisan.SoldToId);
            return View(nisan);
        }

        //
        // POST: /Nisan/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Nisan nisan)
        {
            if (ModelState.IsValid)
            {
                nisan.WorkItem = db.WorkItems.Where(w => w.Id.Equals(nisan.WorkItemId)).First();
                //db.Entry(nisan.WorkItem).State = EntityState.Modified;
                db.Entry(nisan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == UserController.STAFF_TYPE_ID), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == UserController.AGENT_TYPE_ID), "Id", "DisplayName", nisan.SoldToId);
            return View(nisan);
        }

        //
        // GET: /Nisan/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Nisan nisan = db.Nisans.Find(id);
            if (nisan == null)
            {
                return HttpNotFound();
            }
            return View(nisan);
        }

        //
        // POST: /Nisan/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nisan nisan = db.Nisans.Find(id);
            db.Nisans.Remove(nisan);
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