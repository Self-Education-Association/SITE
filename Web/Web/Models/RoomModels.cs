using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

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
                RoomOperation.Id = Guid.NewGuid();
                RoomOperation.Time = DateTime.Now;
                RoomOperation.State = 1;
                RoomOperation.Time = DateTime.Now;
                RoomOperation.Usable = 1;
                RoomOperation.Creator = Extensions.GetCurrentUser();
                db.RoomOperations.Add(RoomOperation);
                db.SaveChanges();
                if (db.RoomOperations.Find(RoomOperation.Id) != null)
                    return true;
                return false;
            }
        }
        public static bool Update(RoomOperation RoomOperation)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                db.Entry(RoomOperation).State = EntityState.Modified;
                db.SaveChanges();
                if (db.RoomOperations.Find(RoomOperation.Id) != null)
                    return true;
                return false;
            }
        }
        public static bool Delete(Guid id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                RoomOperation RoomOperation = db.RoomOperations.Find(id);
                RoomOperation.State = 0;
                db.Entry(RoomOperation).State = EntityState.Modified;
                db.SaveChanges();
                if (RoomOperation.State == 0)
                    return true;
                return false;
            }
        }
        public static object List(string select, bool IsAdministrator)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                string changed="0";
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
                IQueryable<RoomOperation> Room;
                if (IsAdministrator)
                {
                    var user = Extensions.GetCurrentUser();
                    if (select == null | select == "")
                    { Room = db.RoomOperations; }
                    else
                    {
                        Room = (from a in db.RoomOperations
                                where a.Name == @select
                                orderby a.Name
                                select a).AsQueryable();
                    }
                }
                else
                {
                    if (select == null)
                    {
                        Room = (from a in db.RoomOperations
                                where a.State != 0 && a.StartTime > DateTime.Now
                                orderby a.Time
                                select a).AsQueryable();
                    }
                    else
                    {
                        Room = (from a in db.RoomOperations
                                where a.State != 0 && a.Name == @select && a.StartTime > DateTime.Now
                                orderby a.Time
                                select a).AsQueryable();
                    }
                }
                var paginatedNews = new ListPage<RoomOperation>(Room, page, pageSize);
                return paginatedNews;
            }
        }
    }

    public class RoomRecord : Remark
    {
        public RoomOperation RoomOperation { get; set; }
        public static bool Remark(RoomRecord RoomRecord)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                if (RoomRecord.RemarkRate <= 5 && RoomRecord.RemarkRate >= 1 && RoomRecord.RemarkContent != "未评价")
                {
                    RoomRecord.RoomOperation = db.RoomOperations.First(t => t.Creator == db.Users.First(u => u.UserName == System.Web.HttpContext.Current.User.Identity.Name));
                    RoomRecord.Time = DateTime.Now;
                    db.Entry(RoomRecord).State = EntityState.Modified;
                    db.SaveChanges();
                    if (RoomRecord.RemarkContent != "未评价" && RoomRecord.RemarkRate != 0)
                        return true;
                }
                return false;
            }
        }
        public static bool Apply(Guid Id)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                var RoomOperation = db.RoomOperations.Find(Id);
                if (DateTime.Now < RoomOperation.StartTime && RoomOperation.Usable == 1)
                {
                    var RoomRecord = new RoomRecord
                    {
                        Id = Guid.NewGuid(),
                        RoomOperation = RoomOperation,
                        ActionTime = DateTime.Now,
                        Receiver = Extensions.GetCurrentUser(),
                        RemarkContent = "未评价",
                        RemarkRate = 0,
                        Time = new DateTime(1000, 1, 1, 0, 0, 0)
                    };
                    RoomOperation.Usable = 0;
                    RoomOperation.RoomRecords.Add(RoomRecord);
                    db.Entry(RoomOperation).State = EntityState.Modified;
                    db.RoomRecords.Add(RoomRecord);
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
                var RoomOperation = db.RoomOperations.Find(Id);
                if (DateTime.Now < RoomOperation.StartTime)
                {
                    var RoomRecord = db.RoomRecords.First(u => u.Receiver == Extensions.GetCurrentUser() && u.RoomOperation == RoomOperation);
                    RoomOperation.Usable = 1;
                    RoomOperation.RoomRecords.Remove(RoomRecord);
                    db.Entry(RoomOperation).State = EntityState.Modified;
                    db.RoomRecords.Remove(RoomRecord);
                    db.SaveChanges();
                    return true;
                }
                return false;      //超时无法退选
            }
        }
    }
}