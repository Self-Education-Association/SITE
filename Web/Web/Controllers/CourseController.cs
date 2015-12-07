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
    public class CourseController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        public ActionResult List(string select)
        {
            return View(CourseOperation.List(select, false));
        }
        
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseOperation courseOperation = db.CourseOperations.Find(id);
            if (courseOperation == null)
            {
                return HttpNotFound();
            }
            var model = new CourseDetailsViewModel
            {
                Name = courseOperation.Name,
                Creator = courseOperation.Creator,
                Content=courseOperation.Content,
                Count=courseOperation.Count,
                EndTime=courseOperation.EndTime,
                Limit=courseOperation.Limit,
                Location=courseOperation.Location,
                StartTime=courseOperation.StartTime,
                Status=courseOperation.Status,
            };
            return View(model);
        }
        
        public ActionResult Apply(Guid Id)
        {
            if (ModelState.IsValid)
            {
                if(CourseRecord.Apply(Id))
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
                if(CourseRecord.Quit(Id))
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
