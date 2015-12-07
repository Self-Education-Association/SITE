using System;
using System.Collections.Generic;
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
        public ActionResult CourseList()
        {
            return View(CourseOperation.List("", true));
        }
        //返回创建课程的页面
        public ActionResult Create()
        {
            return View("Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCourse([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,State")] CourseOperation courseOperation)
        {
            if (ModelState.IsValid && courseOperation.StartTime <= courseOperation.EndTime)
            {
                //创建成功返回至列表菜单
                if (CourseOperation.Create(courseOperation))
                    return View("CourseList");
            }
            ViewData["ErrorInfo"] = "错误：无法创建课程，不符合创建课程要求";
            return View();
        }
        public ActionResult Update(Guid? id)
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
        public ActionResult Update([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,State")] CourseOperation courseOperation)
        {
            if (ModelState.IsValid)
            {
                if (CourseOperation.Update(courseOperation))
                {
                    foreach (User user in courseOperation.Students)
                    {
                        string title = "课程修改通知";
                        string content = "您好，你选择的课程" + courseOperation.Name + "已被修改，请及时查看相关信息，并根据新的课程信息安排你的日程";
                        Message message = new Message(title, content, user, 0);
                        if (!message.Publish())
                        {
                            ViewData["ErrorInfo"] = "无法给学生发布修改信息";
                            return View();
                        }
                    }
                    return RedirectToAction("CourseList");
                }
                ViewData["ErrorInfo"] = "无法修改";
                return View();
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
            if (!CourseOperation.Delete(id))
            {
                ViewData["ErrorInfo"] = "无法删除";
                return View();
            }
            return RedirectToAction("List");
        }
        public ActionResult Remark(Guid? id)
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
            return View("StudentList");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remark([Bind(Include = "Id,ActionTime,RecordContent,RecordRate,Time")] CourseRecord courseRecord)
        {
            if (ModelState.IsValid)
            {
                if (CourseRecord.Remark(courseRecord))
                    return RedirectToAction("StudentList");
                ViewData["ErrorInfo"] = "错误，你提交的评价不符合标准，请更改评分及评价内容！";
            }
            return View();
        }
        public ActionResult StudentList(Guid Id)
        {
            var user = (from a in db.Users where a.UserName == HttpContext.User.Identity.Name select a).First();
            CourseOperation course = db.CourseOperations.Find(Id);
            if (user == null)
                return View("~/Account/Login");
            if (course == null)
            {
                ViewData["ErrorInfo"] = "该课程不存在！";
                return View("CourseList");
            }
            if (course.Creator != user)
            {
                ViewData["ErrorInfo"] = "你没有权限对该课程进行评价！";
                return View("CourseList");
            }
            var studentList = db.CourseRecords.Where(a => a.CourseOperation == course);
            return View(studentList.ToList());
        }

        public ActionResult Calendar()
        {
            var AllCourseInThisMonth = db.CourseOperations.Where(a => a.Creator == Extensions.GetCurrentUser() && a.StartTime.Month == DateTime.Now.Month);
            int Monthdays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var Monthcourse = new IQueryable<CourseOperation>[Monthdays];
            for (int i = 1; i == Monthdays; i++)
            {
                Monthcourse[i - 1] = AllCourseInThisMonth.Where(a => a.StartTime.Day == i);
            }
            return View(Monthcourse);
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

