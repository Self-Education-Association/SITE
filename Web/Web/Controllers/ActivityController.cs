using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        public ActionResult Index(string select)
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
            return View(ActivityOperation);
        }

        public ActionResult Apply(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var ActivityOperation = db.ActivityOperations.Find(Id);
                var user = db.Users.Find(HttpContext.User.Identity.GetUserId());
                var Activityrecord = (from a in db.ActivityRecords where a.ActivityOperation.Id == ActivityOperation.Id && a.Receiver.Id == user.Id select a).FirstOrDefault();
                if (Activityrecord != null)
                {
                        TempData["ErrorInfo"] = "您已选过该活动！";
                        return RedirectToAction("Index");
                }
                if (ActivityOperation.Count >= ActivityOperation.Limit)
                {
                    TempData["ErrorInfo"] = "该活动已满！";
                    return RedirectToAction("Index");
                }
                if (DateTime.Now > ActivityOperation.StartTime)
                {
                    TempData["ErrorInfo"] = "该活动现在不可预约！";
                    return RedirectToAction("Index");
                }
                if (ActivityRecord.Apply(Id))
                    return RedirectToAction("Index"); ;
                TempData["ErrorInfo"] = "你不符合预约要求！";
            }
            return RedirectToAction("Index");
        }
        public ActionResult Quit(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var ActivityOperation = db.ActivityOperations.Find(Id);
                var user = db.Users.Find(HttpContext.User.Identity.GetUserId());
                var Activityrecord = (from a in db.ActivityRecords where a.ActivityOperation.Id == ActivityOperation.Id && a.Receiver.Id == user.Id select a).FirstOrDefault();
                if (Activityrecord == null)
                {
                        TempData["ErrorInfo"] = "您未选过该活动！";
                        return RedirectToAction("Index");
                }
                if (DateTime.Now > ActivityOperation.StartTime)
                {
                    TempData["ErrorInfo"] = "现在不是可退选的时间！";
                    return RedirectToAction("Index");
                }
                if (ActivityRecord.Quit(Id))
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
