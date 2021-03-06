﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Web.Security;
using HLGranite.Mvc.Models;
using System.Security;
using System.Data.Entity.Validation;
using System.Text;

namespace HLGranite.Mvc.Controllers
{
    public class UserController : Controller
    {
        private hlgraniteEntities db = new hlgraniteEntities();

        /// <summary>
        /// List out all Malaysia states in dropdownlist.
        /// </summary>
        public static IEnumerable<SelectListItem> States = new[]{
            new SelectListItem{Text="Johor", Value="Johor"},
            new SelectListItem{Text="Kedah", Value="Kedah"},
            new SelectListItem{Text="Kelantan", Value="Kelantan"},
            new SelectListItem{Text="Kuala Lumpur", Value="Kuala Lumpur"},
            new SelectListItem{Text="Melaka", Value="Melaka"},
            new SelectListItem{Text="Negeri Sembilan", Value="Negeri Sembilan"},
            new SelectListItem{Text="Pahang", Value="Pahang"},
            new SelectListItem{Text="Penang", Value="Penang"},
            new SelectListItem{Text="Perak", Value="Perak"},
            new SelectListItem{Text="Perlis", Value="Perlis"},
            new SelectListItem{Text="Pulau Labuan", Value="Pulau Labuan"},
            new SelectListItem{Text="Sabah", Value="Sabah"},
            new SelectListItem{Text="Sarawak", Value="Sarawak"},
            new SelectListItem{Text="Selangor", Value="Selangor"},
            new SelectListItem{Text="Terengganu", Value="Terengganu"},
        };

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {
            //if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            if (ModelState.IsValid)
            {
                if (IsValid(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl); // TODO: returnUrl always return null
                    else
                        return RedirectToAction("Index", "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Validate user with own table.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsValid(string userName, string password)
        {
            // developer passport
            if (userName == "admin" && password == "admin")
                return true;

            var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
            if (user == null)
                return false;
            else if(Crypto.VerifyHashedPassword(user.Password, password))
                return true;
            else
                return false;
        }

        //
        // GET: /User/
        [AuthorizeAdmin]
        public ActionResult Index(string type, string searchString)
        {
            ViewBag.Type = new SelectList(db.UserTypes, "Id", "Type");

            var users = db.Users.Include(u => u.UserType);
            if (!String.IsNullOrEmpty(type))
            {
                int id = Convert.ToInt32(type);
                users = users.Where(u => u.UserTypeId == id);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                users = users.Where(u => u.UserName.ToLower().Contains(searchString) || u.FirstName.ToLower().Contains(searchString) || u.LastName.ToLower().Contains(searchString));
            }

            users = users.OrderBy(u => u.UserType.Id).ThenBy(u => u.UserName);
            return View(users.ToList());
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create
        [AuthorizeAdmin]
        public ActionResult Create()
        {
            User user = db.Users.Create();
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Type");
            return View(user);
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeAdmin]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.UserTypeId == 0)
                    user.UserTypeId = HLGranite.Mvc.Models.User.CUSTOMER_TYPE_ID;
                if(!String.IsNullOrEmpty(user.Password))
                    user.Password = Crypto.HashPassword(user.Password);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Type", user.UserTypeId);
            return View(user);
        }

        //
        // GET: /User/Register

        public ActionResult Register()
        {
            User user = db.Users.Create();
            user.UserTypeId = HLGranite.Mvc.Models.User.CUSTOMER_TYPE_ID;
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Type");
            return View(user);
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.UserTypeId == 0)
                    user.UserTypeId = HLGranite.Mvc.Models.User.CUSTOMER_TYPE_ID;
                if (!String.IsNullOrEmpty(user.Password))
                    user.Password = Crypto.HashPassword(user.Password);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Type", user.UserTypeId);
            return View(user);
        }

        //
        // GET: /User/Edit/5
        [AuthorizeOwner]
        public ActionResult Edit(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Type", user.UserTypeId);
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeOwner]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = Crypto.HashPassword(user.Password);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserTypeId = new SelectList(db.UserTypes, "Id", "Type", user.UserTypeId);
            return View(user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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