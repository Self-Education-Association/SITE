using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Ganss.XSS;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.IO;

namespace Web.Controllers
{
    public class RoomManageController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        public ActionResult Index()
        {
            return View(RoomOperation.List("", true));
        }

        //返回创建场地的页面
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,Status")] RoomOperation roomOperation)
        {
            if (ModelState.IsValid && roomOperation != null)
            {
                if (roomOperation.StartTime >= roomOperation.EndTime)
                {
                    TempData["Alert"] = "无法创建场地，开始时间晚于结束时间。";
                    return View();
                }
                //创建成功返回至列表菜单
                if (roomOperation.Create())
                    return RedirectToAction("Index");
            }
            TempData["Alert"] = "错误：无法创建场地，对象不存在或无效。";
            return View(roomOperation);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomOperation roomOperation = db.RoomOperations.Find(id);
            if (roomOperation == null)
            {
                return HttpNotFound();
            }
            return View(roomOperation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(RoomOperation roomOperation)
        {
            if (ModelState.IsValid && roomOperation != null)
            {
                var s = new HtmlSanitizer();
                roomOperation.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"])); ;
                if (roomOperation.StartTime >= roomOperation.EndTime)
                {
                    TempData["Alert"] = "无法完成修改，开始时间晚于结束时间。";
                    return View();
                }
                if (roomOperation.Edit())
                {
                    var user = Extensions.GetContextUser(ref db);
                    var RoomRecords = roomOperation.RoomRecords;
                    if (RoomRecords != null)
                    {
                        var lastRecord = RoomRecords.Where(r => r.ActionTime.AddDays(7.0) > r.RoomOperation.StartTime);
                        if (RoomRecords != null && lastRecord != null)
                        {
                            string title = "场地修改通知";
                            string content = "您好，你选择的场地[" + roomOperation.Name + "]已被修改，请及时查看相关信息，并根据新的场地信息安排你的日程";
                            Message message = new Message(title, content, lastRecord.First().Receiver.Id, MessageType.System, db);
                            if (message.Publish())
                            {
                                return RedirectToAction("Index");
                            }
                            TempData["Alert"] = "无法给学生发布修改信息";
                        }
                    }
                }
                else
                    TempData["Alert"] = "修改失败!";
            }
            else
                TempData["Alert"] = "无法修改！对象不存在或无效。";

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomOperation roomOperation = db.RoomOperations.Find(id);
            if (roomOperation == null)
            {
                return HttpNotFound();
            }
            return View(roomOperation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DoDelete(Guid? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            RoomOperation roomOperation = db.RoomOperations.Find(Id);
            if (!roomOperation.Delete(ref db))
            {
                TempData["Alert"] = "无法删除";
                return View();
            }
            return RedirectToAction("Index");
        }

    }
}