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
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        public bool Edit()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                //try
                //{
                    Time = DateTime.Now;
                    Usable = true;
                    db.Entry(this).State = EntityState.Modified;
                    db.SaveChanges();
                    if (db.RoomOperations.Find(Id) != null)
                        return true;
                    return false;
                //}
                //catch (Exception e)
                //{
                //    return false;
                //}
            }
        }
        public bool Delete(ref BaseDbContext db)
        {
            try
            {
                Enabled = false;
                if (!db.RoomOperations.Local.Contains(this))
                {
                    db.RoomOperations.Attach(this);
                }
                var listRecord = (db.RoomRecords.Where(r => r.RoomOperation.Id == Id));
                foreach (RoomRecord roomRecord in listRecord.ToList())
                {
                    db.Messages.Add(new Message(roomRecord.Receiver, roomRecord.RoomOperation.Name, MessageType.System, MessageTemplate.RoomDelete, ref db));
                    db.RoomRecords.Remove(roomRecord);
                }
                db.RoomOperations.Remove(this);
                db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static List<RoomOperation> List(string select, bool IsTeacher)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
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
                catch (Exception e)
                {
                    return db.RoomOperations.ToList();
                }
            }
        }
    }
    public class RoomRecord : Remark
    {
        public RoomOperation RoomOperation { get; set; }

        public bool Remark()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    User user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    RoomOperation = db.RoomOperations.First(t => t.Creator == user);
                    Time = DateTime.Now;
                    db.Entry(this).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
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
                catch (Exception e)
                {
                    return false;
                }
            }
        }
    }
}
