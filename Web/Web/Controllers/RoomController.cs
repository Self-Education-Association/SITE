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
    public class RoomController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        public ActionResult Index(string select)
        {
            return View(RoomOperation.List(select, false));
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomOperation RoomOperation = db.RoomOperations.Find(id);
            if (RoomOperation == null)
            {
                return HttpNotFound();
            }
            return View(RoomOperation);
        }

        public ActionResult Apply(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var RoomOperation = db.RoomOperations.Find(Id);
                var user = db.Users.Find(HttpContext.User.Identity.GetUserId());
                if (RoomOperation.Usable == false)
                {
                    TempData["ErrorInfo"] = "该场地已被使用！";
                    return RedirectToAction("Index");
                }
                if (DateTime.Now > RoomOperation.StartTime)
                {
                    TempData["ErrorInfo"] = "该场地现在不可预约！";
                    return RedirectToAction("Index");
                }
                var roomRecord = new RoomRecord();
                if (roomRecord.Apply(Id))
                    return RedirectToAction("Index"); ;
                TempData["ErrorInfo"] = "你不符合预约要求！";
            }
            return RedirectToAction("Index");
        }
        public ActionResult Quit(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var RoomOperation = db.RoomOperations.Find(Id);
                var user = db.Users.Find(HttpContext.User.Identity.GetUserId());
                var roomRecord = (from a in db.RoomRecords where a.RoomOperation.Id == RoomOperation.Id && a.Receiver.Id == user.Id select a).FirstOrDefault();
                if (roomRecord == default(RoomRecord) | roomRecord == null)
                {
                    TempData["ErrorInfo"] = "您未选该场地！";
                    return RedirectToAction("Index");
                }
                if (DateTime.Now > RoomOperation.StartTime)
                {
                    TempData["ErrorInfo"] = "现在不是可退选的时间！";
                    return RedirectToAction("Index");
                }
                if (roomRecord.Quit(Id))
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
