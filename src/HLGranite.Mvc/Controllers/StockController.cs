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

        //
        // GET: /Stock/
        [Authorize]
        public ActionResult Index(string type, string searchString)
        {
            ViewBag.Type = new SelectList(db.StockTypes, "Id", "Type");

            var stocks = db.Stocks.Include(s => s.StockType);
            if (!String.IsNullOrEmpty(type))
            {
                int id = Convert.ToInt32(type);
                stocks = stocks.Where(s => s.StockTypeId == id);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                stocks = stocks.Where(s => s.Name.ToLower().Contains(searchString) || s.Code.ToLower().Contains(searchString));
            }
            
            stocks = stocks.OrderBy(s => s.StockType.Type).ThenBy(s => s.Name);
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
        [AuthorizeAdmin]
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
        [AuthorizeAdmin]
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
        [Authorize]
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
        [AuthorizeAdmin]
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
        [AuthorizeAdmin]
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