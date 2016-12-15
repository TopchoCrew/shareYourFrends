using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers
{
    public class SliderController : Controller
    {
        // GET: Slider
        public ActionResult Index()
        {
            using (BlogDbContext db = new BlogDbContext())
            {
                return View(db.gallery.ToList());
            }
            //return View();
        }
        //Add Images in slider
        public ActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddImage(HttpPostedFileBase imagePath)
        {
            if (imagePath != null)
            {
                //Upload your pic
                string pic = System.IO.Path.GetFileName(imagePath.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Content/images/"), pic);
                imagePath.SaveAs(path);
                using (BlogDbContext db = new BlogDbContext())
                {
                    Gallery galery = new Gallery { ImagePath = "~/Content/images/" + pic };
                    db.gallery.Add(galery);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        //Delete Images
        public ActionResult DeleteImages()
        {
            using (BlogDbContext db = new BlogDbContext())
            {
                return View(db.gallery.ToList());
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteImages(IEnumerable<int> ImagesIDs)
        {
            if (!IsUserAuthorizedToEdit())
            {

                using (BlogDbContext db = new BlogDbContext())
                {
                    foreach (var id in ImagesIDs)
                    {
                        var image = db.gallery.Single(s => s.ID == id);
                        string imgPath = Server.MapPath(image.ImagePath);
                        db.gallery.Remove(image);
                        if (System.IO.File.Exists(imgPath))
                            System.IO.File.Delete(imgPath);
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("DeleteImages");
        }

        private bool IsUserAuthorizedToEdit()
        {
            bool IsUser = this.User.IsInRole("User");
            return IsUser;
        }
    }
}