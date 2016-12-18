using Blog.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("ListCategories");
        }

        public ActionResult ListCategories()
        {
            using (var database = new BlogDbContext())
            {
                var categories = database.Categories.Include(c => c.Articles).OrderBy(c => c.Name).ToList();

                return View(categories);
            }
        }
        public ActionResult ListArticles(int? categoryId)
        {
            if (categoryId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new BlogDbContext())
            {
                var articles = database.Articles
                    .Where(a => a.City == categoryId)
                    .Include(a => a.Author)
                    .Include(a => a.ProgrammingLanguage)
                    .ToList();

                return View(articles);
            }
        }
        //add by nasko.
        // GET: Home
        public ActionResult ReCaptcha()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FormSubmit()
        {
            //validаtе google recaptcha here
            var response = Request["g-recaptcha-response"];
            string secretKey = "6LdoEg8UAAAAAJBpG_6lcgapOAZDuYO0Cup3_h8X";
            var client = new WebClient();// using system.net
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            ViewBag.Message = status ? "Google reCaptcha validation success" : "Google reCaptcha validation failed";

            //When  you will post form for save data, you should check both the model validation and google recaptcha validation

            //ex.
            if (ModelState.IsValid && status)
            {
                return View("ifView");
            }

            //Here I am returning to Index page>>> make custom view
            return View("robbot");
        }
        //end add by nasko

    }
}