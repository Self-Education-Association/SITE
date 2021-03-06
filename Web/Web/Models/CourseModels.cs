﻿using System;
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

        [Display(Name = "启用")]
        public bool Status { get; set; }

        public virtual List<User> Students { get; set; }

        [Display(Name = "位置")]
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
                    var course = db.CourseOperations.Find(Id);
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
                        if (db.CourseOperations.Find(Id) == this)
                            return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool Delete(ref BaseDbContext db)
        {
            try
            {
            Enabled = false;
            if (!db.CourseOperations.Local.Contains(this))
            {
                db.CourseOperations.Attach(this);
            }
            var listRecord = (db.CourseRecords.Where(c => c.CourseOperation.Id == Id));
            foreach (CourseRecord courseRecord in listRecord.ToList())
            {
                db.Messages.Add(new Message(courseRecord.Receiver, courseRecord.CourseOperation.Name, MessageType.System, MessageTemplate.CourseDelete, ref db));
                db.CourseRecords.Remove(courseRecord);
            }
            db.CourseOperations.Remove(this);
            db.SaveChanges();
            return true;
            }
            catch
            {
                return false;
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

        public static List<CourseOperation> List(string select, bool IsTeacher,User user)
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
                        user = db.Users.Find(user.Id);
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
                    return null;
                }
            }
        }
    }

    public class CourseRecord : Remark
    {
        public virtual CourseOperation CourseOperation { get; set; }
        public bool Remark()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    Time = DateTime.Now;
                    db.Entry(this).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool Apply(Guid Id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    CourseOperation = db.CourseOperations.Find(Id);
                    Id = Guid.NewGuid();
                    ActionTime = DateTime.Now;
                    Receiver = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    RemarkContent = "";
                    RemarkRate = RemarkType.None;
                    Time = new DateTime(2000, 1, 1, 0, 0, 0);
                    CourseOperation.Count++;
                    CourseOperation.Students.Add(Receiver);
                    db.CourseRecords.Add(this);
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
