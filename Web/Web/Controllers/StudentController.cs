using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.ViewModel;
namespace Website.Controllers
{
    public class StudentController : Controller
    {
        // GET: StudentOrder
        private BaseDbContext db = new BaseDbContext();
        public ActionResult Index()     //对应预约系统主界面
        {
            return View();
        }

        public ActionResult RoomOrder()     //对应预约场地界面 用于显示场地
        {
            var RoomList = db.RoomOperations.Where(t => t.Usable == 0).OrderBy(t => t.Name).ThenBy(t => t.StartTime).ToList();
            return View(RoomList);
        }
        [Authorize]
        public ActionResult DoRoomOrder(Guid? id)       //添加预约场地记录。
        {
            var roomRecord = new RoomRecord();
            roomRecord.RoomOperation = db.RoomOperations.Find(id);
            roomRecord.Id = Guid.NewGuid();
            roomRecord.ActionTime = DateTime.Now;
            roomRecord.Receiver = db.Users.First(u => u.UserName == User.Identity.Name);
            roomRecord.RemarkContent = "未评价";
            roomRecord.RemarkRate = 0;
            roomRecord.RoomOperation.Usable = 1;
            roomRecord.Time =new DateTime(1111, 1, 1, 11, 11, 11); 
            db.RoomRecords.Add(roomRecord);
            db.SaveChanges();
            return View();
        }

        public ActionResult CourseOrderByTeacher()       //对应预约课程界面，按导师排序。
        {
            List<CourseListByCreatorViewModel> viewmodel = new List<CourseListByCreatorViewModel>();
            var aa = (from u in db.Users                    //选出所有老师。
                      where u.Roles.ToString() == "teacher"
                      select u).ToList();
            foreach (User u in aa)                         //选出每个老师对应的课。
            {
                var bb = (from c in db.CourseOperations
                          where c.Creator == u
                          select c).ToList();
                viewmodel.Add(new CourseListByCreatorViewModel { CourseCreator = u, CO = bb });
            }
            return View(viewmodel);
        }

        public ActionResult CourseOrderByStatus()            //课程按课程信息重新排序。
        {
            List<CourseListByStatusViewModel> viewmodel = new List<CourseListByStatusViewModel>();
            var aa = (from u in db.CourseOperations
                      where u.Status == "大课"
                      orderby u.StartTime
                      select u).ToList();
            viewmodel.Add(new CourseListByStatusViewModel { status = "大课", COS = aa });
            var bb = (from u in db.CourseOperations
                      where u.Status == "讨论"
                      orderby u.StartTime
                      select u).ToList();
            viewmodel.Add(new CourseListByStatusViewModel { status = "讨论", COS = bb });
            return View(viewmodel);
        }

        [Authorize]
        public ActionResult DoCourseOrder(Guid? id)       //添加预约课程记录。
        {
            var courseRecord = new CourseRecord();
            courseRecord.CourseOperation = db.CourseOperations.Find(id);
            if (courseRecord.CourseOperation.Count+1<= courseRecord.CourseOperation.Limit)
            {
                courseRecord.CourseOperation.Count++;
                courseRecord.Id =Guid.NewGuid();
                courseRecord.ActionTime = DateTime.Now;
                courseRecord.Receiver = db.Users.First(u => u.UserName == User.Identity.Name);
                courseRecord.RemarkContent = "未评价";
                courseRecord.RemarkRate = 0;
                courseRecord.Time = new DateTime(1111, 1, 1, 11, 11, 11);
                db.CourseRecords.Add(courseRecord);
                db.SaveChanges();
                ViewData["IfSuccess"] = "预约成功";
                return View();
            }
            else
            {
                ViewData["IfSuccess"] = "预约失败";
                return View();      //课程已满显示错误信息。
            }
        }
        public ActionResult StuInfo()
        {
            return View();
        }
        public ActionResult StuCenter()
        {
            return View();
        }
        public ActionResult StuAdd()
        {
            return View();
        }
        public ActionResult StuReg()
        {
            return View();
        }
        public ActionResult StuManage()
        {
            return View();
        }
        public ActionResult StuRaise()
        {
            return View();
        }
        public ActionResult StuMessage()
        {
            return View();
        }
    }
}