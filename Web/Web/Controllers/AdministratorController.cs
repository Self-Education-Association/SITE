using Ganss.XSS;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Users = "admin@uibe.edu.cn")]
    public class AdministratorController : Controller
    {
        private BaseDbContext db = new BaseDbContext();
        private int pageSize = 15;

        #region 用户管理容器
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
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
        #endregion

        // GET: Administrator
        public ActionResult Index(AdminOperationStatus? status)
        {
            ViewBag.Alert =
                status == AdminOperationStatus.Error ? "操作失败。"
                : status == AdminOperationStatus.Success ? "操作成功。" : "";

            return View();
        }
        //4 Views
        #region 文章管理模块
        public ActionResult Articles(int page = 0)
        {

            return View(new ListPage<Article>(db.Articles, page, pageSize));
        }

        public ActionResult ArticleCreate()
        {
            ViewData["StatusList"] = EnumExtension.GetSelectList(typeof(ArticleStatus));
            ViewData["ClassList"] = EnumExtension.GetSelectList(typeof(ArticleClass));

            return View(new Article());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ArticleCreate(Article model)
        {
            var s = new HtmlSanitizer();
            if (ModelState.IsValid)
            {
                model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
                var file = Request.Files[0];
                if (Request.Files.Count >= 1 && file.FileName != "")
                {
                    if (MaterialType.Avatar.Match(file))
                    {
                        model.Image = Material.Create("", MaterialType.Avatar, Request.Files[0], db);
                        if (model.Image == null)
                        {
                            ViewData["StatusList"] = EnumExtension.GetSelectList(typeof(ArticleStatus));
                            ViewData["ClassList"] = EnumExtension.GetSelectList(typeof(ArticleClass));
                            return View(model);
                        }
                    }
                    else
                    {
                        ViewData["StatusList"] = EnumExtension.GetSelectList(typeof(ArticleStatus));
                        ViewData["ClassList"] = EnumExtension.GetSelectList(typeof(ArticleClass));
                        TempData["Alert"] = "请检查上传文件的格式是否正确！";
                        return View(model);
                    }
                }
                else
                {
                    model.Image = db.Materials.Find(Guid.Empty.DefaultMaterial(DefaultMaterial.News));
                }
                model.NewArticle();
                db.Articles.Add(model);
                db.SaveChanges();
                TempData["Alert"] = "文章创建成功！";
                return RedirectToAction("Articles");
            }

            model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
            ViewData["StatusList"] = EnumExtension.GetSelectList(typeof(ArticleStatus));
            ViewData["ClassList"] = EnumExtension.GetSelectList(typeof(ArticleClass));
            return View();
        }

        public ActionResult ArticleEdit(Guid? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Article model = db.Articles.Find(Id);
            if (model == null)
                return HttpNotFound();
            ViewData["StatusList"] = EnumExtension.GetSelectList(typeof(ArticleStatus));
            ViewData["ClassList"] = EnumExtension.GetSelectList(typeof(ArticleClass));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ArticleEdit(Article model)
        {
            var s = new HtmlSanitizer();
            if (ModelState.IsValid)
            {
                model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
                db.Articles.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["Alert"] = "文章编辑成功！";
                return RedirectToAction("Articles");
            }

            model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
            ViewData["StatusList"] = EnumExtension.GetSelectList(typeof(ArticleStatus));
            ViewData["ClassList"] = EnumExtension.GetSelectList(typeof(ArticleClass));
            return View(model);
        }

        public ActionResult ArticleDelete(Guid? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Article model = db.Articles.Find(Id);
            if (model == null)
                return HttpNotFound();

            db.Articles.Remove(model);
            db.SaveChanges();
            TempData["Alert"] = "文章删除成功！";
            return RedirectToAction("Articles");
        }
        #endregion
        //4 Views
        #region 活动管理模块
        public ActionResult Activities(int page = 0)
        {
            return View(new ListPage<ActivityOperation>(db.ActivityOperations, page, pageSize));
        }

        public ActionResult ActivityCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ActivityCreate(ActivityOperation model)
        {
            var s = new HtmlSanitizer();
            if (ModelState.IsValid)
            {
                if (model.StartTime > model.EndTime)
                {
                    ViewData["Alert"] = "活动开始时间必须在结束时间之前。";
                    model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
                    return View(model);
                }
                model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
                model.NewActivity(db);
                db.ActivityOperations.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }

            model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
            return View(model);
        }

        public ActionResult ActivityEdit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = db.ActivityOperations.Find(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ActivityEdit(ActivityOperation model)
        {
            var s = new HtmlSanitizer();
            if (ModelState.IsValid)
            {
                model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"])); ;
                db.ActivityOperations.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }

            model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
            return View(model);
        }

        public ActionResult ActivityDelete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = db.ActivityOperations.Find(id);
            if (model == null)
                return HttpNotFound();
            db.Entry(model).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
        }

        public ActionResult DeleteConfirm(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = db.ActivityOperations.Find(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }
        #endregion
        //5 Views
        #region 上传文件模块
        // GET: Materials
        public ActionResult Materials(int page = 0)
        {
            IListPage test = new Material();
            var model = new ListPage<Material>(db.Materials.Where(m => m.Type == MaterialType.Activity || m.Type == MaterialType.Download || m.Type == MaterialType.Slider), page, pageSize); //实现分页功能
            return View(model);
        }

        // GET: Materials/Details/5
        public ActionResult MaterialDetails(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        // GET: Materials/Create
        public ActionResult MaterialCreate()
        {
            ViewData["TypeList"] = EnumExtension.GetSelectList(typeof(MaterialType));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MaterialCreate([Bind(Include = "Name,Description,Type")] Material material)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count != 1)//如果文件列表为空则返回
                    return View();
                var file = Request.Files[0];//只上传第一个文件
                Material.Create(material.Description, material.Type, file, db);
                return RedirectToAction("Materials");
            }

            return View(material);
        }

        // GET: Materials/Edit/5
        public ActionResult MaterialEdit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            ViewData["TypeList"] = EnumExtension.GetSelectList(typeof(MaterialType));
            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MaterialEdit([Bind(Include = "ID,Description,Type")] Material material)
        {
            if (ModelState.IsValid)
            {
                db.Materials.Find(material.Id).Description = material.Description;
                db.Materials.Find(material.Id).Type = material.Type;
                db.SaveChanges();
                return RedirectToAction("Materials");
            }
            return View(material);
        }

        // GET: Materials/Delete/5
        public ActionResult MaterialDelete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("MaterialDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Material material = db.Materials.Find(id);
            db.Materials.Remove(material);
            db.SaveChanges();
            return RedirectToAction("Materials");
        }
        #endregion
        //3 Views
        #region 用户管理模块
        public ActionResult UserList()
        {
            return View(db.Users.ToList());
        }
        public ActionResult TutorCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> TutorCreate(CreateTutorViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count != 1)//如果文件列表为空则返回
                {
                    ViewBag.Alert = "请检查上传文件！";

                    return View();
                }

                var file = Request.Files[0];//只上传第一个文件

                var user = Models.User.Create(model.Email, model.DisplayName);
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //为账户添加角色
                    var roleName = "Tutor";
                    ApplicationRoleManager roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(db));

                    //判断角色是否存在
                    if (!roleManager.RoleExists(roleName))
                    {
                        //角色不存在则建立角色
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                    //将用户加入角色
                    await UserManager.AddToRoleAsync(user.Id, roleName);

                    // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                    // 发送包含此链接的电子邮件
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");

                    var avatar = Material.Create(user.DisplayName, MaterialType.Avatar, file, db);
                    if (avatar == null)
                    {
                        TempData["ALert"] = "请检查上传文件！";
                        return View(model);
                    }
                    var tutor = new TutorInformation { Id = Guid.NewGuid(), Tutor = db.Users.Find(user.Id), Avatar = avatar, Position = model.Position, Introduction = model.Introduction };
                    db.TutorInformations.Add(tutor);
                    db.SaveChanges();
                    ViewBag.Alert = "操作成功！";

                    return View();
                }
            }
            ViewBag.Alert = "操作失败！";

            return View();
        }

        public ActionResult UserEdit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            User model = db.Users.Find(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit([Bind(Include = "DisplayName,Email,PhoneNumber,Identitied,IsDisabled,Id")]User model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.Id);
                user.DisplayName = string.IsNullOrWhiteSpace(model.DisplayName) ? user.DisplayName : model.DisplayName;
                user.Email = model.Email == null ? user.Email : model.Email;
                user.PhoneNumber = model.PhoneNumber == null ? user.PhoneNumber : model.PhoneNumber;
                user.Profile.Email = user.Email;
                user.Profile.Phone = user.PhoneNumber;
                user.Identitied = model.Identitied;
                user.IsDisabled = model.IsDisabled;

                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }
            return View();
        }
        #endregion
        //2 Views
        #region 审核认证记录模块
        public IdentityType Type { get; set; }
        public ActionResult IdentityRecords(int page = 0)
        {
            var user = new ListPage<IdentityRecord>(db.IdentityRecords.Where(i => i.Status == IdentityStatus.ToApprove), page, pageSize);
            return View("IdentityRecords", user);
        }

        public ActionResult ProjectIdentityRecords(int page = 0)
        {
            var project = new ListPage<Project>(db.Projects.Where(i => i.Status == ProjectStatus.ToApprove), page, pageSize);
            return View(project);
        }

        public ActionResult CompanyIdentityRecords(int page = 0)
        {
            var company = new ListPage<Company>(db.Companys.Where(i => i.Status == CompanyStatus.ToApprove), page, pageSize);
            return View(company);

        }

        public ActionResult IdentityRecordDetails(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            IdentityRecord user = db.IdentityRecords.Find(id);
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(user);
        }

        public ActionResult ProjectIdentityRecordDetails(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Project project = db.Projects.Find(id);
            if (project == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            ViewData["ProgressList"] = EnumExtension.GetSelectList(typeof(ProjectProgressType));
            return View(project);
        }

        public ActionResult CompanyIdentityRecordDetails(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Company company = db.Companys.Find(id);
            if (company == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(company);
        }

        public ActionResult ProjectIdentityRecordApprove(Guid? id, bool isApprove)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Project project = db.Projects.Find(id);
            if (project == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (project.Status != ProjectStatus.ToApprove)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (isApprove)
            {
                project.Status = ProjectStatus.Done;
                db.Messages.Add(new Message(project.Admin.Id, MessageType.System, MessageTemplate.ProjectSuccess, ref db));
                Team team = new Team();
                team.NewTeam(ref project);
                db.TeamRecords.Add(new TeamRecord(team, TeamMemberStatus.Admin, project.Admin));
                db.TeamEvents.Add(new TeamEvent { Id = Guid.NewGuid(), AddTime = DateTime.Now, EventTime = DateTime.Now, EventContent = "通过管理员审批，在本网站上开始招募团队。", EventName = "创建团队", Team = team });
            }
            else
            {
                project.Status = ProjectStatus.Denied;
                db.Messages.Add(new Message(project.Admin.Id, MessageType.System, MessageTemplate.ProjectFailure, ref db));
            }
            db.SaveChanges();
            return RedirectToAction("ProjectIdentityRecords");
        }

        public ActionResult IdentityRecordApprove(Guid? id, bool isApprove)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            IdentityRecord user = db.IdentityRecords.Find(id);
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (user.Status != IdentityStatus.ToApprove)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (isApprove)
            {
                user.Status = IdentityStatus.Done;
                user.User.Identitied = true;
                db.Messages.Add(new Message(user.User.Id, MessageType.System, MessageTemplate.IdentityRecordSuccess, ref db));
                TempData["Alert"] = "审核通过成功！";
            }
            else
            {
                user.User.Identitied = false;
                db.Messages.Add(new Message(user.User.Id, MessageType.System, MessageTemplate.IdentityRecordFailure, ref db));
                db.Materials.Remove(user.FrontIdCard);
                db.Materials.Remove(user.BackIdCard);
                db.IdentityRecords.Remove(user);
                TempData["Alert"] = "审核驳回成功！";
            }
            db.SaveChanges();
            return RedirectToAction("IdentityRecords");
        }
        public ActionResult CompanyIdentityRecordApprove(Guid? id, bool isApprove)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Company company = db.Companys.Find(id);
            if (company == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (company.Status != CompanyStatus.ToApprove)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (isApprove)
            {
                company.Status = CompanyStatus.Done;
                db.Messages.Add(new Message(company.Admin.Id, MessageType.System, MessageTemplate.CompanySuccess, ref db));
                db.TeamEvents.Add(new TeamEvent
                {
                    Id = Guid.NewGuid(),
                    Team = company.Admin.TeamRecord.Team,
                    AddTime = DateTime.Now,
                    EventTime = DateTime.Now,
                    EventName = "注册公司",
                    EventContent = "团队成功通过管理员审核，注册成为一家公司。"
                });
            }
            else
            {
                company.Status = CompanyStatus.Denied;
                db.Messages.Add(new Message(company.Admin.Id, MessageType.System, MessageTemplate.CompanyFailure, ref db));
            }
            db.SaveChanges();
            return RedirectToAction("CompanyIdentityRecords");
        }
        #endregion

        #region 导出模块
        public ActionResult Output()
        {
            var projects = db.Projects.ToList();
            MemoryStream file = new MemoryStream();
            StreamWriter sw = new StreamWriter(file, Encoding.GetEncoding("GBK"));
            sw.Write("项目名称,立项时间,项目行业,团队事项的更新时间,团队成员,电子邮箱,所在学校,联系方式");
            sw.WriteLine();
            foreach (Project project in projects)
            {
                sw.Write(project.Name+",");
                sw.Write(project.Time + ",");
                sw.Write(project.Industry + ",");
                sw.Write(project.Team.Events.OrderByDescending(p => p.AddTime).First().AddTime + ",");
                foreach (TeamRecord record in project.Team.Member.Where(m => m.Status == TeamMemberStatus.Admin || m.Status==TeamMemberStatus.Normal ))
                {
                    User member = record.Receiver;
                    sw.Write(member.DisplayName + ",");
                    sw.Write(member.Profile.Email + ",");
                    sw.Write(member.School + ",");
                    sw.Write(member.Profile.Phone);
                    sw.WriteLine();
                    sw.Write(",,,,");
                }
                sw.Write(",,");
                sw.WriteLine();
            }
            sw.Flush();
            file.Position = 0;
            return File(file, "text/comma-separated-values", "output.csv");
        }


        #endregion

        #region 项目团队管理模块
        // GET: Reports
        public ActionResult Reports(int page = 0)
        {
            int pageindex = 20;

            return View(new ListPage<TeamReport>(db.TeamReports, page, pageindex));
        }

        public ActionResult NewReportRound()
        {
            TempData["Alert"] = "请注意，新建团队报告上传场次之后，所有团队的上传状态都将被清零，重新计算！";
            return View(new TeamReportRound());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewReportRound(TeamReportRound round)
        {
            if (ModelState.IsValid)
            {
                if (round.StartTime >= round.EndTime || round.EndTime <= DateTime.Now)
                {
                    TempData["Alert"] = "请检查时间输入是否有误！";
                    return View(round);
                }
                var appSettings = new AppSettings();
                appSettings.ReportRoundName = round.Name;
                appSettings.ReportStartTime = round.StartTime;
                appSettings.ReportEndTime = round.EndTime;

                foreach (var team in db.Teams)
                {
                    team.ReportUpdated = false;
                }
                db.SaveChanges();
                TempData["Alert"] = string.Format("新建场次【{0}】成功，允许上传时间为{1}-{2}！", round.Name, round.StartTime, round.EndTime);
                return RedirectToAction("Reports");
            }

            return View(round);
        }

        // GET: Reports/Details/5
        public ActionResult ReportDetails(Guid? id)
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
        // GET: Reports/Delete/5
        public ActionResult ReportDelete(Guid? id)
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

        // POST: Reports/ReportDelete/5
        [HttpPost, ActionName("ReportDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult ReportDeleteConfirmed(Guid id)
        {
            TeamReport teamReport = db.TeamReports.Find(id);
            db.TeamReports.Remove(teamReport);
            db.SaveChanges();
            return RedirectToAction("Reports");
        }

        public ActionResult DownloadReports()
        {
            var data = db.TeamReports.DistinctBy(t => t.Round);

            var i = new SelectList(data, "Round", "Round").ToList();
            i.Add(new SelectListItem { Text = "全部报告", Value = "" });
            ViewBag.List = i;

            return View();
        }

        [HttpPost]
        public ActionResult DownloadReports(string round)
        {
            string tempPath = HttpContext.Server.MapPath("~/") + @"Temp\" + Extensions.GetCurrentUser().UserName + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + @"\";
            string zipName = HttpContext.Server.MapPath("~/") + @"Temp\" + Extensions.GetCurrentUser().UserName + @"\" + round + "Reports.zip";
            if (string.IsNullOrWhiteSpace(round))
            {
                var collection = db.TeamReports.ToList();
                if (collection == null || collection.Count() == 0)
                {
                    TempData["Alert"] = "未找到任何报告记录！";
                    return View();
                }
                foreach (var item in collection)
                {
                    string destFileName = tempPath + item.Round + @"\";
                    if (!Directory.Exists(destFileName))
                    {
                        Directory.CreateDirectory(destFileName);
                    }
                    System.IO.File.Copy(item.ReportFile.GetPath(), destFileName + item.Team.Name + Path.GetExtension(item.ReportFile.Name));
                }
            }
            else
            {
                var collection = db.TeamReports.Where(t => t.Round == round).ToList();
                if (collection == null || collection.Count() == 0)
                {
                    TempData["Alert"] = "未找到该场次的任何报告记录，请检查场次名称是否正确！";
                    return View();
                }
                foreach (var item in collection)
                {
                    string destFileName = tempPath;
                    if (!Directory.Exists(destFileName))
                    {
                        Directory.CreateDirectory(destFileName);
                    }
                    System.IO.File.Copy(item.ReportFile.GetPath(), tempPath + round + "-" + item.Team.Name + Path.GetExtension(item.ReportFile.Name));
                }
            }
            ZipHelper.CreateZip(tempPath, zipName);
            return File(zipName, "application/x-zip-compressed", Path.GetFileName(zipName));
        }

        public ActionResult Projects(int page = 0)
        {
            int pageSize = 20;
            return View(new ListPage<Project>(db.Projects, page, pageSize));
        }

        public ActionResult Companies(int page = 0)
        {
            int pageSize = 20;
            return View(new ListPage<Company>(db.Companys, page, pageSize));
        }
        #endregion

        #region 释放资源模块
        protected override void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
            base.Dispose(disposing);
        }
        #endregion

        public enum AdminOperationStatus
        {
            Success,
            Error,
        }
    }
}