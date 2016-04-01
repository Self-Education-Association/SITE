using System;
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
            ViewBag.StatusMessage =
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
                : "";

            var userId = User.Identity.GetUserId();
            var model = new ManageIndexViewModel
            {
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
                    return View(model);
                }
                model.BackIdCard = Material.Create("", MaterialType.Identity, Request.Files[1], db);
                if (model.BackIdCard == null)
                {
                    db.Materials.Remove(model.FrontIdCard);
                    db.SaveChanges();
                    TempData["Alert"] = "请检查上传文件！";
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
            if (user.TeamRecord != null && user.TeamRecord.Status != TeamMemberStatus.Admin)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (user.Project == null)
            {
                var data = db.IndustryList.ToList();
                if (data.Count() == 0)
                {
                    data.Add(new IndustryList { ID = Guid.Empty, IndustryName = "空" });
                }
                ViewBag.Industry = new SelectList(data, "IndustryName", "IndustryName");
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
            User admin = db.Users.Where(u => u.TeamRecord.Team.Id == team.Id && u.TeamRecord.Status == TeamMemberStatus.Admin).First();
            db.Messages.Add(new Message(admin.Id, MessageType.System, MessageTemplate.TeamApply, ref db));
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.ApplySuccess });
        }

        public ActionResult TeamRecruit(int page = 0)
        {
            if (IllegalIdentity())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            int pageSize = 10;
            var list = new ListPage<User>(db.Users.Where(u => u.Profile.Searchable == true), page, pageSize);

            return View(list);
        }

        [ActionName("DoTeamRecruit")]
        public ActionResult TeamRecruit(string userId)
        {
            if (IllegalIdentity())
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
            if (user == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });

            return View(user);
        }

        public ActionResult TeamAccess(int page = 0)
        {
            if (IllegalIdentity())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            int pageSize = 10;
            var list = new ListPage<User>((from u in db.TeamRecords
                                           where u.Team.Admin.Id == Extensions.GetContextUser(ref db).Id &&   //团队管理为该用户的团队
                                           u.Status == TeamMemberStatus.Apply                             //状态为申请
                                           select u.Receiver), page, pageSize);

            return View(list);
        }

        [ActionName("DoTeamAccess")]
        public ActionResult TeamAccess(string userId, bool IsApprove)
        {
            if (IllegalIdentity())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            User applicant = db.Users.Find(userId);
            Team team = db.Teams.First(u => u.Admin.Id == Extensions.GetContextUser(ref db).Id);
            if (applicant == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ApplyRecord = db.TeamRecords.First(t => t.Team.Id == team.Id && t.Receiver.Id == applicant.Id && t.Status == TeamMemberStatus.Apply);
            if (ApplyRecord == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (IsApprove)
            {
                ApplyRecord.Status = TeamMemberStatus.Normal;
                applicant.Project = Extensions.GetContextUser(ref db).Project;
                db.Entry(ApplyRecord).State = System.Data.Entity.EntityState.Modified;
                db.Messages.Add(new Message(applicant.Id, MessageType.System, MessageTemplate.TeamApplySuccess, ref db));
            }
            else
            {
                db.Entry(ApplyRecord).State = System.Data.Entity.EntityState.Deleted;
                db.Messages.Add(new Message(applicant.Id, MessageType.System, MessageTemplate.ProjectFailure, ref db));
            }
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.RecruitSuccess });
        }

        public ActionResult TeamMember(int page = 0)
        {
            if (Extensions.GetContextUser(ref db).TeamRecord == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            Team team = db.Users.Find(Extensions.GetUserId()).TeamRecord.Team;
            if (team == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            var teamMember = team.Member.Where(m => m.Status == TeamMemberStatus.Normal || m.Status == TeamMemberStatus.Admin);

            return View(teamMember);
        }
        public ActionResult TeamMemberDelete(string id)
        {
            if (IllegalIdentity())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            User member = db.Users.Find(id);
            if (member.TeamRecord.Status == TeamMemberStatus.Admin)
            {
                var user = Extensions.GetContextUser(ref db);
                var teamRecord = user.TeamRecord;
                var team = teamRecord.Team;
                if (team.Member.Count == 1)
                {
                    if (TempData["isConfirmed"].ToString() != "true")
                    {
                        TempData["Alert"] = "这将删除你的项目及团队去，该操作无法恢复！请确定是否想要进行该操作?";
                        TempData["isConfirmed"] = "1";
                        return RedirectToAction("TeamMember");
                    }
                    else
                    {
                        db.Projects.Remove(user.Project);
                        db.Teams.Remove(team);
                        db.TeamRecords.Remove(teamRecord);
                        db.SaveChanges();
                        TempData["isConfirmed"] = "0";
                        TempData["Alert"] = "删除成功！";
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            }
            member.Project = null;
            db.Entry(member.TeamRecord).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Index", new { Message = ManageMessageId.OperationSuccess });
        }

        public ActionResult TeamMemberQuit()
        {
            if (Extensions.GetContextUser(ref db).TeamRecord == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (!IllegalIdentity())
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            User member = Extensions.GetContextUser(ref db);
            member.Project = null;
            db.Entry(member.TeamRecord).State = System.Data.Entity.EntityState.Deleted;
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
            if (IllegalIdentity())
            {
                TempData["Alert"] = "你没有权限进行此操作！";
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeamEdit(Team team)
        {
            if (Extensions.GetContextUser(ref db).TeamRecord == null)
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (IllegalIdentity())
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
            if (IllegalIdentity())
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
                if (IllegalIdentity())
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
            if (IllegalIdentity())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (Extensions.GetContextUser(ref db).TeamRecord.Team.Company != null)
            {
                ViewBag.Status = Extensions.GetContextUser(ref db).TeamRecord.Team.Company.Status;
                return View(Extensions.GetContextUser(ref db).TeamRecord.Team.Company);
            }

            ViewBag.Status = CompanyStatus.None;
            return View(new Company());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Company(Company model)
        {
            if (IllegalIdentity())
                return RedirectToAction("Index", new { Message = ManageMessageId.AcessDenied });
            if (ModelState.IsValid)
            {
                if (Request.Files.Count != 1)//如果文件列表为空则返回
                {
                    ViewBag.Alert = "请检查上传文件！";
                    return View();
                }
                var file = Request.Files[0];//只上传第一个文件
                var user = Extensions.GetContextUser(ref db);
                model.Plan = Material.Create("商业计划书", MaterialType.Management, file, db);
                model.Admin = user;
                user.TeamRecord.Team.Company = model;
                db.SaveChanges();
                return View();
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

        public bool IllegalIdentity()
        {
            var user = Extensions.GetContextUser(ref db);
            var teamRecord = user.TeamRecord;
            var project = user.Project;
            if (teamRecord == null || project == null)
                return true;
            if (project.Status != ProjectStatus.Done || teamRecord.Status != TeamMemberStatus.Admin)
                return true;
            return false;
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