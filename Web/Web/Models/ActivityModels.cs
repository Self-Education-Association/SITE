using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ActivityOperation:Operation
    {
        [Required]
        public int Count { get; set; }

        [Required]
        public int Limit { get; set; }

        public virtual ICollection<ActivityRecord> Records { get; set; }

        public static bool Create(ActivityOperation ActivityOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    ActivityOperation.Id = Guid.NewGuid();
                    ActivityOperation.Time = DateTime.Now;
                    ActivityOperation.Count = 0;
                    ActivityOperation.Enabled = 1;
                    ActivityOperation.Creator = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    db.ActivityOperations.Add(ActivityOperation);
                    db.SaveChanges();
                    var Activity = db.ActivityOperations.Find(ActivityOperation.Id);
                    if (Activity != null)
                        return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool Update([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,State")] ActivityOperation ActivityOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    if (ActivityOperation.Records != null)
                    {
                        if (ActivityOperation.Records.Count > ActivityOperation.Limit)
                            return false;
                    }
                    ActivityOperation.Time = DateTime.Now;
                    db.Entry(ActivityOperation).State = EntityState.Modified;
                    db.SaveChanges();
                    if (db.ActivityOperations.Find(ActivityOperation.Id) != null)
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
                    ActivityOperation ActivityOperation = db.ActivityOperations.Find(id);
                    ActivityOperation.Enabled = 0;
                    db.Entry(ActivityOperation).State = EntityState.Modified;
                    db.SaveChanges();
                    if (ActivityOperation.Enabled == 0)
                        return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static List<ActivityOperation> List(string select, bool IsAdministrator)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    int pageSize = 5;
                    int page = 0;
                    IQueryable<ActivityOperation> Activity = db.ActivityOperations.Where(a => a.Enabled != 0);
                    if (IsAdministrator)
                    {
                        var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                        if (select == null | select == "")
                        {
                            Activity = (from a in db.ActivityOperations where a.Creator.Id == user.Id orderby a.Name select a).AsQueryable();
                        }
                        else
                        {
                            Activity = (from a in db.ActivityOperations
                                      where a.Creator.Id == user.Id && a.Name == @select
                                      orderby a.Name
                                      select a).AsQueryable();
                        }
                    }
                    else
                    {
                        if (select == null)
                        {
                            Activity = (from a in db.ActivityOperations
                                      where a.StartTime > DateTime.Now
                                      orderby a.Time
                                      select a).AsQueryable();
                        }
                        else
                        {
                            Activity = (from a in db.ActivityOperations
                                      where a.Name == @select && a.StartTime > DateTime.Now
                                      orderby a.Time
                                      select a).AsQueryable();
                        }
                    }
                    var paginatedNews = new ListPage<ActivityOperation>(Activity, page, pageSize);
                    return paginatedNews;
                }
                catch
                {
                    return db.ActivityOperations.ToList();
                }
            }
        }

        public void NewActivity(BaseDbContext db)
        {
            Id = Guid.NewGuid();
            Creator = db.Users.Find(Extensions.GetUserId());
            Time = DateTime.Now;
            Enabled = 1;
            Count = 0;
        }
    }


    public class ActivityRecord:Remark
    {
        public virtual ActivityOperation ActivityOperation { get; set; }
        public static bool Remark([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] ActivityRecord ActivityRecord)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    if (ActivityRecord.RemarkRate > 0 && ActivityRecord.RemarkRate <= 5)
                    {
                        User user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                        ActivityRecord.ActivityOperation = db.ActivityOperations.First(t => t.Creator == user);
                        ActivityRecord.Time = DateTime.Now;
                        db.Entry(ActivityRecord).State = EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                    return false;
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
                    var ActivityOperation = db.ActivityOperations.Find(Id);
                    var ActivityRecord = new ActivityRecord
                    {
                        Id = Guid.NewGuid(),
                        ActivityOperation = ActivityOperation,
                        ActionTime = DateTime.Now,
                        Receiver = db.Users.Find(HttpContext.Current.User.Identity.GetUserId()),
                        RemarkContent = "未评价",
                        RemarkRate = 0,
                        Time = new DateTime(2000, 1, 1, 0, 0, 0)
                    };
                    ActivityOperation.Count++;
                    db.ActivityRecords.Add(ActivityRecord);
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
                    var ActivityOperation = db.ActivityOperations.Find(Id);
                    var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    var ActivityRecord = (from a in db.ActivityRecords where a.Receiver.Id == user.Id && a.ActivityOperation.Id == ActivityOperation.Id select a).First();
                    ActivityOperation.Count--;
                    if (ActivityOperation.Records != null)
                    {
                        if (ActivityOperation.Records.Contains(ActivityRecord))
                            ActivityOperation.Records.Remove(ActivityRecord);
                    }
                    db.ActivityRecords.Remove(ActivityRecord);
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
