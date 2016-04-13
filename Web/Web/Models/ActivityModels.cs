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
    public class ActivityOperation : Operation
    {
        [Required]
        [Display(Name = "报名人数")]
        public int Count { get; set; }

        [Required]
        [Display(Name = "上限人数")]
        public int Limit { get; set; }

        public virtual ICollection<ActivityRecord> Records { get; set; }

        public void NewActivity(BaseDbContext db)
        {
            Id = Guid.NewGuid();
            Creator = db.Users.Find(Extensions.GetUserId());
            Time = DateTime.Now;
            Enabled = true;
            Count = 0;
        }
    }

    public class ActivityRecord : Remark
    {
        public virtual ActivityOperation ActivityOperation { get; set; }

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
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        public bool Apply(Guid? id)
        {
            if (id == null)
                return false;

            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    var activityOperation = db.ActivityOperations.Find(id);
                    if (activityOperation == null)
                        return false;

                    id = Guid.NewGuid();
                    ActivityOperation = activityOperation;
                    ActionTime = DateTime.Now;
                    Receiver = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    RemarkContent = "";
                    RemarkRate = RemarkType.None;
                    Time = new DateTime(2000, 1, 1, 0, 0, 0);
                    ActivityOperation.Count++;
                    db.ActivityRecords.Add(this);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
        public bool Quit(Guid? id)
        {
            if (id == null)
                return false;

            using (BaseDbContext db = new BaseDbContext())
            {
                try
                {
                    if (ActivityOperation.Id != id)
                        return false;
                    var activityOperation = db.ActivityOperations.Find(id);
                    var contextRecord = db.ActivityRecords.Find(Id);
                    var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
                    activityOperation.Count--;
                    if (activityOperation.Records != null)
                    {
                        activityOperation.Records.Remove(contextRecord);
                    }
                    db.ActivityRecords.Remove(contextRecord);
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
