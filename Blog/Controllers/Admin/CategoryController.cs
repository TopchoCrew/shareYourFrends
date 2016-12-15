using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers.Admin
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            if (IsUserAuthorized())
            {
                return RedirectToAction("List");
            }
            return RedirectToAction("Login", "Account");
        }
        //
        //GET: Category/List
        public ActionResult List()
        {
            if (IsUserAuthorized())
            {
                using (var database = new BlogDbContext())
                {
                    var categories = database.Categories.ToList();

                    return View(categories);
                }
            }
            return RedirectToAction("Login", "Account");
        }
        //
        //GET: Category/Create
        public ActionResult Create()
        {
            if (IsUserAuthorized())
            {
                return View();
            }
            return RedirectToAction("Login", "Account");
        }
        //
        //POST: Category/Create
        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (IsUserAuthorized())
            {
                if (ModelState.IsValid)
                {
                    using (var database = new BlogDbContext())
                    {
                        database.Categories.Add(category);
                        database.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }

                return View(category);
            }

            return RedirectToAction("Login", "Account");
        }
        //
        //GET: Category/Edit
        public ActionResult Edit (int? id)
        {
            if (IsUserAuthorized())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                using (var database = new BlogDbContext())
                {
                    var category = database.Categories.FirstOrDefault(c => c.Id == id);

                    if (category == null)
                    {
                        return HttpNotFound();
                    }

                    return View(category);
                }
            }

            return RedirectToAction("Login", "Account");
        }
        //
        //POST: Category/Edit
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (IsUserAuthorized())
            {
                if (ModelState.IsValid)
                {
                    using (var database = new BlogDbContext())
                    {
                        database.Entry(category).State = EntityState.Modified;
                        database.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }

                return View(category);
            }

            return RedirectToAction("Login", "Account");
        }
        //
        //GET: Category/Delete
        public ActionResult Delete(int? id)
        {
            if (IsUserAuthorized())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                using (var database = new BlogDbContext())
                {
                    var category = database.Categories.FirstOrDefault(c => c.Id == id);

                    if (category == null)
                    {
                        return HttpNotFound();
                    }

                    return View(category);
                }
            }

            return RedirectToAction("Login", "Account");
        }
        //
        //POST: Category/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (IsUserAuthorized())
            {
                using (var database = new BlogDbContext())
                {
                    var category = database.Categories.FirstOrDefault(c => c.Id == id);

                    var categoryArticles = category.Articles.ToList();

                    foreach (var article in categoryArticles)
                    {
                        database.Articles.Remove(article);
                    }

                    database.Categories.Remove(category);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Login", "Account");
        }
        private bool IsUserAuthorized()
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isModerator = this.User.IsInRole("Moderator");

            return isAdmin || isModerator;
        }
    }
   
}