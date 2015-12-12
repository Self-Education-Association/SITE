using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        BaseDbContext db = new BaseDbContext();

        public ActionResult Index()
        {
            var model = new HomeIndexViewModel();
            int ShowActivityCount = 3;


            if (db.ActivityOperations.Count() < ShowActivityCount)
            {
                model.LatestActivitys = db.ActivityOperations.OrderBy(u => u.Time).Take(db.ActivityOperations.Count()).ToList();
            }
            else
            {
                model.LatestActivitys = db.ActivityOperations.OrderBy(u => u.Time).Take(ShowActivityCount).ToList();
            }

            model.LatestActivitys.Add(new ActivityOperation { Name = "123", Content = "456" });
            model.LatestActivitys.Add(new ActivityOperation { Name = "123", Content = "456" });
            model.LatestActivitys.Add(new ActivityOperation { Name = "123", Content = "456" });
            model.LatestActivitys.Add(new ActivityOperation { Name = "123", Content = "456" });

            if (db.Materials.Where(u => u.Type == MaterialType.Slider).Count() == 0)
                model.Sliders = new List<Material>();
            else
                model.Sliders = db.Materials.Where(u => u.Type == MaterialType.Slider).ToList();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }

        public ActionResult Download(int page = 0)
        {
            int pageSize = 5;

            var paginatedNews = new ListPage<Material>(db.Materials.Where(m => m.Type == MaterialType.Download), page, pageSize);

            return View(paginatedNews);
        }
        public ActionResult Constructing()
        {
            return View();
        }
    }
}