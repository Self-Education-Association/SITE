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

        public ActionResult VoiceNews(int page = 0)
        {
            int pageSize = 20;
            var news = from a in db.Articles where a.Class == ArticleClass.News select a;
            var articles = new ListPage<Article>(news, page, pageSize);

            return View(articles);
        }

        public ActionResult VoicePoints(int page = 0)
        {
            int pageSize = 20;
            var news = from a in db.Articles where a.Class == ArticleClass.Points select a;
            var articles = new ListPage<Article>(news, page, pageSize);

            return View(articles);
        }

        public ActionResult LearnSelf()
        {
            return View();
        }

        public ActionResult TutorOnline()
        {
            var select = db.TutorInformations.ToList();
            var model = new List<HomeTutorOnlineViewModel>();
            foreach (var item in select)
            {
                model.Add(new HomeTutorOnlineViewModel
                {
                    Name = item.Tutor.DisplayName,
                    Position = item.Position,
                    Introduction = item.Introduction,
                    Avatar = item.Avatar,
                    Courses = CourseOperation.List("", true, item.Tutor)
                });
            }

            return View(model);
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

        public ActionResult ActivitiesByMonth(int year, int month)
        {
            DateTime start = new DateTime(year, month, 1, 0, 0, 0);
            DateTime end = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
            var data = db.ActivityOperations.Where(a => a.EndTime >= start && a.EndTime <= end).ToList();
            var model = new List<Calendar>();
            foreach(var item in data)
            {
                model.Add(new Calendar { Id = item.Id, Name = item.Name, ShortContent = item.ShortContent });
            }
            return View(model);
        }
    }
}