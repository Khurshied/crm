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
            var nisans = db.Nisans.Include(n => n.Stock).Include(n => n.User);
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
            ViewBag.StockId = new SelectList(db.Stocks, "Id", "Name");
            ViewBag.SoldToId = new SelectList(db.Users, "Id", "DisplayName");
            return View();
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
                //db.SaveChanges();

                nisan.WorItemId = workItem.Id;
                db.Nisans.Add(nisan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StockId = new SelectList(db.Stocks, "Id", "Code", nisan.StockId);
            ViewBag.SoldToId = new SelectList(db.Users, "Id", "DisplayName", nisan.SoldToId);
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
            ViewBag.StockId = new SelectList(db.Stocks, "Id", "Code", nisan.StockId);
            ViewBag.SoldToId = new SelectList(db.Users, "Id", "DisplayName", nisan.SoldToId);
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
                db.Entry(nisan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StockId = new SelectList(db.Stocks, "Id", "Code", nisan.StockId);
            ViewBag.SoldToId = new SelectList(db.Users, "Id", "DisplayName", nisan.SoldToId);
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