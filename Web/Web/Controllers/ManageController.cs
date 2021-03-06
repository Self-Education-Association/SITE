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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Collections.Generic;

namespace Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private BaseDbContext db = new BaseDbContext();

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

        public ActionResult Index(ManageMessageId? message)
        {
            if (message != null)
            {
                TempData["Alert"] =
                message == ManageMessageId.Error ? "出现错误。"
                : message == ManageMessageId.AddEducationSuccess ? "已添加你的一条教育经历。"
                : message == ManageMessageId.AddWorkSuccess ? "已添加一条你的工作经历。"
                : message == ManageMessageId.ChangePasswordSuccess ? "修改密码成功。"
                : message == ManageMessageId.AcessDenied ? "你没有权限进行这项操作。"
                : message == ManageMessageId.ApplySuccess ? "申请加入成功，请等待团队管理员审批。"
                : message == ManageMessageId.ProjectSuccess ? "项目申请成功，请等待管理员审批。"
                : message == ManageMessageId.RecruitSuccess ? "招募请求发送成功，请等待该用户响应。"
                : message == ManageMessageId.UpdateUserProfileSuccess ? "修改个人信息成功。"
                : message == ManageMessageId.OperationSuccess ? "操作成功。"
                : message == ManageMessageId.AdminQuit ? "作为团队创始人你无法将自己从团队中删除。"
                : message == ManageMessageId.ApproveSuccess ? "成员列表审核成功！"
                : message == ManageMessageId.UserIdentitySuccess ? "申请实名认证成功，请等待管理员审批！"
                : "";
            }

            var user = Extensions.GetContextUser(ref db);
            var model = new ManageIndexViewModel
            {
                NewMessage = user.Messages.Where(m => m.Time >= DateTime.Now.AddMonths(-3)).OrderByDescending(m => m.Time).ToList()
            };
            return View(model);
        }

        #region 个人履历模块
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

                return View();
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

        public ActionResult UserActivity()
        {
            var model = Extensions.GetContextUser(ref db).Activity.OrderByDescending(a => a.Time);

            return View(model);
        }

        public ActionResult UserCourse()
        {
            var model = Extensions.GetContextUser(ref db).Course.OrderByDescending(c => c.Time);

            return View(model);
        }

        public ActionResult UserRoom()
        {
            var model = Extensions.GetContextUser(ref db).Room.OrderByDescending(r => r.Time);

            return View(model);
        }
        #endregion

        #region 用户信息与认证模块
        public ActionResult ChangePassword()
        {
            return View();
        }

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

        public ActionResult UserProfile()
        {
            User user = UserManager.FindById(User.Identity.GetUserId());
            Profile model = user.Profile;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserProfile(Profile model)
        {
            if (ModelState.IsValid)
            {
                using (BaseDbContext db = new BaseDbContext())
                {
                    db.Users.Find(User.Identity.GetUserId()).Profile = model;
                    db.SaveChanges();
                }
                TempData["Alert"] = "修改成功！";
                return View();
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult UserIdentity()
        {
            var status = db.Users.Find(User.Identity.GetUserId()).IdentityRecord;
            if (status != null)
            {
                switch (status.Status)
                {
                    case IdentityStatus.ToApprove:
                        TempData["Alert"] = "你已提交过实名认证，管理员尚未确认！";
                        break;
                    case IdentityStatus.Done:
                        TempData["Alert"] = "你通过实名认证！";
                        break;
                }

                return RedirectToAction("Index", "Manage");
            }
            ViewBag.School = Extensions.GetCurrentUser().School;
            ViewBag.Academy = Extensions.GetCurrentUser().Academy;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserIdentity(IdentityRecord model)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count != 2)
                    return View();
                /*var allowExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                foreach ( string f in Request.Files)
                {
                    if (!allowExtensions.Contains(Path.GetExtension(Request.Files.Get(f).FileName)))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }*/
                model.FrontIdCard = Material.Create("", MaterialType.Identity, Request.Files[0], db);
                if (model.FrontIdCard == null)
                {
                    TempData["Alert"] = "请检查上传文件！";
                    ViewBag.School = Extensions.GetCurrentUser().School;
                    ViewBag.Academy = Extensions.GetCurrentUser().Academy;
                    return View(model);
                }
                model.BackIdCard = Material.Create("", MaterialType.Identity, Request.Files[1], db);
                if (model.BackIdCard == null)
                {
                    db.Materials.Remove(model.FrontIdCard);
                    db.SaveChanges();
                    TempData["Alert"] = "请检查上传文件！";
                    ViewBag.School = Extensions.GetCurrentUser().School;
                    ViewBag.Academy = Extensions.GetCurrentUser().Academy;
                    return View(model);
                }
                model.Status = IdentityStatus.ToApprove;
                model.Time = DateTime.Now;
                model.Id = Guid.NewGuid();
                model.User = db.Users.Find(User.Identity.GetUserId());
                db.IdentityRecords.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.UserIdentitySuccess });
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }
        #endregion

        #region 项目、团队与公司模块
        public ActionResult Project()
        {
            User user = db.Users.Find(Extensions.GetUserId());
            ViewData["ProgressList"] = EnumExtension.GetSelectList(typeof(ProjectProgressType));
            var data = db.IndustryLists.OrderBy(i => i.IndustryName).ToList();
            if (data.Count() == 0)
            {
                data.Add(new IndustryList { ID = Guid.Empty, IndustryName = "空" });
            }
            ViewBag.Industry = new SelectList(data, "IndustryName", "IndustryName");

            if (user.TeamRecord != null && user.TeamRecord.Status != TeamMemberStatus.Admin)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (user.Project == null)
            {
                return View(new Project());
            }
            if (user.Project.Status == ProjectStatus.Denied)
            {
                TempData["DeniedInfo"] = "项目未通过,请重新申请。";
                return View(user.Project);
            }

            return RedirectToAction("ProjectProfile");
        }

        public ActionResult ProjectProfile()
        {
            User user = db.Users.Find(Extensions.GetUserId());
            if (user == null || user.Project == null)
            {
                TempData["Alert"] = "您请求的项目不存在！";
                return RedirectToAction("Index");
            }
            if (user.Project.Status == ProjectStatus.Denied)
            {
                return RedirectToAction("Project", user.Project);
            }

            return View(user.Project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Project(Project model)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.Find(Extensions.GetUserId());
                if (Request.Files.Count != 1)//如果文件列表为空则返回
                    return View();
                var file = Request.Files[0];//只上传第一个文件
                if (!MaterialType.Avatar.Match(file))
                {
                    TempData["Alert"] = "请上传格式为jpg, jpeg，png的图片";
                    model.Avatar = null;
                    var data = db.IndustryLists.OrderBy(i => i.IndustryName).ToList();
                    if (data.Count() == 0)
                    {
                        data.Add(new IndustryList { ID = Guid.Empty, IndustryName = "空" });
                    }
                    ViewBag.Industry = new SelectList(data, "IndustryName", "IndustryName");
                    ViewData["ProgressList"] = EnumExtension.GetSelectList(typeof(ProjectProgressType));
                    return View(model);
                }
                if (model.Avatar == null)
                {
                    model.Avatar = Material.Create("", MaterialType.Avatar, file, db);
                }
                else
                {
                    model.Avatar = Material.ChangeFile(model.Avatar.Id, file, db);
                }
                Project old = Extensions.GetContextUser(ref db).Project;
                if (old != null)
                {
                    db.Entry(db.Projects.Find(old.Id)).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
                model.NewProject(db);
                db.Projects.Add(model);
                user.Project = model;
                db.SaveChanges();

                return RedirectToAction("Index", new { Message = ManageMessageId.ProjectSuccess });
            }

            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult TeamApply(int page = 0)
        {
            if (Extensions.GetContextUser(ref db).TeamRecord != null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            int pageSize = 10;
            var list = new ListPage<Team>(db.Teams.Where(u => u.Searchable == true), page, pageSize);

            return View(list);
        }

        [ActionName("DoTeamApply")]
        public ActionResult TeamApply(Guid teamId)
        {
            if (Extensions.GetContextUser(ref db).TeamRecord != null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            Team team = db.Teams.Find(teamId);
            if (team == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            db.TeamRecords.Add(new TeamRecord(team, TeamMemberStatus.Apply, Extensions.GetContextUser(ref db)));
            db.Messages.Add(new Message(team.Admin.Id, MessageType.System, MessageTemplate.TeamApply, ref db));
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.ApplySuccess });
        }

        public ActionResult TeamRecruit(int page = 0)
        {
            if (!IsTeamAdmin())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            int pageSize = 10;
            var list = new ListPage<User>(db.Users.Where(u => u.Profile.Searchable == true), page, pageSize);

            return View(list);
        }

        [ActionName("DoTeamRecruit")]
        public ActionResult TeamRecruit(string userId)
        {
            if (!IsTeamAdmin())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            User user = db.Users.Find(userId);
            Team team = db.Teams.First(u => u.Admin.Id == Extensions.GetContextUser(ref db).Id);
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            db.TeamRecords.Add(new TeamRecord(team, TeamMemberStatus.Recruit, user));
            db.Messages.Add(new Message(user.Id, MessageType.System, MessageTemplate.TeamRecruit, ref db));
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.RecruitSuccess });
        }

        public ActionResult UserDetails(string userId)
        {
            User user = db.Users.Find(userId);
            if (user == null || user.Profile.Searchable == false)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });

            return View(user);
        }

        public ActionResult TeamAccess(string userId, bool isApprove)
        {
            if (!IsTeamAdmin())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            User applicant = db.Users.Find(userId);
            User user = Extensions.GetContextUser(ref db);
            if (applicant == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ApplyRecord = applicant.TeamRecord;
            if (ApplyRecord == null || applicant.TeamRecord.Team.Id != user.TeamRecord.Team.Id)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (isApprove)
            {
                ApplyRecord.Status = TeamMemberStatus.Normal;
                applicant.Project = user.Project;
                db.Entry(ApplyRecord).State = EntityState.Modified;
                db.Messages.Add(new Message(applicant.Id, MessageType.System, MessageTemplate.TeamApplySuccess, ref db));
                db.TeamEvents.Add(new TeamEvent
                {
                    Id = Guid.NewGuid(),
                    Team = ApplyRecord.Team,
                    AddTime = DateTime.Now,
                    EventTime = DateTime.Now,
                    EventContent = string.Format("【{0}】加入团队！", applicant.DisplayName),
                    EventName = "新成员加入团队"
                });
            }
            else
            {
                db.Entry(ApplyRecord).State = EntityState.Deleted;
                db.Messages.Add(new Message(applicant.Id, MessageType.System, MessageTemplate.TeamApplyFailure, ref db));
            }
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.ApproveSuccess });
        }

        public ActionResult TeamMember()
        {
            if (Extensions.GetContextUser(ref db).TeamRecord == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            Team team = db.Users.Find(Extensions.GetUserId()).TeamRecord.Team;
            if (team == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            List<TeamRecord> teamMember;
            if (!IsTeamAdmin())
            {
                teamMember = team.Member.Where(m => m.Status == TeamMemberStatus.Normal || m.Status == TeamMemberStatus.Admin).ToList();
            }
            else
            {
                teamMember = team.Member.ToList();
            }

            return View(teamMember);
        }
        public ActionResult TeamMemberDelete(string id)
        {
            if (!IsTeamAdmin())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            var user = Extensions.GetContextUser(ref db);
            var member = db.Users.Find(id);
            if (id == null || member == null)
                return HttpNotFound();
            var teamRecord = user.TeamRecord;
            var team = teamRecord.Team;
            if (member.TeamRecord.Status == TeamMemberStatus.Admin)
            {
                if (team.Member.Count == 1)
                {
                    db.Projects.Remove(user.Project);
                    db.Teams.Remove(team);
                    db.TeamRecords.Remove(teamRecord);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", new { message = ManageMessageId.AdminQuit });
            }
            db.TeamRecords.Remove(member.TeamRecord);
            member.TeamRecord = null;
            member.Project = null;
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.OperationSuccess });
        }

        public ActionResult TeamMemberQuit()
        {
            if (Extensions.GetContextUser(ref db).TeamRecord == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (IsTeamAdmin())
            {
                return RedirectToAction("Index", new { message = ManageMessageId.AdminQuit });
            }
            User member = Extensions.GetContextUser(ref db);
            db.TeamRecords.Remove(member.TeamRecord);
            member.Project = null;
            member.TeamRecord = null;
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.OperationSuccess });
        }

        public ActionResult TeamEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            if (!IsTeamAdmin())
            {
                TempData["Alert"] = "你没有权限进行此操作！";
                return RedirectToAction("Index");
            }

            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeamEdit(Team team)
        {
            if (Extensions.GetContextUser(ref db).TeamRecord == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (!IsTeamAdmin())
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            Team editTeam = db.Teams.Find(team.Id);
            editTeam.Name = team.Name;
            editTeam.Introduction = team.Introduction;
            editTeam.Announcement = team.Announcement;
            editTeam.Searchable = team.Searchable;
            db.Entry(editTeam).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.OperationSuccess });
        }

        public ActionResult TeamProfile()
        {
            if (!IsTeamAdmin())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            Team team = Extensions.GetContextUser(ref db).TeamRecord.Team;
            TeamProfileViewModel model = new TeamProfileViewModel
            {
                Id = team.Id,
                Name = team.Name,
                Administrator = team.Admin.DisplayName,
                Time = team.Time,
                Introduction = team.Introduction,
                Announcement = team.Announcement
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeamProfile([Bind(Include = "Annoucement,Introduction,Searchable")]TeamProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!IsTeamAdmin())
                    return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
                Team team = db.Teams.First(t => t.Id == Extensions.GetContextUser(ref db).TeamRecord.Team.Id);
                team.Announcement = model.Announcement;
                team.Introduction = model.Introduction;
                team.Searchable = model.Searchable;
                db.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.OperationSuccess });
            }

            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        public ActionResult Company()
        {
            if (!IsTeamAdmin())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (Extensions.GetContextUser(ref db).TeamRecord.Team.Company != null)
            {
                TempData["Alert"] = "请不要重复申请！";
                return RedirectToAction("Index");
            }

            ViewBag.Status = CompanyStatus.None;
            return View(new Company());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Company(Company model)
        {
            var user = Extensions.GetContextUser(ref db);
            if (!IsTeamAdmin())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (user.TeamRecord.Team.Company != null)
                return RedirectToAction("Index", new { message = ManageMessageId.AcessDenied });
            if (ModelState.IsValid)
            {
                if (Request.Files.Count != 1)//如果文件列表为空则返回
                {
                    ViewBag.Alert = "请检查上传文件！";
                    return View();
                }
                var file = Request.Files[0];//只上传第一个文件
                model.NewCompany(ref db);
                model.Plan = Material.Create("商业计划书", MaterialType.Management, file, db);
                model.Admin = user;
                user.TeamRecord.Team.Company = model;
                db.SaveChanges();
                return RedirectToAction("Index", new { message = ManageMessageId.OperationSuccess });
            }

            return View();
        }

        public ActionResult Avatar(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = db.Materials.Find(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Avatar(Material model)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count != 1)//如果文件列表为空则返回
                    return View();
                var file = Request.Files[0];//只上传第一个文件
                //根据日期生成服务器端文件名
                string uploadFileName = model.Id + Path.GetExtension(file.FileName);
                //生成服务器端绝对路径
                string absolutFileName = Server.MapPath("~/") + "UserUpload/Avatar/" + uploadFileName;
                //执行上传
                if (System.IO.File.Exists(absolutFileName))
                {
                    System.IO.File.Delete(absolutFileName);
                }
                if (!Directory.Exists(Path.GetDirectoryName(absolutFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(absolutFileName));
                }
                file.SaveAs(absolutFileName);
                model.Name = uploadFileName;
                //添加Material记录
                db.Materials.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                //保存更改
                db.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.OperationSuccess });
            }

            ViewBag.Error = "存在错误，请检查输入。";
            return View();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            if (db != null)
            {
                db.Dispose();
                db = null;
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

        public bool IsTeamAdmin()
        {
            var user = Extensions.GetContextUser(ref db);
            var teamRecord = user.TeamRecord;
            if (teamRecord == null || teamRecord.Status != TeamMemberStatus.Admin)
                return false;
            return true;
        }

        public bool IsTeamMember()
        {
            var user = Extensions.GetContextUser(ref db);
            var teamRecord = user.TeamRecord;
            if (teamRecord == null || teamRecord.Status != TeamMemberStatus.Normal)
                return false;
            return true;
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
            ApproveSuccess,
            AcessDenied,
            AdminQuit,
            Error
        }

        #endregion
    }
}