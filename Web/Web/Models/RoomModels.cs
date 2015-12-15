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
    public class RoomOperation : Operation
    {
        [Display(Name = "可使用")]
        public bool Usable { get; set; }

        public virtual List<RoomRecord> RoomRecords { get; set; }

        public bool Create()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    Id = Guid.NewGuid();
                    Time = DateTime.Now;
                    Usable = true;
                    Enabled = true;
                    Creator = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    db.RoomOperations.Add(this);
                    db.SaveChanges();
                    var Room = db.RoomOperations.Find(Id);
                    if (Room != null)
                        return true;
                    return false;
                }
                catch
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
                    RoomOperation.Enabled = false;
                    db.Entry(RoomOperation).State = EntityState.Modified;
                    db.SaveChanges();
                    if (RoomOperation.Enabled == false)
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
                    //模拟触发器
                    foreach (RoomOperation roomOperation in db.RoomOperations)
                    {
                        if (DateTime.Now > roomOperation.EndTime)
                        {
                            roomOperation.StartTime.AddDays(7.0);
                            roomOperation.EndTime.AddDays(7.0);
                            roomOperation.Usable = true;
                        }
                    }
                    db.SaveChanges();
                    int pageSize = 5;
                    int page = 0;
                    IQueryable<RoomOperation> Room = db.RoomOperations.Where(a => a.Enabled != false);
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
                    User user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    RoomRecord.RoomOperation = db.RoomOperations.First(t => t.Creator == user);
                    RoomRecord.Time = DateTime.Now;
                    db.Entry(RoomRecord).State = EntityState.Modified;
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
                    var roomOperation = db.RoomOperations.Find(Id);
                    if (roomOperation == null)
                        return false;
                    Id = Guid.NewGuid();
                    RoomOperation = roomOperation;
                    ActionTime = DateTime.Now;
                    Receiver = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    RemarkContent = "";
                    RemarkRate = RemarkType.None;
                    Time = new DateTime(2000, 1, 1, 0, 0, 0);
                    RoomOperation.Usable = false;
                    db.RoomRecords.Add(this);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool Quit(Guid Id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    var roomOperation = db.RoomOperations.Find(Id);
                    if (RoomOperation != roomOperation)
                        return false;
                    var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    RoomOperation.Usable = true;
                    if (RoomOperation.RoomRecords != null)
                    {
                        if (RoomOperation.RoomRecords.Contains(this))
                            RoomOperation.RoomRecords.Remove(this);
                    }
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
