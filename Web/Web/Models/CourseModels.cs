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
using System.Web;
using Microsoft.AspNet.Identity;

namespace Web.Models
{
    public class CourseOperation : Operation
    {
        [Display(Name = "报名人数")]
        public int Count { get; set; }

        [Display(Name = "上限人数")]
        public int Limit { get; set; }

        [Display(Name = "状态")]
        public string Status { get; set; }

        public virtual List<User> Students { get; set; }

        [Display(Name = "上限人数")]
        public string Location { get; set; }

        public bool Create()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    Id = Guid.NewGuid();
                    Time = DateTime.Now;
                    Count = 0;
                    Enabled = true;
                    Creator = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    db.CourseOperations.Add(this);
                    db.SaveChanges();
                    var course = db.CourseOperations.Find(this.Id);
                    if (course != null)
                        return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool Edit()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    Time = DateTime.Now;
                    db.Entry(this).State = EntityState.Modified;
                    db.SaveChanges();
                    if (db.CourseOperations.Find(Id) != null)
                        if(db.CourseOperations.Find(Id) == this)
                        return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool Delete(Guid id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    CourseOperation courseOperation = db.CourseOperations.Find(id);
                    courseOperation.Enabled = false;
                    db.Entry(courseOperation).State = EntityState.Modified;
                    db.SaveChanges();
                    if (courseOperation.Enabled == false)
                        return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static List<CourseOperation> List(string select, bool IsTeacher)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    int pageSize = 5;
                    int page = 0;
                    IQueryable<CourseOperation> Course = db.CourseOperations.Where(a => a.Enabled != false);
                    if (IsTeacher)
                    {
                        var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                        if (select == null | select == "")
                        {
                            Course = (from a in db.CourseOperations where a.Creator.Id == user.Id orderby a.Name select a).AsQueryable();
                        }
                        else
                        {
                            Course = (from a in db.CourseOperations
                                      where a.Creator.Id == user.Id && a.Name == @select
                                      orderby a.Name
                                      select a).AsQueryable();
                        }
                    }
                    else
                    {
                        if (select == null)
                        {
                            Course = (from a in db.CourseOperations
                                      where a.StartTime > DateTime.Now
                                      orderby a.Time
                                      select a).AsQueryable();
                        }
                        else
                        {
                            Course = (from a in db.CourseOperations
                                      where a.Name == @select && a.StartTime > DateTime.Now
                                      orderby a.Time
                                      select a).AsQueryable();
                        }
                    }
                    var paginatedNews = new ListPage<CourseOperation>(Course, page, pageSize);
                    return paginatedNews;
                }
                catch
                {
                    return db.CourseOperations.ToList();
                }
            }
        }
    }

    public class CourseRecord : Remark
    {
        public CourseOperation CourseOperation { get; set; }
        public static bool Remark([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] CourseRecord courseRecord)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    User user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    courseRecord.CourseOperation = db.CourseOperations.First(t => t.Creator == user);
                    courseRecord.Time = DateTime.Now;
                    db.Entry(courseRecord).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool Apply(Guid Id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    var CourseOperation = db.CourseOperations.Find(Id);
                    var courseRecord = new CourseRecord
                    {
                        Id = Guid.NewGuid(),
                        CourseOperation = CourseOperation,
                        ActionTime = DateTime.Now,
                        Receiver = db.Users.Find(HttpContext.Current.User.Identity.GetUserId()),
                        RemarkContent = "",
                        RemarkRate = RemarkType.None,
                        Time = new DateTime(2000, 1, 1, 0, 0, 0)
                    };
                    CourseOperation.Count++;
                    db.CourseRecords.Add(courseRecord);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool Quit(Guid Id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    var CourseOperation = db.CourseOperations.Find(Id);
                    var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    var courseRecord = (from a in db.CourseRecords where a.Receiver.Id == user.Id && a.CourseOperation.Id == CourseOperation.Id select a).First();
                    CourseOperation.Count--;
                    if (CourseOperation.Students != null)
                    {
                        if (CourseOperation.Students.Contains(user))
                            CourseOperation.Students.Remove(user);
                    }
                    db.CourseRecords.Remove(courseRecord);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
