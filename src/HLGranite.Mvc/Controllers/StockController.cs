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
    public class StockController : Controller
    {
        private hlgraniteEntities db = new hlgraniteEntities();

        public static int RENOVATION_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                StockType type = db.StockTypes.Where(t => t.Type.ToLower().Equals("renovation")).FirstOrDefault();
                if (type == null)
                    return 0;
                else
                    return type.Id;
            }
        }

        public static int TOMB_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                StockType type = db.StockTypes.Where(t => t.Type.ToLower().Equals("tomb")).FirstOrDefault();
                if (type == null)
                    return 0;
                else
                    return type.Id;
            }
        }

        public static int NISAN_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                StockType type = db.StockTypes.Where(t => t.Type.ToLower().Equals("nisan")).FirstOrDefault();
                if (type == null)
                    return 0;
                else
                    return type.Id;
            }
        }

        //
        // GET: /Stock/

        public ActionResult Index()
        {
            var stocks = db.Stocks.Include(s => s.StockType);
            return View(stocks.ToList());
        }

        //
        // GET: /Stock/Details/5

        public ActionResult Details(int id = 0)
        {
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        //
        // GET: /Stock/Create

        public ActionResult Create()
        {
            Stock stock = db.Stocks.Create();
            ViewBag.StockTypeId = new SelectList(db.StockTypes, "Id", "Type");
            return View(stock);
        }

        //
        // POST: /Stock/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Stocks.Add(stock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StockTypeId = new SelectList(db.StockTypes, "Id", "Type", stock.StockTypeId);
            return View(stock);
        }

        //
        // GET: /Stock/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            ViewBag.StockTypeId = new SelectList(db.StockTypes, "Id", "Type", stock.StockTypeId);
            return View(stock);
        }

        //
        // POST: /Stock/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StockTypeId = new SelectList(db.StockTypes, "Id", "Type", stock.StockTypeId);
            return View(stock);
        }

        //
        // GET: /Stock/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        //
        // POST: /Stock/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock stock = db.Stocks.Find(id);
            db.Stocks.Remove(stock);
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