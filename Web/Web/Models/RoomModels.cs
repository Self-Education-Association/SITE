using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Web.Models
{
    public class RoomOperation : Operation
    {
        public int Usable { get; set; }
        public virtual List<RoomRecord> RoomRecords { get; set; }
        public static bool Create(RoomOperation RoomOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    RoomOperation.Id = Guid.NewGuid();
                    RoomOperation.Time = DateTime.Now;
                    RoomOperation.Usable = 1;
                    RoomOperation.State = 1;
                    RoomOperation.Creator = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    db.RoomOperations.Add(RoomOperation);
                    db.SaveChanges();
                    var Room = db.RoomOperations.Find(RoomOperation.Id);
                    if (Room != null)
                        return true;
                    return false;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        public static bool Update([Bind(Include = "Id,Usable,Location,Name,StartTime,EndTime,Content,State")] RoomOperation RoomOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    RoomOperation.Time = DateTime.Now;
                    db.Entry(RoomOperation).State = EntityState.Modified;
                    db.SaveChanges();
                    if (db.RoomOperations.Find(RoomOperation.Id) != null)
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
                    RoomOperation RoomOperation = db.RoomOperations.Find(id);
                    RoomOperation.State = 0;
                    db.Entry(RoomOperation).State = EntityState.Modified;
                    db.SaveChanges();
                    if (RoomOperation.State == 0)
                        return true;
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static List<RoomOperation> List(string select, bool IsTeacher)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    string changed = "0";
                    //模拟触发器
                    foreach (RoomOperation roomOperation in db.RoomOperations)
                    {
                        if (DateTime.Now > roomOperation.EndTime)
                        {
                            roomOperation.StartTime.AddDays(7.0);
                            roomOperation.EndTime.AddDays(7.0);
                            roomOperation.Usable = 1;
                            changed = "yes,you have changed";
                        }
                    }
                    if (changed == "yes,you have changed")
                        db.SaveChanges();
                    int pageSize = 5;
                    int page = 0;
                    IQueryable<RoomOperation> Room = db.RoomOperations.Where(a => a.State != 0);
                    if (IsTeacher)
                    {
                        var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                        if (select == null | select == "")
                        {
                            Room = (from a in db.RoomOperations where a.Creator.Id == user.Id orderby a.Name select a).AsQueryable();
                        }
                        else
                        {
                            Room = (from a in db.RoomOperations
                                      where a.Creator.Id == user.Id && a.Name == @select
                                      orderby a.Name
                                      select a).AsQueryable();
                        }
                    }
                    else
                    {
                        if (select == null)
                        {
                            Room = (from a in db.RoomOperations
                                      where a.StartTime > DateTime.Now
                                      orderby a.Time
                                      select a).AsQueryable();
                        }
                        else
                        {
                            Room = (from a in db.RoomOperations
                                      where a.Name == @select && a.StartTime > DateTime.Now
                                      orderby a.Time
                                      select a).AsQueryable();
                        }
                    }
                    var paginatedNews = new ListPage<RoomOperation>(Room, page, pageSize);
                    return paginatedNews;
                }
                catch
                {
                    return db.RoomOperations.ToList();
                }
            }
        }
    }
    public class RoomRecord : Remark
    {
        public RoomOperation RoomOperation { get; set; }
        public static bool Remark([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] RoomRecord RoomRecord)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    if (RoomRecord.RemarkRate > 0 && RoomRecord.RemarkRate <= 5)
                    {
                        User user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                        RoomRecord.RoomOperation = db.RoomOperations.First(t => t.Creator == user);
                        RoomRecord.Time = DateTime.Now;
                        db.Entry(RoomRecord).State = EntityState.Modified;
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
                    var RoomOperation = db.RoomOperations.Find(Id);
                    var RoomRecord = new RoomRecord
                    {
                        Id = Guid.NewGuid(),
                        RoomOperation = RoomOperation,
                        ActionTime = DateTime.Now,
                        Receiver = db.Users.Find(HttpContext.Current.User.Identity.GetUserId()),
                        RemarkContent = "未评价",
                        RemarkRate = 0,
                        Time = new DateTime(2000, 1, 1, 0, 0, 0)
                    };
                    RoomOperation.Usable = 0;
                    db.RoomRecords.Add(RoomRecord);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
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
                    var RoomOperation = db.RoomOperations.Find(Id);
                    var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    var RoomRecord = (from a in db.RoomRecords where a.Receiver.Id == user.Id && a.RoomOperation.Id == RoomOperation.Id select a).First();
                    RoomOperation.Usable=1;
                    if (RoomOperation.RoomRecords != null)
                    {
                        if (RoomOperation.RoomRecords.Contains(RoomRecord))
                            RoomOperation.RoomRecords.Remove(RoomRecord);
                    }
                    db.RoomRecords.Remove(RoomRecord);
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
