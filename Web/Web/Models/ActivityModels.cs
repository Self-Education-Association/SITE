using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ActivityOperation:Operation
    {
        public int Count { get; set; }

        public int Limit { get; set; }

        public virtual ICollection<ActivityRecord> Records { get; set; }
        public static bool Create(ActivityOperation ActivityOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                ActivityOperation.Id = Guid.NewGuid();
                ActivityOperation.Time = DateTime.Now;
                ActivityOperation.Count = 0;
                ActivityOperation.State = 1;
                ActivityOperation.Time = DateTime.Now;
                ActivityOperation.Creator = Extensions.GetCurrentUser();
                db.ActivityOperations.Add(ActivityOperation);
                db.SaveChanges();
                if (db.ActivityOperations.Find(ActivityOperation.Id) != null)
                    return true;
                return false;
            }
        }
        public static bool Update(ActivityOperation ActivityOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                db.Entry(ActivityOperation).State = EntityState.Modified;
                db.SaveChanges();
                if (db.ActivityOperations.Find(ActivityOperation.Id) != null)
                    return true;
                return false;
            }
        }
        public static bool Delete(Guid id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                ActivityOperation ActivityOperation = db.ActivityOperations.Find(id);
                ActivityOperation.State = 0;
                db.Entry(ActivityOperation).State = EntityState.Modified;
                db.SaveChanges();
                if (ActivityOperation.State == 0)
                    return true;
                return false;
            }
        }
        public static object List(string select, bool IsAdministrator)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                int pageSize = 5;
                int page = 0;
                IQueryable<ActivityOperation> Activity;
                if (IsAdministrator)
                {
                    var user = Extensions.GetCurrentUser();
                    if (select == null | select=="")
                    { Activity = db.ActivityOperations.AsQueryable(); }
                    else
                    {
                        Activity = (from a in db.ActivityOperations
                                  where a.Name == @select
                                  orderby a.Name
                                  select a).AsQueryable();
                    }
                }
                else
                {
                    if (select == null)
                    {
                        Activity = (from a in db.ActivityOperations
                                  where a.State != 0 && a.StartTime > DateTime.Now
                                  orderby a.Time
                                  select a).AsQueryable();
                    }
                    else
                    {
                        Activity = (from a in db.ActivityOperations
                                  where a.State != 0 && a.Name == @select && a.StartTime > DateTime.Now
                                  orderby a.Time
                                  select a).AsQueryable();
                    }
                }
                var paginatedNews = new ListPage<ActivityOperation>(Activity, page, pageSize);
                return paginatedNews;
            }
        }
    }


    public class ActivityRecord:Remark
    {
        public virtual ActivityOperation ActivityOperation { get; set; }
        public static bool Remark(ActivityRecord ActivityRecord)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                if (ActivityRecord.RemarkRate <= 5 && ActivityRecord.RemarkRate >= 1 && ActivityRecord.RemarkContent != "未评价")
                {
                    ActivityRecord.ActivityOperation = db.ActivityOperations.First(t => t.Creator == db.Users.First(u => u.UserName == HttpContext.Current.User.Identity.Name));
                    ActivityRecord.Time = DateTime.Now;
                    db.Entry(ActivityRecord).State = EntityState.Modified;
                    db.SaveChanges();
                    if (ActivityRecord.RemarkContent != "未评价" && ActivityRecord.RemarkRate != 0)
                        return true;
                }
                return false;
            }
        }
        public static bool Apply(Guid Id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                var ActivityOperation = db.ActivityOperations.Find(Id);
                if (ActivityOperation.Count < ActivityOperation.Limit && DateTime.Now < ActivityOperation.StartTime)
                {
                    var ActivityRecord = new ActivityRecord
                    {
                        Id = Guid.NewGuid(),
                        ActivityOperation = ActivityOperation,
                        ActionTime = DateTime.Now,
                        Receiver = Extensions.GetCurrentUser(),
                        RemarkContent = "未评价",
                        RemarkRate = 0,
                        Time = new DateTime(1000, 1, 1, 0, 0, 0)
                    };
                    ActivityOperation.Count++;
                    db.Entry(ActivityOperation).State = EntityState.Modified;
                    db.ActivityRecords.Add(ActivityRecord);
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
                var ActivityOperation = db.ActivityOperations.Find(Id);
                if (DateTime.Now < ActivityOperation.StartTime)
                {
                    var ActivityRecord = db.ActivityRecords.First(u => u.Receiver == Extensions.GetCurrentUser() && u.ActivityOperation == ActivityOperation);
                    ActivityOperation.Count--;
                    ActivityOperation.Records.Remove(ActivityRecord);
                    db.Entry(ActivityOperation).State = EntityState.Modified;
                    db.ActivityRecords.Remove(ActivityRecord);
                    db.SaveChanges();
                    return true;
                }
                return false;      //超时无法退选
            }
        }
    }
}