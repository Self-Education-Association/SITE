using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.Models;

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
                    TempData["ErrorInfo"] = "无法创建场地，开始时间晚于结束时间。";
                    return View();
                }
                //创建成功返回至列表菜单
                if (roomOperation.Create())
                    return RedirectToAction("Index");
            }
            TempData["ErrorInfo"] = "错误：无法创建场地，对象不存在或无效。";
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
        public ActionResult Edit([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,Status")] RoomOperation roomOperation)
        {
            if (ModelState.IsValid && roomOperation != null)
            {
                if (roomOperation.StartTime >= roomOperation.EndTime)
                {
                    TempData["ErrorInfo"] = "无法完成修改，开始时间晚于结束时间。";
                    return View();
                }
                if (roomOperation.Edit())
                {
                    var user = Extensions.GetContextUser(ref db);
                    var RoomRecords = roomOperation.RoomRecords;
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
                        TempData["ErrorInfo"] = "无法给学生发布修改信息";
                    }
                }
                TempData["ErrorInfo"] = "修改失败!";
            }
            else
                TempData["ErrorInfo"] = "无法修改！对象不存在或无效。";

            return View();
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
                TempData["ErrorInfo"] = "无法删除";
                return View();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Remark(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomRecord roomRecord = db.RoomRecords.Find(id);

            //这句话有问题，为什么找不到room呢。。。
            RoomOperation room = roomRecord.RoomOperation;
            if (roomRecord == null)
            {
                return HttpNotFound();
            }
            if (roomRecord.RemarkRate > 0)
            {

                return View(roomRecord);
            }
            if (room != null)
            {
                if (DateTime.Now > room.EndTime)
                {
                    if (roomRecord.Receiver != null)

                        return View(roomRecord);
                }
                TempData["ErrorInfo"] = "还未到允许评论的时间！";
            }
            else
            {
                TempData["ErrorInfo"] = "该课程没有成员！";
            }
            return RedirectToAction("StudentList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remark([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] RoomRecord roomRecord)
        {
            if (ModelState.IsValid)
            {
                if (roomRecord.Remark())
                    return RedirectToAction("StudentList");
                TempData["ErrorInfo"] = "错误，你提交的评价不符合标准，请更改评分及评价内容！";
            }
            return View();
        }

        public ActionResult StudentList(Guid? Id)
        {
            var user = Extensions.GetContextUser(ref db);
            if (Id == null)
                return View(db.RoomRecords.Where(c => c.RoomOperation.Creator.Id == user.Id).ToList());
            RoomOperation room = db.RoomOperations.Find(Id);
            if (room == null)
            {
                TempData["ErrorInfo"] = "该课程不存在！";
                return RedirectToAction("Index");
            }
            if (room.Creator != user)
            {
                TempData["ErrorInfo"] = "你没有权限对该场地进行评价！";
                return View("Index");
            }
            IQueryable<RoomRecord> studentList = (from a in db.RoomRecords where a.RoomOperation.Id == room.Id select a).Distinct();
            if (studentList.FirstOrDefault() == null)
                return View(db.RoomRecords.ToList());
            return View(studentList);
        }

    }
}