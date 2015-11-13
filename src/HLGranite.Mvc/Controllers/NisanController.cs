using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Web.Mvc;
using HLGranite.Jawi;
using HLGranite.Mvc.Models;

namespace HLGranite.Mvc.Controllers
{
    public class NisanController : Controller
    {
        private hlgraniteEntities db = new hlgraniteEntities();
        // TODO: SelectList statusList = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name");//, submit);
        public static IEnumerable<SelectListItem> StatusList = new[]{
                new SelectListItem{Text="Pending", Value=""},
                new SelectListItem{Text="Save", Value="Save"},
                new SelectListItem{Text="Submit", Value="Submit"},
                new SelectListItem{Text="Design", Value="Design"},
                new SelectListItem{Text="Cut", Value="Cut"},
                new SelectListItem{Text="Complete", Value="Complete"},
                new SelectListItem{Text="Deliver", Value="Deliver"},
                new SelectListItem{Text="All Status", Value="all"}
        };

        public static IEnumerable<SelectListItem> MuslimMonthList = new[]{
                new SelectListItem{Text="", Value="0"},
                new SelectListItem{Text="Muharram", Value="1"},
                new SelectListItem{Text="Safar", Value="2"},
                new SelectListItem{Text="Rabiulawal", Value="3"},
                new SelectListItem{Text="Rabiulakhir", Value="4"},
                new SelectListItem{Text="Jamadilawal", Value="5"},
                new SelectListItem{Text="Jamadilakhir", Value="6"},
                new SelectListItem{Text="Rejab", Value="7"},
                new SelectListItem{Text="Syaaban", Value="8"},
                new SelectListItem{Text="Ramadhan", Value="9"},
                new SelectListItem{Text="Syawal", Value="10"},
                new SelectListItem{Text="Zulkaedah", Value="11"},
                new SelectListItem{Text="Zulhijjah", Value="12"}
        };


        //
        // GET: /Nisan/

        public ActionResult Index(string soldTo, string status, string searchString)
        {
            ViewBag.SoldTo = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName");
            //short submit = db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID && s.Name == "Submit").First().Id;
            ViewBag.Status = StatusList;
            var nisans = db.Nisans.Include(n => n.Stock).Include(n => n.SoldTo);//.OrderBy(n => n.StatusId);

            HLGranite.Mvc.Models.User user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (User.Identity.Name.Length == 0)
            {
                nisans = db.Nisans.Where(n => n.Id == 0);
                return View(nisans);
            }
            else
            {
                if (user == null)
                {
                    nisans = db.Nisans.Where(n => n.Id == 0);
                    return View();
                }
                else
                {
                    if (user.UserTypeId != Models.User.ADMIN_TYPE_ID && user.UserTypeId != Models.User.STAFF_TYPE_ID)
                    {
                        nisans = nisans.Where(n => n.SoldToId == user.Id);
                    }
                }
            }

            if (!String.IsNullOrEmpty(soldTo))
            {
                int id = Convert.ToInt32(soldTo);
                nisans = nisans.Where(n => n.SoldToId == id);
            }

            if (String.IsNullOrEmpty(status))
            {
                // pending case
                short draft = db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID).First().Id;
                //short close = db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID).OrderByDescending(s => s.Id).First().Id;
                // 47 is deliver
                nisans = nisans.Where(n => n.StatusId > draft && n.StatusId < 47); // close);
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

            // last updated not support in linq sorting
            nisans = nisans.OrderBy(n => n.StatusId).OrderByDescending(n => n.Id);//.ThenBy(n => n.Id);
            return View(nisans.ToList());
        }

        /// <summary>
        /// Show Nisan report.
        /// </summary>
        /// <param name="soldTo"></param>
        /// <param name="status"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public ActionResult Report(string soldTo, string status, DateTime? from, DateTime? to)
        {
            ViewBag.SoldTo = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName");
            ViewBag.Status = StatusList;
            var nisans = db.Nisans.Include(n => n.Stock).Include(n => n.SoldTo);

            HLGranite.Mvc.Models.User user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (User.Identity.Name.Length == 0)
            {
                nisans = db.Nisans.Where(n => n.Id == 0);
                return View(nisans);
            }
            else
            {
                if (user == null)
                {
                    nisans = db.Nisans.Where(n => n.Id == 0);
                    return View();
                }
                else
                {
                    if (user.UserTypeId != Models.User.ADMIN_TYPE_ID && user.UserTypeId != Models.User.STAFF_TYPE_ID)
                    {
                        nisans = nisans.Where(n => n.SoldToId == user.Id);
                    }
                }
            }

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
                    nisans = nisans.Where(n => n.StatusId == id);
                }
            }

            if (from.HasValue)
            {
                DateTime fromDate = new DateTime(from.Value.Year, from.Value.Month, from.Value.Day);
                List<Nisan> holder = new List<Nisan>();

                // HACK: Created cannot quarable with linq because it is an virtual field.
                foreach (Nisan nisan in nisans)
                {
                    if (nisan.Created >= fromDate)
                        holder.Add(nisan);
                }
                nisans = holder.AsQueryable();
            }

