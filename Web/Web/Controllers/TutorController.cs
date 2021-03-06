﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Roles ="Tutor")]
    public class TutorController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private BaseDbContext db = new BaseDbContext();

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //作为tutor自己的课程列表，在使用时若无需检索请输入参数为null或"";
        public ActionResult Index()
        {
            return View(CourseOperation.List("", true));
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { });
            }
            return View(model);
        }
        //返回创建课程的页面
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,Status")] CourseOperation courseOperation)
        {
            if (ModelState.IsValid && courseOperation != null)
            {
                if (courseOperation.StartTime >= courseOperation.EndTime)
                {
                    TempData["Alert"] = "无法创建课程，开始时间晚于结束时间。";
                    return View();
                } 
                //创建成功返回至列表菜单
                if (courseOperation.Create())
                    return RedirectToAction("Index");
            }
            TempData["Alert"] = "错误：无法创建课程，对象不存在或无效。";
            return View(courseOperation);
        }

        public ActionResult Edit(Guid? id)
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
        public ActionResult Edit([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,Status")] CourseOperation courseOperation)
        {
            if (ModelState.IsValid && courseOperation != null)
            {
                if (courseOperation.StartTime >= courseOperation.EndTime)
                {
                    TempData["Alert"] = "无法完成修改，开始时间晚于结束时间。";
                    return View();
                }
                if (courseOperation.Students != null)
                {
                    if (courseOperation.Students.Count > courseOperation.Limit)
                    {
                        TempData["Alert"] = "无法修改！新人数上限小于现有人数，请审核修改内容。";
                        return View();
                    }
                }
                if (courseOperation.Edit())
                {
                    if (courseOperation.Students != null)
                    {
                        foreach (User user in courseOperation.Students)
                        {
                            string title = "课程修改通知";
                            string content = "您好，你选择的课程[" + courseOperation.Name + "]已被修改，请及时查看相关信息，并根据新的课程信息安排你的日程";
                            Message message = new Message(title, content, user.Id, MessageType.System, db);
                            if (!message.Publish())
                            {
                                TempData["Alert"] = "无法给学生发布修改信息";
                                return View();
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }
                TempData["Alert"] = "无法修改！无法连接到服务器.";
                return View();
            }
            TempData["Alert"] = "无法修改！对象不存在或无效。";
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
        public ActionResult DoDelete(Guid? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            CourseOperation courseOperation = db.CourseOperations.Find(Id);
            if (!courseOperation.Delete(ref db))
            {
                TempData["Alert"] = "无法删除";
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
            CourseRecord courseRecord = db.CourseRecords.Find(id);

            //这句话有问题，为什么找不到course呢。。。
            CourseOperation course = courseRecord.CourseOperation;
            if (courseRecord == null)
            {
                return HttpNotFound();
            }
            if (courseRecord.RemarkRate > 0)
            {

                return View(courseRecord);
            }
            if (course != null)
            {
                if (DateTime.Now > course.EndTime)
                {
                    if (course.Students != null)

                        return View(courseRecord);
                }
                    TempData["Alert"] = "还未到允许评论的时间！";
            }
            else
            {
                TempData["Alert"] = "该课程没有成员！";
            }
            return RedirectToAction("StudentList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remark([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] CourseRecord courseRecord)
        {
            if (ModelState.IsValid)
            {
                if (courseRecord.Remark())
                    return RedirectToAction("StudentList");
                TempData["Alert"] = "错误，你提交的评价不符合标准，请更改评分及评价内容！";
            }
            return View();
        }

        public ActionResult StudentList(Guid Id)
        {
            var user = Extensions.GetContextUser(ref db);
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            CourseOperation course = db.CourseOperations.Find(Id);
            if (course == null)
            {
                TempData["Alert"] = "该课程不存在！";
                return RedirectToAction("Index");
            }
            if (course.Creator != user)
            {
                TempData["Alert"] = "你没有权限对该课程进行评价！";
                return View("Index");
            }
            IQueryable<CourseRecord> studentList = (from a in db.CourseRecords where a.CourseOperation.Id == Id select a).Distinct();
            if (studentList.FirstOrDefault() == null)
                return View(db.CourseRecords.ToList());
            return View(studentList);
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

