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

        #region 文章管理模块
        public ActionResult Articles(int page = 0)
        {
            return View(new ListPage<Article>(db.Articles, page, pageSize));
        }

        public ActionResult ArticleCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ArticleCreate(Article model)
        {
            if (ModelState.IsValid)
            {
                model.NewArticle();
                db.Articles.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }

            return View();
        }

        public ActionResult ArticleEdit(Guid? Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Article model = db.Articles.Find(Id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ArticleEdit(Article model)
        {
            if (ModelState.IsValid)
            {
                db.Articles.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { status = AdminOperationStatus.Success });
            }

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
        public ActionResult ActivityCreate(ActivityOperation model)
        {
            if (ModelState.IsValid)
            {
                if (model.StartTime > model.EndTime)
                {
                    ViewData["ErrorInfo"] = "活动开始时间必须在结束时间之前。";
                    return View();
                }
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
        public ActionResult ActivityEdit(ActivityOperation model)
        {
            if (ModelState.IsValid)
            {
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MaterialCreate([Bind(Include = "Name,Description,Type")] Material material)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count != 1)//如果文件列表为空则返回
                    return View();
                var file = Request.Files[0];//只上传第一个文件
                //根据日期生成服务器端文件名
                string uploadFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName);
                //生成服务器端绝对路径
                string absolutFileName = Server.MapPath("~/") + "UserUpload/Administrator/" + uploadFileName;
                //执行上传
                file.SaveAs(absolutFileName);
                //添加Material记录
                db.Materials.Add(new Material(uploadFileName, material.Description, MaterialType.Download));
                //保存更改
                db.SaveChanges();
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

        #region 用户管理模块
        public ActionResult TutorCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> TutorCreate(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email, DisplayName = model.DisplayName, Time = DateTime.Now, IsDisabled = false, Profile = new Profile { Email = model.Email, Phone = "", Searchable = true, InformationPrivacy = false, Other = "" } };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    //为账户添加角色
                    var roleName = "tutor";
                    ApplicationRoleManager roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(new BaseDbContext()));

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

                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public ActionResult UserEdit(string Id)
        {
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            User model = db.Users.Find(Id);
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
            }
            return View();
        }
        #endregion

        #region 审核认证记录模块
        public ActionResult IdentityRecords(int page = 0)
        {
            var model = new ListPage<IdentityRecord>(db.IdentityRecords.Where(i => i.Status == IdentityStatus.ToApprove), page, pageSize);

            return View(model);
        }

        public ActionResult IdentityRecordDetails(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            IdentityRecord model = db.IdentityRecords.Find(id);
            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(model);
        }

        public ActionResult IdentityRecordApprove(Guid? id, bool isApprove)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            IdentityRecord model = db.IdentityRecords.Find(id);
            if (model == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            if (model.Status != IdentityStatus.ToApprove)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (isApprove)
            {
                model.Status = IdentityStatus.Done;
                model.User.Identitied = true;
            }
            else
            {
                model.Status = IdentityStatus.Denied;
                model.User.Identitied = false;
            }
            db.SaveChanges();

            return RedirectToAction("IdentityRecords");
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