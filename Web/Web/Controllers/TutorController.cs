using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class TutorController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        //作为tutor自己的课程列表，在使用时若无需检索请输入参数为null或"";
        public ActionResult Index()
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
        public ActionResult Create([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,State")] CourseOperation courseOperation)
        {
            if (ModelState.IsValid && courseOperation.StartTime <= courseOperation.EndTime)
            {
                //创建成功返回至列表菜单
                if (CourseOperation.Create(courseOperation))
                    return RedirectToAction("Index");
            }
            ViewData["ErrorInfo"] = "错误：无法创建课程，不符合创建课程要求";
            return View(courseOperation);
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
                    if (courseOperation.Students != null)
                    {
                        foreach (User user in courseOperation.Students)
                        {
                            string title = "课程修改通知";
                            string content = "您好，你选择的课程" + courseOperation.Name + "已被修改，请及时查看相关信息，并根据新的课程信息安排你的日程";
                            Message message = new Message(title, content, user, 0,db);
                            if (!message.Publish())
                            {
                                ViewData["ErrorInfo"] = "无法给学生发布修改信息";
                                return View();
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }
                ViewData["ErrorInfo"] = "无法修改,可能的问题：降低了人数上限，无法连接到服务器，修改后的内容超出规定";
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
        public ActionResult DoDelete(Guid id)
        {
            if (!CourseOperation.Delete(id))
            {
                ViewData["ErrorInfo"] = "无法删除";
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
                if (DateTime.Now < course.EndTime)
                {
                    if (course.Students != null)
                        return RedirectToAction("StudentList", course.Id);
                }
                TempData["ErrorInfo"] = "还未到允许评论的时间！";
            }
            else
            {
                TempData["ErrorInfo"] = "该课程没有成员！";
            }
            return RedirectToAction("Index");
            //return RedirectToAction("StudentList");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remark([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] CourseRecord courseRecord)
        {
            if (ModelState.IsValid)
            {
                if (CourseRecord.Remark(courseRecord))
                return RedirectToAction("StudentList");
                ViewData["ErrorInfo"] = "错误，你提交的评价不符合标准，请更改评分及评价内容！";
            }
            return View();
        }
        public ActionResult StudentList(Guid? Id)
        {
            var user = Extensions.GetContextUser(db);
            if(Id==null)
                return View(db.CourseRecords.ToList());
            CourseOperation course = db.CourseOperations.Find(Id);
            if (course == null)
            {
                TempData["ErrorInfo"] = "该课程不存在！";
                return RedirectToAction("Index");
            }
            if (course.Creator != user)
            {
                TempData["ErrorInfo"] = "你没有权限对该课程进行评价！";
                return View("Index");
            }
            IQueryable<CourseRecord> studentList = (from a in db.CourseRecords where a.CourseOperation.Id==course.Id select a ).Distinct();
            if (studentList.FirstOrDefault() == null)
                return View(db.CourseRecords.ToList());
            return View(studentList);
        }

        public ActionResult Create1()
        {
            return View("Create1");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create1([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] CourseRecord courseRecord)
        {
            if (ModelState.IsValid)
            {
                courseRecord.ActionTime = DateTime.Now;
                courseRecord.Time = new DateTime(2000,1,1,0,0,0);
                courseRecord.Id = Guid.NewGuid();
                db.CourseRecords.Add(courseRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courseRecord);
        }
        public ActionResult Calendar()
        {
            var AllCourseInThisMonth = db.CourseOperations.Where(a => a.Creator == Extensions.GetContextUser(db) && a.StartTime.Month == DateTime.Now.Month);
            int Monthdays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var Monthcourse = new IQueryable<CourseOperation>[Monthdays];
            for (int i = 1; i == Monthdays; i++)
            {
                Monthcourse[i - 1] = AllCourseInThisMonth.Where(a => a.StartTime.Day == i);
            }
            var b = Monthcourse.AsEnumerable();
            return View(b);
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

