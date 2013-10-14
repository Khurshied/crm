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
    public class StockTypeController : Controller
    {
        private hlgraniteEntities db = new hlgraniteEntities();

        //
        // GET: /StockType/

        public ActionResult Index()
        {
            return View(db.StockTypes.ToList());
        }

        //
        // GET: /StockType/Details/5

        public ActionResult Details(short id = 0)
        {
            StockType stocktype = db.StockTypes.Find(id);
            if (stocktype == null)
            {
                return HttpNotFound();
            }
            return View(stocktype);
        }

        //
        // GET: /StockType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /StockType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockType stocktype)
        {
            if (ModelState.IsValid)
            {
                db.StockTypes.Add(stocktype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stocktype);
        }

        //
        // GET: /StockType/Edit/5

        public ActionResult Edit(short id = 0)
        {
            StockType stocktype = db.StockTypes.Find(id);
            if (stocktype == null)
            {
                return HttpNotFound();
            }
            return View(stocktype);
        }

        //
        // POST: /StockType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StockType stocktype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stocktype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stocktype);
        }

        //
        // GET: /StockType/Delete/5

        public ActionResult Delete(short id = 0)
        {
            StockType stocktype = db.StockTypes.Find(id);
            if (stocktype == null)
            {
                return HttpNotFound();
            }
            return View(stocktype);
        }

        //
        // POST: /StockType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            StockType stocktype = db.StockTypes.Find(id);
            db.StockTypes.Remove(stocktype);
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