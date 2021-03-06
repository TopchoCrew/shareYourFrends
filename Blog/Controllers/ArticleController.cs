﻿using Blog.Controllers;
using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        //
        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        //GET: Article/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                // Get articles from database
                var articles = database.Articles
                    .Include(a => a.Author)
                    .Include(a => a.ProgrammingLanguage)
                    .ToList();

                return View(articles);
            }
        }

        //
        //GET: Article/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new BlogDbContext())
            {

                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.ProgrammingLanguage)
                    .Include(a => a.Comments)
                    .Include("Comments.Author")
                    .First();

                //Get the article from database
                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }
        //GET: Article/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var database = new BlogDbContext())
            {
                var model = new ArticleViewModel();
                model.Categories = database.Categories.OrderBy(c => c.Name).ToList();
                return View(model);
            }

        }
        //
        //POST: Article/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    //Get author id
                    var authorId = database.Users.Where(u => u.UserName == this.User.Identity.Name).First().Id;

                    var article = new Article(authorId, model.Friends_Name, model.Content, model.City);

                    this.SetArticleTags(article, model, database);

                    //Save article in DB
                    database.Articles.Add(article);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }
        //
        //GET: Article/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get article from database
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Category)
                    .Include(a => a.ProgrammingLanguage)
                    .Include(a => a.Comments)
                    .First();

              

                // Validating user identity
                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                ViewBag.TagsString = string.Join(", ", article.ProgrammingLanguage.Select(t => t.Name));

                //Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }
                //Pass article to view
                return View(article);
            }
        }
        //
        //POST: Article/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get article from db
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Category)
                    .Include(a => a.ProgrammingLanguage)
                    .Include(a => a.Comments)
                    .First();

                //Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                //Remove article from db
                database.Articles.Remove(article);
                database.SaveChanges();

                //Redirect to index paga
                return RedirectToAction("Index");
            }
        }
        //
        //GET: Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get article from the database
                var article = database.Articles.Where(a => a.Id == id).First();

                // Validating user identity
                if (!IsUserAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                //Check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                //Create the view model
                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Friends_Name = article.Friends_Name;
                model.Content = article.Content;
                model.City = article.City;
                model.Categories = database.Categories.OrderBy(c => c.Name).ToList();
                model.ProgrammingLanguage = string.Join(", ", article.ProgrammingLanguage.Select(t => t.Name));

                //Pass the view model to view
                return View(model);
            }
        }
        //
        //POST: Article/Edit
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            //Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {

                    // Get article from db
                    var article = database.Articles.FirstOrDefault(a => a.Id == model.Id);

                    //Set article properties
                    article.Friends_Name = model.Friends_Name;
                    article.Content = model.Content;
                    article.City = model.City;
                    this.SetArticleTags(article, model, database);

                    // Save article state in database
                    database.Entry(article).State = EntityState.Modified;
                    database.SaveChanges();

                    //Redirect to the index page
                    return RedirectToAction("Details", new { article.Id});
                }
            }

            // If model state is invalid, return to the same view
            return View(model);
        }

        private void SetArticleTags(Article article, ArticleViewModel model, BlogDbContext database)
        {
            //Split tags
            var tagsStrings = model.ProgrammingLanguage
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLower())
                .Distinct();

            //Clear current article tags
            article.ProgrammingLanguage.Clear();

            //Set new tags
            foreach (var tagString in tagsStrings)
            {
                //Get tag from database by its name
                Tag tag = database.ProgrammingLanguage.FirstOrDefault(t => t.Name.Equals(tagString));

                //If the tag is null, create new tag
                if (tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    database.ProgrammingLanguage.Add(tag);
                }

                //Add tag to article tags
                article.ProgrammingLanguage.Add(tag);
            }
        }

        private bool IsUserAuthorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isModerator = this.User.IsInRole("Moderator");
            bool isEditor = this.User.IsInRole("Editor");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor || isEditor || isModerator;
        }
        //GET Article/ListUserArticle
        public ActionResult ListUserArticle()
        {

            using (var database = new BlogDbContext())
            {

                var authorId = database.Users.First(u => u.UserName == this.User.Identity.Name).Id;

                var articles = database.Articles
                    .Where(a => a.Author.Id.Equals(authorId))
                    .Include(a => a.Author)
                    .Include(a => a.ProgrammingLanguage)
                    .ToList();
                return View(articles);
            }
        }
    }
}