using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainModel;
using DomainModel.Initializers;
using DomainModel.Infrastructure;
using System.Data.Entity;

namespace UserInterface.Controllers
{
    public class HomeController : Controller
    {
        AppContext _repo;
        public HomeController()
        {
            _repo = new AppContext();
        }
        public ActionResult Index()
        {
            Database.SetInitializer(new Initializer());
            Dictionary<int, string> images = _repo.CommonImages.ToDictionary(p => p.Id, p => p.Url);
            return View(images);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var cmi = new CommonImageManager();
            if (file != null)
            {
                var res = cmi.AddImage(file, 900, 600, 300, 24);
                if (res != null)
                {
                    res.ForEach(i => _repo.Entry(i).State = EntityState.Added);
                    _repo.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteImage (int id)
        {
            var img = _repo.CommonImages.Find(id);
            if (img != null)
            {
                var imgMan = new CommonImageManager();
                var res = imgMan.DeleteImage(img);
                if (res)
                {
                    _repo.Entry(img).State = EntityState.Deleted;
                    _repo.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
}