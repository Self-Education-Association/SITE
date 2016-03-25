using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class TeamEventsController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        // GET: TeamEvents
        public ActionResult Index()
        {
            if (HasTeam())
            {
                var model = Extensions.GetContextUser(ref db).TeamRecord.Team.Events.OrderByDescending(t => t.EventTime);
                ViewBag.IsAdmin = IsTeamAdmin();
                return View(model);
            }

            return View(db.TeamEvents.ToList());
        }

        // GET: TeamEvents/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamEvent teamEvent = db.TeamEvents.Find(id);
            if (teamEvent == null)
            {
                return HttpNotFound();
            }
            return View(teamEvent);
        }

        // GET: TeamEvents/Create
        public ActionResult Create()
        {
            if (IsTeamAdmin())
                return View();
            return new HttpStatusCodeResult(403);
        }

        // POST: TeamEvents/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,EventContent,EventTime,AddTime")] TeamEvent teamEvent)
        {
            if (IsTeamAdmin())
            {
                if (ModelState.IsValid)
                {
                    var user = Extensions.GetContextUser(ref db);
                    var team = user.TeamRecord.Team;
                    teamEvent.Id = Guid.NewGuid();
                    teamEvent.Team = team;
                    db.TeamEvents.Add(teamEvent);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(teamEvent);
            }

            return new HttpStatusCodeResult(403);
        }

        // GET: TeamEvents/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (IsTeamAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TeamEvent teamEvent = db.TeamEvents.Find(id);
                if (teamEvent == null)
                {
                    return HttpNotFound();
                }
                return View(teamEvent);
            }

            return new HttpStatusCodeResult(403);
        }

        // POST: TeamEvents/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventName,EventContent,EventTime")] TeamEvent teamEvent)
        {
            if (IsTeamAdmin())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(teamEvent).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(teamEvent);
            }

            return new HttpStatusCodeResult(403);
        }

        // GET: TeamEvents/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (IsTeamAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TeamEvent teamEvent = db.TeamEvents.Find(id);
                if (teamEvent == null)
                {
                    return HttpNotFound();
                }
                return View(teamEvent);
            }

            return new HttpStatusCodeResult(403);
        }

        // POST: TeamEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            if (IsTeamAdmin())
            {
                TeamEvent teamEvent = db.TeamEvents.Find(id);
                db.TeamEvents.Remove(teamEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return new HttpStatusCodeResult(403);
        }
        #region HelpFunction
        protected bool HasTeam()
        {
            var user = Extensions.GetContextUser(ref db);
            if (user == null)
                return false;
            return user.TeamRecord != null;
        }

        protected bool IsTeamAdmin()
        {
            var user = Extensions.GetContextUser(ref db);
            if (HasTeam() == false)
                return false;
            return user.TeamRecord.Status == TeamMemberStatus.Admin;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
