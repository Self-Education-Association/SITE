using Microsoft.AspNet.Identity;
using System;
using System.Net;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        public ActionResult Index(string select)
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
            return View(courseOperation);
        }
        
        public ActionResult Apply(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var CourseOperation = db.CourseOperations.Find(Id);
                if (CourseOperation.Students != null)
                {
                    if (CourseOperation.Students.Contains(db.Users.Find(HttpContext.User.Identity.GetUserId())))
                    {
                        TempData["ErrorInfo"] = "您已选过该课程！";
                        return RedirectToAction("Index");
                    }
                }
                if (CourseOperation.Count >= CourseOperation.Limit)
                {
                    TempData["ErrorInfo"] = "该课程已满！";
                    return RedirectToAction("Index");
                }
                if ( DateTime.Now > CourseOperation.StartTime)
                {
                    TempData["ErrorInfo"] = "该课程现在不可预约！";
                    return RedirectToAction("Index");
                }
                if (CourseRecord.Apply(Id))
                    return RedirectToAction("Index");;
                TempData["ErrorInfo"] = "你不符合预约要求！";
            }
            return RedirectToAction("Index");
        }
        public ActionResult Quit(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var CourseOperation = db.CourseOperations.Find(Id);
                if (CourseOperation.Students == null)
                {
                        TempData["ErrorInfo"] = "您未选过该课程！";
                        return RedirectToAction("Index");
                }
                if (CourseOperation.Students != null)
                {
                    if (!CourseOperation.Students.Contains(db.Users.Find(HttpContext.User.Identity.GetUserId())))
                    {
                        TempData["ErrorInfo"] = "您未选过该课程！";
                        return RedirectToAction("Index");
                    }
                }
                if (DateTime.Now > CourseOperation.StartTime)
                {
                    TempData["ErrorInfo"] = "现在不是可退选的时间！";
                    return RedirectToAction("Index");
                }
                    if (CourseRecord.Quit(Id))
                return RedirectToAction("Index");
                TempData["ErrorInfo"] = "无法退选！";
            }
            return RedirectToAction("Index"); 
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
