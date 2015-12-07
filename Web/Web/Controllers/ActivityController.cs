using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ActivityController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        public ActionResult List(string select)
        {
            return View(ActivityOperation.List(select, false));
        }
        
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityOperation ActivityOperation = db.ActivityOperations.Find(id);
            if (ActivityOperation == null)
            {
                return HttpNotFound();
            }
            var model = new ActivityDetailsViewModel
            {
                Name = ActivityOperation.Name,
                Creator = ActivityOperation.Creator,
                Content=ActivityOperation.Content,
                Count=ActivityOperation.Count,
                EndTime=ActivityOperation.EndTime,
                Limit=ActivityOperation.Limit,
                StartTime=ActivityOperation.StartTime
            };
            return View(model);
        }

        public ActionResult Apply(Guid Id)
        {
            if (ModelState.IsValid)
            {
                if (ActivityRecord.Apply(Id))
                    return RedirectToAction("List");
                ViewData["ErrorInfo"] = "你不符合预约要求！";
                return View(Details(Id));
            }
            return View(Details(Id));
        }
        public ActionResult Quit(Guid Id)
        {
            if (ModelState.IsValid)
            {
                if (ActivityRecord.Quit(Id))
                    return RedirectToAction("List");
                ViewData["ErrorInfo"] = "无法退选！";
                return View(Details(Id));
            }
            return View(Details(Id));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