            if (to.HasValue)
            {
                DateTime toDate = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day);
                toDate = toDate.AddDays(1);

                List<Nisan> holder = new List<Nisan>();
                foreach (Nisan nisan in nisans)
                {
                    if (nisan.Created < toDate)
                        holder.Add(nisan);
                }
                nisans = holder.AsQueryable();
            }

            return View(nisans.ToList());
        }

        /// <summary>
        /// Show nisan chart.
        /// </summary>
        /// <remarks>
        /// see also http://www.scriptscoop.net/t/834358f45722/c-asp-net-mvc-chart-how-to-show-values-on-each-column.html
        /// </remarks>
        /// <returns></returns>
        public ActionResult Chart()
        {
            // prepare data
            string sql = "select";
            sql += " Year(Activities.Date)*100+Month(Activities.Date) as Month, Stocks.Name, COUNT(Nisans.Id) as Quantity";
            sql += " from Nisans";
            //sql += " join Users on Nisans.SoldToId=Users.Id";
            sql += " join Stocks on Nisans.StockId=Stocks.Id";
            sql += " join WorkItems on Nisans.WorkItemId=WorkItems.Id";
            sql += " join Activities on WorkItems.Id=Activities.WorkItemId";
            sql += " where Activities.StatusId=43";
            sql += " and Activities.Id in (";
            sql += "    select a.Id";
            sql += "    from (select b.Id, rowid = ROW_NUMBER() OVER (PARTITION BY WorkItemId ORDER BY Id) from Activities b) a";
            sql += "    where rowid <= 1";
            sql += " )";
            sql += " group by Year(Activities.Date)*100+Month(Activities.Date), Stocks.Name";
            sql += " order by Year(Activities.Date)*100+Month(Activities.Date), Stocks.Name";

            var data = db.Database.SqlQuery<HLGranite.Mvc.Models.MonthlyStock>(sql).ToList();
            var chart = new Chart();
            chart.Titles.Add("Monthly Sold Quantity Per Stock");
            chart.Width = 1400;
            chart.Height = 600;
            chart.ChartAreas.Add(new ChartArea());

            var allStocks = data.Select(f => f.Name).ToList().Distinct();
            List<string> stocks = allStocks.ToList();
            stocks.Sort();

            // flatten the data in columns dimension
            var allDates = data.Select(f => f.Month).ToList().Distinct();
            List<int> dates = allDates.ToList();
            dates.Sort();

            foreach (string stock in stocks)
            {
                chart.Series.Add(new Series(stock));
                chart.Series[stock].ChartType = SeriesChartType.StackedColumn;
                foreach (int date in dates)
                {
                    var monthlyStock = data.Where(f => f.Month.Equals(date) && f.Name.Equals(stock)).FirstOrDefault();
                    int qty = (monthlyStock != null) ? monthlyStock.Quantity : 0;
                    chart.Series[stock].Points.AddXY(date.ToString(), qty);
                }
                if (stock.Contains("2' Batu Batik") || stock.Contains("Tai Hitam") || stock.Contains("Tai Putih"))
                    chart.Series[stock].IsValueShownAsLabel = true;
            }
            chart.Legends.Add("2' Batu Batik"); // HACK

            using (MemoryStream ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                //return File(ms.ToArray(), "image/png");
                return View(new ViewChart(ms.ToArray()));
            }
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

        /// <summary>
        /// Set ViewBag collection return to view.
        /// </summary>
        /// <param name="nisan"></param>
        private void SetViewBag(Nisan nisan)
        {
            ViewBag.StatusId = new SelectList(db.Statuses.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID), "Id", "Name", nisan.StatusId);
            ViewBag.StockId = new SelectList(db.Stocks.Where(s => s.StockTypeId == HLGranite.Mvc.Models.StockType.NISAN_TYPE_ID && s.Active == true).OrderBy(s => s.Name), "Id", "Name", nisan.StockId);
            ViewBag.AssigneeId = new SelectList(db.Users.Where(u => (u.UserTypeId == HLGranite.Mvc.Models.User.STAFF_TYPE_ID || u.UserTypeId == HLGranite.Mvc.Models.User.ADMIN_TYPE_ID) && u.Active == true).OrderBy(u => u.UserName), "Id", "DisplayName", nisan.AssigneeId);

            HLGranite.Mvc.Models.User user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                if (user.UserTypeId != Models.User.ADMIN_TYPE_ID && user.UserTypeId != Models.User.STAFF_TYPE_ID)
                    ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID || u.UserTypeId == HLGranite.Mvc.Models.User.CUSTOMER_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName", nisan.SoldToId);
                else
                    ViewBag.SoldToId = new SelectList(db.Users.Where(u => u.UserTypeId == HLGranite.Mvc.Models.User.AGENT_TYPE_ID).OrderBy(u => u.UserName), "Id", "DisplayName", nisan.SoldToId);
            }

            ViewBag.MuslimMonth = MuslimMonthList;
        }

        //
        // GET: /Nisan/Create
        [Authorize]
        public ActionResult Create()
        {
            HLGranite.Mvc.Models.User user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            Nisan nisan = db.Nisans.Create();
            if (user != null)
            {
                if (user.UserTypeId != Models.User.ADMIN_TYPE_ID && user.UserTypeId != Models.User.STAFF_TYPE_ID)
                    nisan.SoldToId = user.Id;
            }

            SetViewBag(nisan);
            return View(nisan);
        }

        //
        // POST: /Nisan/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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

            SetViewBag(nisan);
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
        [AuthorizeOwner]
        public ActionResult Edit(int id = 0)
        {
            Nisan nisan = db.Nisans.Find(id);
            if (nisan == null)
            {
                return HttpNotFound();
            }

            SetViewBag(nisan);
            return View(nisan);
        }

        //
        // POST: /Nisan/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOwner]
        public ActionResult Edit(Nisan nisan)
        {
            if (ModelState.IsValid)
            {
                // put loggin person as assignee after submit (normally submit is the second status after save or new).
                List<Status> statuses = db.Statuses.Where(s => s.StockTypeId == StockType.NISAN_TYPE_ID).Take(3).ToList();
                Status status = statuses[2];
                if (nisan.AssigneeId == null || nisan.AssigneeId == 0)
                {
                    if (nisan.StatusId >= status.Id)
                    {
                        Mvc.Models.User assignee = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                        if (assignee != null) nisan.AssigneeId = assignee.Id;
                    }
                }

                nisan.WorkItem = db.WorkItems.Where(w => w.Id.Equals(nisan.WorkItemId)).First();
                db.Entry(nisan).State = EntityState.Modified;
                LogActivity(nisan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            SetViewBag(nisan);
            return View(nisan);
        }

        //
        // GET: /Nisan/Delete/5

        [Authorize]
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
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nisan nisan = db.Nisans.Find(id);
            db.Nisans.Remove(nisan);
            db.SaveChanges();
            return RedirectToAction("Index");
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

            ViewBag.Date = date;
            ViewBag.Gregorian = gregorian;
            ViewBag.Muslim = muslim;
            return View();
        }

        public ActionResult Translate(string rumi)
        {
            string result = string.Empty;

            JawiTranslator translator = new JawiTranslator();
            if (!String.IsNullOrEmpty(rumi))
            {
                string[] words = rumi.Split(new char[] { ' ' });
                foreach (string word in words)
                    result += translator.Translate(word) + " ";
            }

            ViewBag.Jawi = result.Trim();
            return View();
        }

        /// <summary>
        /// Return rss feed.
        /// See http://blogs.microsoft.co.il/blogs/bursteg/archive/2009/01/11/asp-net-mvc-rss-feed-action-result.aspx
        /// </summary>
        /// <returns></returns>
        public ActionResult Rss()
        {
            Feed feed = new Feed { Title = "Nisan Orders", Description = "Latest updates" };
            var activities = db.Activities.OrderByDescending(a => a.Date).Take(50);
            foreach (var activity in activities)
            {
                // compose nisan case
                FeedItem item = new FeedItem();
                var nisan = db.Nisans.Where(n => n.WorkItemId == activity.WorkItemId).FirstOrDefault();
                if (nisan != null)
                {
                    item.Title = activity.Status.Name.ToLower() + ": " + nisan.SoldTo.DisplayName + " | " + nisan + " - " + nisan.Stock.Name;
                    if (nisan.Assignee != null) item.Creator = nisan.Creator.DisplayName;
                    item.Description = activity.User.UserName + " " + activity.Status.Name.ToLower() + " " + nisan + " - " + nisan.Stock.Name + " at " + activity.Date;
                    item.Published = activity.Date;// DateTime.Now;
                    item.Url = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + "/Nisan/Edit/" + nisan.Id;
                }

                if (!String.IsNullOrEmpty(item.Title))
                    feed.Items.Add(item);
            }

            return View(feed);
        }

        /// <summary>
        /// TODO: Generate svg template.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileStreamResult Svg(int id)
        {
            Nisan nisan = db.Nisans.Find(id);
            MemoryStream stream = new MemoryStream();
            //MemoryWriter mwriter = new MemoryWriter();

            TextWriter writer = new StreamWriter(stream);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            writer.WriteLine("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
            writer.WriteLine("<svg viewBox=\"0 0 600 600\" version=\"1.1\" xmlns:svg=\"http://www.w3.org/2000/svg\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");
            //foreach (UIElement child in this.workspace.Children)
            //{
            //    if (child is System.Windows.Shapes.Path)
            //        writer.WriteLine(ConvertToSvgPathSyntax(child as System.Windows.Shapes.Path));
            //}

            writer.WriteLine("</svg>");
            writer.Flush();

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/text", nisan.Rumi + ".svg");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}