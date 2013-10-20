using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HLGranite.Mvc.Models;
using HLGranite.Jawi;

namespace HLGranite.Mvc.Controllers
{
    public class NisanController : Controller
    {
        private hlgraniteEntities db = new hlgraniteEntities();

        //
        // GET: /Nisan/

        public ActionResult Index(string soldTo, string status, string searchString)
        {
            ViewBag.SoldTo = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName");
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

            nisans = nisans.OrderBy(n => n.StatusId).ThenBy(n => n.Id);
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
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID).OrderBy(s => s.Name), "Id", "Name");
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName");
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName");
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
                nisan.WorkItemId = workItem.Id;

                db.Nisans.Add(nisan);

                LogActivity(nisan);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID || u.UserTypeId == HLGranite.Mvc.Models.User.ADMIN_TYPE_ID), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID), "Id", "DisplayName", nisan.SoldToId);
            return View(nisan);
        }

        private void LogActivity(Nisan nisan)
        {
            HLGranite.Mvc.Models.User user = db.Users.Where(u => u.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            Activity activity = db.Activities.Create();
            activity.WorkItemId = nisan.WorkItemId;
            activity.Date = DateTime.Now;
            activity.StatusId = nisan.StatusId;
            if (user != null) activity.UserId = user.Id;
            db.Activities.Add(activity);
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
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID).OrderBy(s => s.Name), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID || u.UserTypeId == HLGranite.Mvc.Models.User.ADMIN_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName", nisan.SoldToId);
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
                // put loggin person as assignee after submit (normally submit is the second status after save or new).
                List<Status> statuses = db.Statuses.Where(s => s.StockTypeId == StockType.NISAN_TYPE_ID).Take(3).ToList();
                Status status = statuses[2];
                if (nisan.StatusId == status.Id && (nisan.AssigneeId == null || nisan.AssigneeId == 0))
                {
                    Mvc.Models.User assignee = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                    if (assignee != null)
                        nisan.AssigneeId = assignee.Id;
                }

                nisan.WorkItem = db.WorkItems.Where(w => w.Id.Equals(nisan.WorkItemId)).First();
                db.Entry(nisan).State = EntityState.Modified;
                LogActivity(nisan);
                db.SaveChanges();
                //return RedirectToAction("Edit", "Nisan",  new { Id = nisan.Id});
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID).OrderBy(s => s.Name), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID || u.UserTypeId == HLGranite.Mvc.Models.User.ADMIN_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName", nisan.AssigneeId);
            ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName", nisan.SoldToId);
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

        private DataTable ReadXml(string fileName)
        {
            DataTable table = new DataTable();
            DataSet dataset = new DataSet();

            try
            {
                if (System.IO.File.Exists(fileName))
                    dataset.ReadXml(fileName);
                if (dataset.Tables.Count > 0)
                    table = dataset.Tables[0].Copy();

                return table;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return table;
            }
            finally { dataset.Dispose(); }
        }
        public ActionResult Calendar()
        {
            ViewBag.Date = DateTime.Now;
            ViewBag.Muslim = "";
            return View();
        }
        [HttpPost]
        public ActionResult Calendar(string date)
        {
            DateTime gregorian;
            DateTime.TryParse(date, out gregorian);

            string file = Request.MapPath("~\\App_Data\\muslimcal.xml");
            MuslimCalendar calendar = new MuslimCalendar(ReadXml(file));
            calendar.GetDate(gregorian);

            DateTime muslim = new DateTime(calendar.Year, calendar.Month, calendar.Day);
            string output = muslim.Day.ToString("00") + "/" + muslim.Month.ToString("00") + "/" + muslim.Year;

            ViewBag.Muslim = output;
            ViewBag.Date = date;
            ViewBag.Gregorian = gregorian.Day.ToString("00") + "/" + gregorian.Month.ToString("00") + "/" + gregorian.Year;
            return View();
        }

        /// <summary>
        /// Return rss feed.
        /// See http://blogs.microsoft.co.il/blogs/bursteg/archive/2009/01/11/asp-net-mvc-rss-feed-action-result.aspx
        /// </summary>
        /// <returns></returns>
        public ActionResult Rss()
        {
            Feed feed = new Feed { Title = "Nisan Orders", Description = "Latest updates"};
            var activities = db.Activities.Include(a => a.User).Include(a => a.Status).Take(50).OrderByDescending(a => a.Date);
            foreach (var activity in activities)
            {
                // compose nisan case
                FeedItem item = new FeedItem();
                var nisan = db.Nisans.Include(n => n.SoldTo).Include(n => n.Stock).Where(n => n.Id.Equals(activity.WorkItemId)).FirstOrDefault();
                if(nisan != null)
                {
                    item.Title = nisan.SoldTo.DisplayName + " | " + nisan + " - " + nisan.Stock.Name;
                    if (nisan.Assignee != null)
                        item.Creator = nisan.Creator.DisplayName;
                    item.Description = activity.User.UserName + " " + activity.Status.Name.ToLower() + " " + nisan + " - " + nisan.Stock.Name + " at " + activity.Date;
                    item.Published = activity.Date;// DateTime.Now;
                    item.Url = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + "/Nisan/Edit/" + nisan.Id;
                }

                if(!String.IsNullOrEmpty(item.Title))
                    feed.Items.Add(item);
            }

            return View(feed);
        }
    }
}