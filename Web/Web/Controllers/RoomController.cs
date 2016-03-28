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
            foreach (RoomOperation roomOperation in db.RoomOperations)
            {
                if (DateTime.Now > roomOperation.EndTime)
                {
                    roomOperation.StartTime = roomOperation.StartTime.AddDays(7.0);
                    roomOperation.EndTime = roomOperation.EndTime.AddDays(7.0);
                    roomOperation.Usable = true;
                }
            }
            db.SaveChanges();
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
                if (RoomOperation == null)
                {
                    TempData["Alert"] = "该场地不存在！";
                    return RedirectToAction("Index");
                }
                if (RoomOperation.Usable == false)
                {
                    TempData["Alert"] = "该场地已被使用！";
                    return RedirectToAction("Index");
                }
                if (DateTime.Now > RoomOperation.StartTime)
                {
                    TempData["Alert"] = "该场地现在不可预约！";
                    return RedirectToAction("Index");
                }
                var roomRecord = new RoomRecord();
                if (roomRecord.Apply(Id))
                    return RedirectToAction("Index"); ;
                TempData["Alert"] = "你不符合预约要求！";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Quit(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var roomOperation = db.RoomOperations.Find(Id);
                var user = Extensions.GetContextUser(ref db);
                if (roomOperation == null)
                    return new HttpStatusCodeResult(404);
                if (DateTime.Now > roomOperation.StartTime)
                {
                    TempData["Alert"] = "现在不是可退选的时间！";
                }
                if (roomOperation.Usable == false)
                {
                    var RoomRecords = roomOperation.RoomRecords.Where(c => c.Receiver.Id == user.Id);
                    var lastRecord = RoomRecords.Where(r => r.ActionTime.AddDays(7.0) > r.RoomOperation.StartTime);
                    if (RoomRecords != null && lastRecord != null && lastRecord.First().Receiver == user)
                    {
                        db.RoomRecords.Remove(lastRecord.First());
                        roomOperation.Usable = true;
                        db.SaveChanges();
                        if (roomOperation.Usable != false)
                        {
                            TempData["Alert"] = "退选成功";

                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            TempData["Alert"] = "退选失败";

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
