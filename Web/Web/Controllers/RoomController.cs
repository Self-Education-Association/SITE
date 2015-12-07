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
    public class RoomController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        public ActionResult List(string select)
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
            var model = new RoomDetailsViewModel
            {
                Usable = RoomOperation.Usable,
                Name = RoomOperation.Name,
                Creator = RoomOperation.Creator,
                Content = RoomOperation.Content,
                EndTime = RoomOperation.EndTime,
                StartTime = RoomOperation.StartTime
            };
            return View(model);
        }

        public ActionResult Apply(Guid Id)
        {
            if (ModelState.IsValid)
            {
                if (RoomRecord.Apply(Id))
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
                if (RoomRecord.Quit(Id))
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