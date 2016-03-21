using Ganss.XSS;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
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
            ViewBag.StatusMessage =
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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ArticleCreate(Article model)
        {
            if (ModelState.IsValid)
            {
                var s = new HtmlSanitizer();
                model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"]));
                if (Request.Files.Count >= 1 && Request.Files[0].FileName != "")
                {
                    model.Image = Material.Create("", MaterialType.Avatar, Request.Files[0], db);
                    if (model.Image == null)
                    {
                        ViewData["StatusList"] = EnumExtension.GetSelectList(typeof(ArticleStatus));
                        ViewData["ClassList"] = EnumExtension.GetSelectList(typeof(ArticleClass));
                        return View();
                    }
                }
                else
                {
                    model.Image = db.Materials.Find(Guid.Empty.DefaultMaterial(DefaultMaterial.News));
                }
                model.NewArticle();
                db.Articles.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }

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
            if (ModelState.IsValid)
            {
                var s = new HtmlSanitizer();
                model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"])); ;
                db.Articles.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }

            ViewData["StatusList"] = EnumExtension.GetSelectList(typeof(ArticleStatus));
            ViewData["ClassList"] = EnumExtension.GetSelectList(typeof(ArticleClass));
            return View();
        }

        public ActionResult ArticleDeleteConfirm(Guid? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Article model = db.Articles.Find(Id);
            if (model == null)
                return HttpNotFound();

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

            return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
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
            if (ModelState.IsValid)
            {
                if (model.StartTime > model.EndTime)
                {
                    ViewData["ErrorInfo"] = "活动开始时间必须在结束时间之前。";
                    return View();
                }
                var s = new HtmlSanitizer();
                model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"])); ;
                model.NewActivity(db);
                db.ActivityOperations.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }
            return View();
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
            if (ModelState.IsValid)
            {
                var s = new HtmlSanitizer();
                model.Content = Server.HtmlDecode(s.Sanitize(Request.Params["ck"])); ;
                db.ActivityOperations.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }
            return View();
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
        //constructing
        #region 场地管理模块
        public ActionResult Index()
        {
            return View(RoomOperation.List("", true));
        }

        //返回创建场地的页面
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,Status")] RoomOperation roomOperation)
        {
            if (ModelState.IsValid && roomOperation != null)
            {
                if (roomOperation.StartTime >= roomOperation.EndTime)
                {
                    TempData["ErrorInfo"] = "无法创建场地，开始时间晚于结束时间。";
                    return View();
                }
                //创建成功返回至列表菜单
                if (roomOperation.Create())
                    return RedirectToAction("Index");
            }
            TempData["ErrorInfo"] = "错误：无法创建场地，对象不存在或无效。";
            return View(roomOperation);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomOperation roomOperation = db.RoomOperations.Find(id);
            if (roomOperation == null)
            {
                return HttpNotFound();
            }
            return View(roomOperation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Count,Limit,Location,Name,StartTime,EndTime,Content,Status")] RoomOperation roomOperation)
        {
            if (ModelState.IsValid && roomOperation != null)
            {
                if (roomOperation.StartTime >= roomOperation.EndTime)
                {
                    TempData["ErrorInfo"] = "无法完成修改，开始时间晚于结束时间。";
                    return View();
                }
                if (roomOperation.Edit())
                {
                    var user = Extensions.GetContextUser(ref db);
                    var RoomRecords = roomOperation.RoomRecords;
                    var lastRecord = RoomRecords.Where(r => r.ActionTime.AddDays(7.0) > r.RoomOperation.StartTime);
                    if (RoomRecords != null && lastRecord != null)
                    {
                        string title = "场地修改通知";
                        string content = "您好，你选择的场地[" + roomOperation.Name + "]已被修改，请及时查看相关信息，并根据新的场地信息安排你的日程";
                        Message message = new Message(title, content, lastRecord.First().Receiver.Id, MessageType.System, db);
                        if (message.Publish())
                        {
                            return RedirectToAction("Index");
                        }
                        TempData["ErrorInfo"] = "无法给学生发布修改信息";
                    }
                }
                    TempData["ErrorInfo"] = "修改失败!";
            }
            else
                TempData["ErrorInfo"] = "无法修改！对象不存在或无效。";

            return View();
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomOperation roomOperation = db.RoomOperations.Find(id);
            if (roomOperation == null)
            {
                return HttpNotFound();
            }
            return View(roomOperation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DoDelete(Guid? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            RoomOperation roomOperation = db.RoomOperations.Find(Id);
            if (!roomOperation.Delete(ref db))
            {
                TempData["ErrorInfo"] = "无法删除";
                return View();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Remark(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoomRecord roomRecord = db.RoomRecords.Find(id);

            //这句话有问题，为什么找不到room呢。。。
            RoomOperation room = roomRecord.RoomOperation;
            if (roomRecord == null)
            {
                return HttpNotFound();
            }
            if (roomRecord.RemarkRate > 0)
            {

                return View(roomRecord);
            }
            if (room != null)
            {
                if (DateTime.Now > room.EndTime)
                {
                    if (roomRecord.Receiver != null)

                        return View(roomRecord);
                }
                TempData["ErrorInfo"] = "还未到允许评论的时间！";
            }
            else
            {
                TempData["ErrorInfo"] = "该课程没有成员！";
            }
            return RedirectToAction("StudentList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remark([Bind(Include = "Id,ActionTime,RemarkContent,RemarkRate,Time")] RoomRecord roomRecord)
        {
            if (ModelState.IsValid)
            {
                if (roomRecord.Remark())
                    return RedirectToAction("StudentList");
                TempData["ErrorInfo"] = "错误，你提交的评价不符合标准，请更改评分及评价内容！";
            }
            return View();
        }

        public ActionResult StudentList(Guid? Id)
        {
            var user = Extensions.GetContextUser(ref db);
            if (Id == null)
                return View(db.RoomRecords.Where(c => c.RoomOperation.Creator.Id == user.Id).ToList());
            RoomOperation room = db.RoomOperations.Find(Id);
            if (room == null)
            {
                TempData["ErrorInfo"] = "该课程不存在！";
                return RedirectToAction("Index");
            }
            if (room.Creator != user)
            {
                TempData["ErrorInfo"] = "你没有权限对该场地进行评价！";
                return View("Index");
            }
            IQueryable<RoomRecord> studentList = (from a in db.RoomRecords where a.RoomOperation.Id == room.Id select a).Distinct();
            if (studentList.FirstOrDefault() == null)
                return View(db.RoomRecords.ToList());
            return View(studentList);
        }

        #endregion
        //5 Views
        #region 上传文件模块
        // GET: Materials
        public ActionResult Materials(int page = 0)
        {
            IListPage test = new Material();
            var model = new ListPage<Material>(db.Materials, page, pageSize); //实现分页功能
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
                    ViewBag.Status = "请检查上传文件！";

                    return View();
                }

                var file = Request.Files[0];//只上传第一个文件

                var user = Models.User.Create(model.Email, model.DisplayName);
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //为账户添加角色
                    var roleName = "tutor";
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

                    var tutor = new TutorInformation { Id = Guid.NewGuid(), Tutor = db.Users.Find(user.Id), Avatar = Material.Create(user.DisplayName, MaterialType.Avatar, file, db), Position = model.Position, Introduction = model.Introduction };
                    db.TutorInformations.Add(tutor);
                    db.SaveChanges();
                    ViewBag.Status = "操作成功！";

                    return View();
                }
            }
            ViewBag.Status = "操作失败！";

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
        public ActionResult UserEdit(User model)
        {
            if (ModelState.IsValid)
            {
                db.Users.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
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
            return View("ProjectIdentityRecords", project);
        }

        public ActionResult CompanyIdentityRecords(int page = 0)
        {
            var company = new ListPage<Company>(db.Companys.Where(i => i.Status == CompanyStatus.ToApprove), page, pageSize);
            return View("CompanyIdentityRecords", company);

        }

        public ActionResult IdentityRecordDetails(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            IdentityRecord user = db.IdentityRecords.Find(id);
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View("IdentityRecordDetails", user);
        }

        public ActionResult ProjectIdentityRecordDetails(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Project project = db.Projects.Find(id);
            if (project == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            ViewData["ProgressList"] = EnumExtension.GetSelectList(typeof(ProjectProgressType));
            return View("ProjectIdentityRecordDetails", project);
        }

        public ActionResult CompanyIdentityRecordDetails(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Company company = db.Companys.Find(id);
            if (company == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View("CompanyIdentityRecordDetails", company);
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
                Team Team = new Team();
                Team.NewTeam(ref project);
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
            }
            else
            {
                user.Status = IdentityStatus.Denied;
                user.User.Identitied = false;
                db.Messages.Add(new Message(user.User.Id, MessageType.System, MessageTemplate.IdentityRecordFailure, ref db));
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