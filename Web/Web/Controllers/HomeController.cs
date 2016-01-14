using System;
using System.Collections.Generic;
using System.IO;
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

            if (db.Materials.Where(u => u.Type == MaterialType.Slider).Count() == 0)
                model.Sliders = new List<Material>();
            else
                model.Sliders = db.Materials.Where(u => u.Type == MaterialType.Slider).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(upload.FileName);
            string absolutFileName = Server.MapPath("~/") + "UserUpload/Image/" + fileName;
            upload.SaveAs(absolutFileName);

            var url = "/UserUpload/Image/" + fileName;
            var CKEditorFuncNum = System.Web.HttpContext.Current.Request["CKEditorFuncNum"];

            //上传成功后，我们还需要通过以下的一个脚本把图片返回到第一个tab选项
            return Content("<script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\");</script>");
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

        public ActionResult Download(MaterialType type = MaterialType.Download, int page = 0)
        {
            int pageSize = 20;
            switch (type)
            {
                case MaterialType.Download:
                    ViewBag.Title = "模板文件";
                    break;
                case MaterialType.Activity:
                    ViewBag.Title = "活动资料";
                    break;
                default:
                    ViewBag.Title = "模板文件";
                    type = MaterialType.Download;
                    break;
            }

            var materials = new ListPage<Material>(db.Materials.Where(m => m.Type == type), page, pageSize);

            return View(materials);
        }

        public ActionResult Constructing()
        {
            return View();
        }

        public ActionResult VoiceNews(int page=0)
        {
            int pageSize = 20;
            var articles = new ListPage<Article>(db.Articles, page, pageSize);

            return View(articles);
        }

        public ActionResult VoicePoints()
        {
            return View();
        }

        public ActionResult LearnSelf()
        {
            return View();
        }

        public ActionResult TutorOnline()
        {
            return View();
        }

        public ActionResult TutorJoin()
        {
            return View();
        }

        public ActionResult AboutSite()
        {
            return View();
        }

        public ActionResult AboutNew()
        {
            return View();
        }

        public ActionResult VoiceRace()
        {
            return View();
        }
        public ActionResult NewsPage1()
        {
            return View();
        }
    }
}