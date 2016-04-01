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
    [Authorize]
    public class ReportsController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        /*
        // GET: Reports
        public ActionResult Index()
        {
            if (!IsTeamAdmin())
            {
                return new HttpStatusCodeResult(403);
            }

            return View(db.TeamReports.ToList());
        }

        // GET: Reports/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeamReport teamReport = db.TeamReports.Find(id);
            if (teamReport == null)
            {
                return HttpNotFound();
            }
            return View(teamReport);
        }*/

        // GET: Reports/Create
        public ActionResult Create()
        {
            if (!IsTeamAdmin())
            {
                return new HttpStatusCodeResult(403);
            }

            return View();
        }

        // POST: Reports/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] TeamReport teamReport)
        {
            if (!IsTeamAdmin())
            {
                return new HttpStatusCodeResult(403);
            }
            if (ModelState.IsValid && Request.Files.Count == 1)
            {
                var file = Request.Files[0];
                var team = Extensions.GetContextUser(ref db).TeamRecord.Team;
                if (MaterialType.Management.Match(file))
                {
                    var report = Material.Create(team.Name + DateTime.Now, MaterialType.Management, file, db);
                    if (report == null)
                    {
                        TempData["Alert"] = "请检查上传文件！";
                        return View(teamReport);
                    }
                    teamReport.Id = Guid.NewGuid();
                    teamReport.ReportFile = report;
                    db.TeamReports.Add(teamReport);
                    db.SaveChanges();
                    TempData["Alert"] = "上传成功！";
                    return RedirectToAction("Index", "Manage");
                }
                else
                {
                    ViewBag.Alert = "请确认上传文件的文件格式！";
                    return View(teamReport);
                }
            }

            ViewBag.Alert = "请检查各选项，并确保上传了正确的文件。";
            return View(teamReport);
        }

        /*
// GET: Reports/Edit/5
public ActionResult Edit(Guid? id)
{
    if (!IsTeamAdmin())
    {
        return new HttpStatusCodeResult(403);
    }
    if (id == null)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
    TeamReport teamReport = db.TeamReports.Find(id);
    if (teamReport == null)
    {
        return HttpNotFound();
    }
    return View(teamReport);
}

// POST: Reports/Edit/5
// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit([Bind(Include = "Id")] TeamReport teamReport)
{
    if (!IsTeamAdmin())
    {
        return new HttpStatusCodeResult(403);
    }
    if (ModelState.IsValid && Request.Files.Count == 1)
    {
        var file = Request.Files[0];
        var team = Extensions.GetContextUser(ref db).TeamRecord.Team;
        var contextReport = db.TeamReports.Find(teamReport.Id);

        if (contextReport == null)
            return HttpNotFound();
        var originFile = db.Materials.Find(contextReport.ReportFile.Id);
        if (MaterialType.Management.Match(file))
        {
            var reportFile = Material.ChangeFile(originFile.Id, file, db);
            teamReport.Id = Guid.NewGuid();
            teamReport.ReportFile = reportFile;
            db.TeamReports.Add(teamReport);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.Alert = "请确认上传文件的文件格式！";
            return View(teamReport);
        }
    }

    ViewBag.Alert = "请检查各选项，并确保上传了正确的文件。";
    return View(teamReport);
}
// GET: Reports/Delete/5
public ActionResult Delete(Guid? id)
{
    if (id == null)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
    TeamReport teamReport = db.TeamReports.Find(id);
    if (teamReport == null)
    {
        return HttpNotFound();
    }
    return View(teamReport);
}

// POST: Reports/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public ActionResult DeleteConfirmed(Guid id)
{
    TeamReport teamReport = db.TeamReports.Find(id);
    db.TeamReports.Remove(teamReport);
    db.SaveChanges();
    return RedirectToAction("Index");
}
*/

        #region HelperFunction
        protected bool IsTeamAdmin()
        {
            var user = Extensions.GetContextUser(ref db);
            if (user == null || user.TeamRecord == null)
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
