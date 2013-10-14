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
    public class StatusController : Controller
    {
        private hlgraniteEntities db = new hlgraniteEntities();

        //
        // GET: /Status/

        public ActionResult Index()
        {
            var statuses = db.Statuses.Include(s => s.StockType);
            return View(statuses.ToList());
        }

        //
        // GET: /Status/Details/5

        public ActionResult Details(short id = 0)
        {
            Status status = db.Statuses.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        //
        // GET: /Status/Create

        public ActionResult Create()
        {
            ViewBag.StockTypeId = new SelectList(db.StockTypes, "Id", "Type");
            return View();
        }

        //
        // POST: /Status/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Status status)
        {
            if (ModelState.IsValid)
            {
                db.Statuses.Add(status);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StockTypeId = new SelectList(db.StockTypes, "Id", "Type", status.StockTypeId);
            return View(status);
        }

        //
        // GET: /Status/Edit/5

        public ActionResult Edit(short id = 0)
        {
            Status status = db.Statuses.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            ViewBag.StockTypeId = new SelectList(db.StockTypes, "Id", "Type", status.StockTypeId);
            return View(status);
        }

        //
        // POST: /Status/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Status status)
        {
            if (ModelState.IsValid)
            {
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StockTypeId = new SelectList(db.StockTypes, "Id", "Type", status.StockTypeId);
            return View(status);
        }

        //
        // GET: /Status/Delete/5

        public ActionResult Delete(short id = 0)
        {
            Status status = db.Statuses.Find(id);
            if (status == null)
            {
                return HttpNotFound();
            }
            return View(status);
        }

        //
        // POST: /Status/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Status status = db.Statuses.Find(id);
            db.Statuses.Remove(status);
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