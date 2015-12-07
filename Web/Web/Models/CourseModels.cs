using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Web.Mvc;
using Web.Models;

namespace Web.Models
{
    public class CourseOperation : Operation
    {
        public int Count { get; set; }

        public int Limit { get; set; }

        public string Status { get; set; }

        public List<User> Students { get; set; }

        public string Location { get; set; }
        public static bool Create(CourseOperation courseOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                courseOperation.Id = Guid.NewGuid();
                courseOperation.Time = DateTime.Now;
                courseOperation.Count = 0;
                courseOperation.State = 1;
                courseOperation.Time = DateTime.Now;
                courseOperation.Creator = Extensions.GetCurrentUser();
                db.CourseOperations.Add(courseOperation);
                db.SaveChanges();
                if (db.CourseOperations.Find(courseOperation.Id) != null)
                    return true;
                return false;
            }
        }
        public static bool Update([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,State")] CourseOperation courseOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                //try
                //{
                    //if (courseOperation.Students.Count > courseOperation.Limit)
                    //    return false;
                    db.Entry(courseOperation).State = EntityState.Modified;
                    db.SaveChanges();
                    if (db.CourseOperations.Find(courseOperation.Id) != null)
                        return true;
                    return false;
                //}
                //catch
                //{
                //    return false;
                //}
            }
        }
        public static bool Delete(Guid id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                CourseOperation courseOperation = db.CourseOperations.Find(id);
                courseOperation.State = 0;
                db.Entry(courseOperation).State = EntityState.Modified;
                db.SaveChanges();
                if (courseOperation.State == 0)
                    return true;
                return false;
            }
        }
        public static List<CourseOperation> List(string select, bool IsTeacher)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                int pageSize = 5;
                int page = 0;
                IQueryable<CourseOperation> Course;
                if (IsTeacher)
                {
                    var user = Extensions.GetCurrentUser();
                    if (select == null | select == "")
                    { Course = (from a in db.CourseOperations where a.State != 0 && a.Creator == user orderby a.Name select a).AsQueryable(); }
                    else
                    {
                        Course = (from a in db.CourseOperations
                                  where a.State != 0 && a.Creator == user && a.Name == @select
                                  orderby a.Name
                                  select a).AsQueryable();
                    }
                }
                else
                {
                    if (select == null)
                    {
                        Course = (from a in db.CourseOperations
                                  where a.State != 0 && a.StartTime > DateTime.Now
                                  orderby a.Time
                                  select a).AsQueryable();
                    }
                    else
                    {
                        Course = (from a in db.CourseOperations
                                  where a.State != 0 && a.Name == @select && a.StartTime > DateTime.Now
                                  orderby a.Time
                                  select a).AsQueryable();
                    }
                }
                var paginatedNews = new ListPage<CourseOperation>(Course, page, pageSize);
                return paginatedNews;
            }
        }
    }
    public enum state
    {
        已被删除 = 0,
        可选 = 1,
        正在进行 = 2,
        已结束 = 3,
    }
    public class CourseRecord : Remark
    {
        public CourseOperation CourseOperation { get; set; }
        public static bool Remark(CourseRecord courseRecord)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                if (courseRecord.RemarkRate <= 5 && courseRecord.RemarkRate >= 1 && courseRecord.RemarkContent != "未评价")
                {
                    courseRecord.CourseOperation = db.CourseOperations.First(t => t.Creator == db.Users.First(u => u.UserName == System.Web.HttpContext.Current.User.Identity.Name));
                    courseRecord.Time = DateTime.Now;
                    db.Entry(courseRecord).State = EntityState.Modified;
                    db.SaveChanges();
                    if (courseRecord.RemarkContent != "未评价" && courseRecord.RemarkRate != 0)
                        return true;
                }
                return false;
            }
        }
        public static bool Apply(Guid Id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                var CourseOperation = db.CourseOperations.Find(Id);
                if (CourseOperation.Count < CourseOperation.Limit )// && DateTime.Now < CourseOperation.StartTime)
                {
                    var courseRecord = new CourseRecord
                    {
                        Id = Guid.NewGuid(),
                        CourseOperation = CourseOperation,
                        ActionTime = DateTime.Now,
                        Receiver = Extensions.GetCurrentUser(),
                        RemarkContent = "未评价",
                        RemarkRate = 0,
                        Time = new DateTime(1000, 1, 1, 0, 0, 0)
                    };
                    CourseOperation.Count++;
                    CourseOperation.Students.Add(Extensions.GetCurrentUser());
                    db.Entry(CourseOperation).State = EntityState.Modified;
                    db.CourseRecords.Add(courseRecord);
                    db.SaveChanges();
                    return true;
                }
                return false;      //人数已满显示错误信息。
            }
        }
        public static bool Quit(Guid Id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                var CourseOperation = db.CourseOperations.Find(Id);
                if (DateTime.Now < CourseOperation.StartTime)
                {
                    var courseRecord = db.CourseRecords.First(u => u.Receiver == Extensions.GetCurrentUser() && u.CourseOperation == CourseOperation);
                    CourseOperation.Count--;
                    CourseOperation.Students.Remove(Extensions.GetCurrentUser());
                    db.Entry(CourseOperation).State = EntityState.Modified;
                    db.CourseRecords.Remove(courseRecord);
                    db.SaveChanges();
                    return true;
                }
                return false;      //超时无法退选
            }
        }
    }
}
