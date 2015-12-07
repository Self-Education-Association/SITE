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
    public class CourseRecordsController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        // GET: CourseRecords
        public ActionResult Index()
        {
            return View(db.CourseRecords.ToList());
        }

        // GET: CourseRecords/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseRecord courseRecord = db.CourseRecords.Find(id);
            if (courseRecord == null)
            {
                return HttpNotFound();
            }
            return View(courseRecord);
        }

        // GET: CourseRecords/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CourseRecords/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] CourseRecord courseRecord)
        {
            if (ModelState.IsValid)
            {
                courseRecord.Id = Guid.NewGuid();
                db.CourseRecords.Add(courseRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseRecord);
        }

        // GET: CourseRecords/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseRecord courseRecord = db.CourseRecords.Find(id);
            if (courseRecord == null)
            {
                return HttpNotFound();
            }
            return View(courseRecord);
        }

        // POST: CourseRecords/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] CourseRecord courseRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courseRecord);
        }

        // GET: CourseRecords/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseRecord courseRecord = db.CourseRecords.Find(id);
            if (courseRecord == null)
            {
                return HttpNotFound();
            }
            return View(courseRecord);
        }

        // POST: CourseRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CourseRecord courseRecord = db.CourseRecords.Find(id);
            db.CourseRecords.Remove(courseRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
