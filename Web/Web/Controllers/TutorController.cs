using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class TutorController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        //作为tutor自己的课程列表，在使用时若无需检索请输入参数为null或"";
        public ActionResult Courselist()
        {
            return View(CourseOperation.List("", true));
        }
        //返回创建课程的页面
        public ActionResult CreateCourse()
        {
            return View("CreateCourse");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCourse([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,State")] CourseOperation courseOperation)
        {
            if (ModelState.IsValid && courseOperation.StartTime <= courseOperation.EndTime)
            {
                //创建成功返回至列表菜单
                if (CourseOperation.Create(courseOperation))
                    return View("Courselist");
            }
            return View();
        }
        public ActionResult Courseset(Guid? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Courseset([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,State")] CourseOperation courseOperation)
        {
            if (ModelState.IsValid)
            {
                if(CourseOperation.Update(courseOperation))
                {
                    return RedirectToAction("List");
                    //发送信息
                }
            }
            return View();
        }
        public ActionResult Delete(Guid? id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            if (CourseOperation.Delete(id))
            { 
                //错误信息
            }
            return RedirectToAction("List");
        }
        public ActionResult StManage(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseRecord courseRecord = db.CourseRecords.Find(id);
            if (courseRecord == null)
            {
                return HttpNotFound();
            }
            if (courseRecord.RemarkRate <= 5 && courseRecord.RemarkRate >= 1 && courseRecord.RemarkContent != "未评价")
            {
                return View(courseRecord);
            }
            return View("TuManage");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StManage([Bind(Include = "Id,ActionTime,RecordContent,RecordRate,Time")] CourseRecord courseRecord)
        {
            if (ModelState.IsValid)
            {
                if(CourseRecord.Remark(courseRecord))
                return RedirectToAction("TuManage","");
            }
            return View("StManage");
        }
        public ActionResult TuManage(Guid Id)
        {
            var user = (from a in db.Users where a.UserName == HttpContext.User.Identity.Name select a).First();
            CourseOperation course = db.CourseOperations.Find(Id);
            if (user == null)
                //返回登陆界面
                return null;
            if (course == null)
                //返回错误信息：该课程不存在
                return null;
            if (course.Creator != user)
                //返回错误信息：没有权限
                return null;
            var studentList = db.CourseRecords.Where(a => a.CourseOperation == course);
            return View(studentList.ToList());
        }

        public ActionResult TuActivity()
        {
            return View();
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
