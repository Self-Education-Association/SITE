﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Web.Models;
using System.IO;
using System.Net;

namespace Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private BaseDbContext db = new BaseDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.Error ? "出现错误。"
                : message == ManageMessageId.AddEducationSuccess ? "已添加你的一条教育经历。"
                : message == ManageMessageId.AddWorkSuccess ? "已添加一条你的工作经历。"
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
            };
            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        public ActionResult RecordList()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                User user = db.Users.Find(User.Identity.GetUserId());
                var educations = user.Education;
                var works = user.Work;
                educations.OrderBy(e => e.DegreeType);
                ViewBag.Educations = educations;
                ViewBag.Works = works;

                return View(educations);
            }
        }

        public ActionResult AddEducationRecord()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEducationRecord(EducationRecord educationRecord)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                if (ModelState.IsValid)
                {
                    educationRecord.Id = Guid.NewGuid();
                    if (db.Users.Find(User.Identity.GetUserId()).Education == null)
                        db.Users.Find(User.Identity.GetUserId()).Education = new System.Collections.Generic.List<EducationRecord>();
                    db.Users.Find(User.Identity.GetUserId()).Education.Add(educationRecord);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Message = ManageMessageId.AddEducationSuccess });
                }
            }

            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult AddWorkRecord()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddWorkRecord(WorkRecord workRecord)
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                if (ModelState.IsValid)
                {
                    workRecord.Id = Guid.NewGuid();
                    if (db.Users.Find(User.Identity.GetUserId()).Work == null)
                        db.Users.Find(User.Identity.GetUserId()).Work = new System.Collections.Generic.List<WorkRecord>();
                    db.Users.Find(User.Identity.GetUserId()).Work.Add(workRecord);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { Message = ManageMessageId.AddWorkSuccess });
                }
            }

            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult UserProfile()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            UserProfileViewModel model = (UserProfileViewModel)user.Profile;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (BaseDbContext db = new BaseDbContext())
                {
                    db.Users.Find(User.Identity.GetUserId()).Profile = model;
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.UpdateUserProfileSuccess });
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult UserIdentity()
        {
            ViewBag.Status = db.Users.Find(User.Identity.GetUserId()).IdentityRecord.Status;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserIdentity(UserIdentityViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count != 2)
                    return View();
                var allowExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                foreach (HttpPostedFileBase f in Request.Files)
                {
                    if (allowExtensions.Contains(Path.GetExtension(f.FileName)))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //根据日期生成服务器端文件名
                string uploadFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(Request.Files[0].FileName);
                //生成服务器端绝对路径
                string absolutFileName = Server.MapPath("~/") + "UserUpload/Identity/" + uploadFileName;
                //执行上传
                Request.Files[0].SaveAs(absolutFileName);
                model.FrontIdCard = new Material(uploadFileName, "", MaterialType.Identity);
                //根据日期生成服务器端文件名
                uploadFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(Request.Files[1].FileName);
                //生成服务器端绝对路径
                absolutFileName = Server.MapPath("~/") + "UserUpload/Identity/" + uploadFileName;
                //执行上传
                Request.Files[1].SaveAs(absolutFileName);
                model.BackIdCard = new Material(uploadFileName, "", MaterialType.Identity);
                return RedirectToAction("Index", new { Message = ManageMessageId.UserIdentitySuccess });
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult Project()
        {

            return View(new ProjectViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Project(ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.Projects.Any(p => p.Administrator == Extensions.GetCurrentUser()))
                    db.Projects.First(p => p.Administrator == Extensions.GetCurrentUser()).Information = model;
                else
                    db.Projects.Add(new Project(Extensions.GetCurrentUser(), model));
                db.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.ProjectSuccess });
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult TeamApply(int page = 0)
        {
            if (Extensions.GetCurrentUser().TeamRecord != null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            int pageSize = 10;
            var list = new ListPage<Team>(db.Teams.Where(u => u.Searchable == true), page, pageSize);
            return View(list);
        }

        public ActionResult TeamApply(Guid teamId)
        {
            if (Extensions.GetCurrentUser().TeamRecord != null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            Team team = db.Teams.Find(teamId);
            if (team == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            db.TeamRecords.Add(new TeamRecord(team, TeamMemberStatus.Applied));
            db.SaveChanges();
            return RedirectToAction("Index", new { Message = ManageMessageId.ApplySuccess });
        }

        public ActionResult TeamRecruit(int page = 0)
        {
            if (Extensions.GetCurrentUser().TeamRecord.Status != TeamMemberStatus.Admin)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            int pageSize = 10;
            var list = new ListPage<User>(db.Users.Where(u => u.Profile.Searchable == true), page, pageSize);
            return View(list);
        }

        public ActionResult TeamRecruit(string userId)
        {
            if (Extensions.GetCurrentUser().TeamRecord.Status != TeamMemberStatus.Admin)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            User user = db.Users.Find(userId);
            Team team = db.Teams.First(u => u.Administrator.Id == Extensions.GetCurrentUser().Id);
            if (user==null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            db.TeamRecords.Add(new TeamRecord(team, TeamMemberStatus.Recruited, user));
            db.SaveChanges();
            return RedirectToAction("Index", new { Message = ManageMessageId.RecruitSuccess });
        }

        public ActionResult TeamMember(int page=0)
        {
            Team team = Extensions.GetCurrentUser().TeamRecord.Team;
            if (team == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            int pageSize = 10;
            var list = new ListPage<TeamRecord>(db.TeamRecords.Where(u => u.Team == team), page, pageSize);
            return View(list);
        }

        public ActionResult TeamMemberDelete(string userId)
        {
            if (Extensions.GetCurrentUser().TeamRecord.Status != TeamMemberStatus.Admin)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            db.Entry(db.Users.Find(userId).TeamRecord).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index", new { Message = ManageMessageId.OperationSuccess });
        }

        public ActionResult TeamProfile()
        {
            User user = Extensions.GetCurrentUser();
            if (user.TeamRecord.Status != TeamMemberStatus.Admin)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            Team team = user.TeamRecord.Team;
            TeamProfileViewModel model = new TeamProfileViewModel
            {
                Id = team.Id,
                Name = team.Name,
                Administrator = team.Administrator.DisplayName,
                Time = team.Time,
                Introduction = team.Introduction,
                Announcement = team.Announcement
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeamProfile([Bind(Include ="Annoucement,Introduction,Searchable")]TeamProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = Extensions.GetCurrentUser();
                if (user.TeamRecord.Status != TeamMemberStatus.Admin)
                    return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
                Team team = db.Teams.First(t => t.Id == user.TeamRecord.Team.Id);
                team.Announcement = model.Announcement;
                team.Introduction = model.Introduction;
                team.Searchable = model.Searchable;
                db.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.ProjectSuccess });
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult Company()
        {
            User user = Extensions.GetCurrentUser();
            if (user.TeamRecord.Status != TeamMemberStatus.Admin)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (user.TeamRecord.Team.Company != null)
            {
                ViewBag.Status = user.TeamRecord.Team.Company.Status;
                return View((CompanyViewModel)user.TeamRecord.Team.Company.Information);
            }

            ViewBag.Status = CompanyStatus.None;
            return View(new CompanyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Company(CompanyViewModel model)
        {
            User user = Extensions.GetCurrentUser();
            if (user.TeamRecord.Status != TeamMemberStatus.Admin)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (ModelState.IsValid)
            {
                db.Teams.Find(user.TeamRecord.Team.Id).Company.Information = model;
                db.SaveChanges();
                return View();
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region 帮助程序

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasIdentitied()
        {
            using (BaseDbContext db = new BaseDbContext())
            {
                return db.Users.Find(User.Identity.GetUserId()).Identitied;
            }
        }

        public enum ManageMessageId
        {
            AddEducationSuccess,
            AddWorkSuccess,
            UpdateUserProfileSuccess,
            ChangePasswordSuccess,
            UserIdentitySuccess,
            ProjectSuccess,
            ApplySuccess,
            RecruitSuccess,
            OperationSuccess,
            AcessDenied,
            Error
        }

        #endregion
    }
}