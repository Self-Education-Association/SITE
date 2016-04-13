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

        [AllowAnonymous]
        public ActionResult Index()
        {
            var model = db.ActivityOperations.Where(a => a.EndTime >= DateTime.Now && a.Enabled == true);

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult History()
        {
            var model = db.ActivityOperations.Where(a => a.EndTime <= DateTime.Now && a.Enabled == true);

            return View(model);
        }

        [AllowAnonymous]
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

        public ActionResult Apply(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(400);
            }
            var activityOperation = db.ActivityOperations.Find(id);
            if (activityOperation == null)
            {
                return HttpNotFound();
            }

            var user = Extensions.GetContextUser(ref db);
            var activityRecord = user.Activity.Find(a => a.ActivityOperation == activityOperation);

            if (activityRecord == null)            //User has no record for this operation
            {
                //Operation status check
                if (activityOperation.Count >= activityOperation.Limit)
                {
                    TempData["Alert"] = "该活动已满！";
                    return RedirectToAction("Details", new { @id = id });
                }
                if (DateTime.Now > activityOperation.StartTime)
                {
                    TempData["Alert"] = "该活动现在不可预约！";
                    return RedirectToAction("Details", new { @id = id });
                }

                //Try to apply for this operation
                activityRecord = new ActivityRecord();
                if (activityRecord.Apply(id))
                {
                    TempData["Alert"] = "活动预约成功！";
                    return RedirectToAction("Index");
                }
                TempData["Alert"] = "你不符合预约要求！";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Alert"] = "您已选过该活动！";
                return RedirectToAction("Details", new { @id = id });
            }
        }
        public ActionResult Quit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);

            var activityOperation = db.ActivityOperations.Find(id);
            if (activityOperation == null)
                return HttpNotFound();

            var user = Extensions.GetContextUser(ref db);
            var activityRecord = user.Activity.Find(a => a.ActivityOperation == activityOperation);
            if (activityRecord == null)
            {
                TempData["Alert"] = "您未预约过该活动！";
                return RedirectToAction("Index");
            }
            if (DateTime.Now > activityOperation.StartTime)
            {
                TempData["Alert"] = "现在不是可取消预约的时间！";
                return RedirectToAction("Index");
            }
            if (activityRecord.Quit(id))
            {
                TempData["Alert"] = "活动取消预约成功！";
                return RedirectToAction("Index");
            }
            TempData["Alert"] = "无法取消预约！";

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
