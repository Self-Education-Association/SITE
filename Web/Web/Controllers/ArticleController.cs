﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ArticleController : Controller
    {
        BaseDbContext db = new BaseDbContext();

        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(ArticleClass articleClass=ArticleClass.News,int pageSize=10,int page=0)
        {
            var model = new ListPage<Article>(db.Articles.Where(a => a.Class == articleClass), page, pageSize);

            return View(model);
        }

        public ActionResult Show(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = db.Articles.Find(id);
            if (model == null)
                return HttpNotFound();

            switch(model.Class)
            {
                case ArticleClass.News:
                    return View("News", model);
                case ArticleClass.Points:
                    return View("Points", model);
                default:
                    return RedirectToAction("List");
            }
        }
    }
}