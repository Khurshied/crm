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

        public ActionResult Index(string soldTo, string status, string searchString)
        {
            ViewBag.SoldTo = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID), "Id", "DisplayName");
            short submit = db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID && s.Name == "Submit").First().Id;

            // TODO: SelectList statusList = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name");//, submit);
            IEnumerable<SelectListItem> statusList = new[]{
                new SelectListItem{Text="Pending", Value=""},
                new SelectListItem{Text="Save", Value="Save"},
                new SelectListItem{Text="Submit", Value="Submit"},
                new SelectListItem{Text="Design", Value="Design"},
                new SelectListItem{Text="Cut", Value="Cut"},
                new SelectListItem{Text="Complete", Value="Complete"},
                new SelectListItem{Text="Deliver", Value="Deliver"},
                new SelectListItem{Text="Close", Value="Close"},
                new SelectListItem{Text="All Status", Value="all"}
            };
            ViewBag.Status = statusList;

            var nisans = db.Nisans.Include(n => n.Stock).Include(n => n.SoldTo);//.OrderBy(n => n.StatusId);
            if (!String.IsNullOrEmpty(soldTo))
            {
                int id = Convert.ToInt32(soldTo);
                nisans = nisans.Where(n => n.SoldToId == id);
            }

            if (String.IsNullOrEmpty(status))
            {
                // pending case
                short draft = db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID).First().Id;
                short close = db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID).OrderByDescending(s => s.Id).First().Id;
                nisans = nisans.Where(n => n.StatusId > draft && n.StatusId < close);
            }
            else
            {
                if (status.ToLower() == "all")
                {
                    //short close = db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID).OrderByDescending(s => s.Id).First().Id;
                    //nisans = nisans.Where(n => n.StatusId < close);
                }
                else
                {
                    short id = db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID && s.Name == status).First().Id;
                    //int id = Convert.ToInt32(status);
                    nisans = nisans.Where(n => n.StatusId == id);
                }
            }

            if (!String.IsNullOrEmpty(searchString))
                nisans = nisans.Where(n => n.Rumi.ToLower().Contains(searchString.ToLower()));

            nisans = nisans.OrderBy(n => n.StatusId);
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
            //nisan.StatusId = db.Statuses.Where(s => s.StockTypeId == StockController.NISAN_TYPE_ID).First().Id;
            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name");
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name");
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID), "Id", "DisplayName");
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID), "Id", "DisplayName");
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

            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID), "Id", "DisplayName", nisan.SoldToId);
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
            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID), "Id", "DisplayName", nisan.SoldToId);
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
                db.Entry(nisan).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Edit", "Nisan",  new { Id = nisan.Id});
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID), "Id", "DisplayName", nisan.SoldToId);
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