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
    public class CommentController : Controller
    {
        // GET: Comment
        public ActionResult Index()
        {
            return RedirectToAction("Details", "Article");
        }
        //GET: Comment/Create
        [Authorize]
        public ActionResult Create()
        {
                return View();
        }
        //Post: Comment/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Comment comment)
        {
           
                using (var database = new BlogDbContext())
                {
                    var commentAuthor = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .FirstOrDefault();

                    if (commentAuthor == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    comment.Author = commentAuthor;

                    database.Comments.Add(comment);
                    database.SaveChanges();

                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }
        }
        //GET: Comment/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get comment from the database
                var comment = database.Comments.Where(a => a.Id == id).First();

                // Validating user identity
                if (!IsUserAuthorizedToEdit(comment))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                //Check if comment exists
                if (comment == null)
                {
                    return HttpNotFound();
                }

                //Create the view model
                var model = new Comment();
                model.Id = comment.Id;
                model.Content = comment.Content;
               

                //Pass the view model to view
                return View(model);
            }
        }
        //POST: Comment/Edit
        [HttpPost]
        public ActionResult Edit(Comment model)
        {
            //Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {

                    // Get comment from db
                    var comment = database.Comments.FirstOrDefault(a => a.Id == model.Id);

                    //Set comment properties
                    comment.Content = model.Content;

                    //Save comment state in database
                    database.Entry(comment).State = EntityState.Modified;
                    database.SaveChanges();

                    //Get Article Id from database
                    var articleId = comment.Article.Id;

                    //Redirect to the Article Details page;
                    return RedirectToAction("Details", "Article", new { id = articleId } );
                }
            }

            // If model state is invalid, return to the same view
            return View(model);
        }
        //GET: Comment/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                //Get comment from database
                var comment = database.Comments
                    .Where(c => c.Id == id)
                    .Include(c => c.Author)
                    .First();



                // Validating user identity
                if (!IsUserAuthorizedToEdit(comment))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                //Check if comment exists
                if (comment == null)
                {
                    return HttpNotFound();
                }
                //Pass comment to view
                return View(comment);
            }
        }
        //
        //POST: Comment/Delete
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
                // Get comment from db
                var comment = database.Comments
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                //Check if comment exists
                if (comment == null)
                {
                    return HttpNotFound();
                }

                //Get Article Id from database
                var articleId = comment.Article.Id;

                //Remove comment from db
                database.Comments.Remove(comment);
                database.SaveChanges();

                

                // TO DO Redirect to the Article Details page
                return RedirectToAction("Details", "Article", new { id = articleId });
            }
        }
        private bool IsUserAuthorizedToEdit(Comment comment)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isModerator = this.User.IsInRole("Moderator");
            bool isEditor = this.User.IsInRole("Editor");
            bool isAuthor = comment.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor || isEditor || isModerator;
        }
        //GET Comment/ListUserComments
        public ActionResult ListUserComments()
        {

            using (var database = new BlogDbContext())
            {

                var authorId = database.Users.FirstOrDefault(u => u.UserName == this.User.Identity.Name).Id;

                var comments = database.Comments
                    .Where(a => a.Author.Id.Equals(authorId))
                    .Include(a => a.Author)
                    .ToList();
                return View(comments);
            }
        }
    }
}