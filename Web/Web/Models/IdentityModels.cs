using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class User : IdentityUser,IListPage
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }

        public string DisplayName { get; set; }//用户昵称

        public Profile Profile { get; set; }

        public virtual  IdentityRecord IdentityRecord { get; set; }//认证记录

        public virtual List<EducationRecord> Education { get; set; }//教育经历

        public virtual List<WorkRecord> Work { get; set; }//工作经历

        public virtual Project Project { get; set; }

        public virtual TeamRecord TeamRecord { get; set; }

        public virtual List<CourseRecord> Course { get; set; }

        public virtual List<RoomRecord> Room { get; set; }

        public virtual List<ActivityRecord> Activity { get; set; }

        public bool Identitied { get; set; } //是否通过实名认证

        public DateTime Time { get; set; }//账户创建时间

        public bool IsDisabled { get; set; } //是否被禁用
    }

    /// <summary>
    /// 认证记录
    /// </summary>
    public class IdentityRecord:IListPage
    {
        [Display(Name ="唯一编号")]
        public Guid Id { get; set; }

        [Display(Name = "身份证号或学号")]
        public string IdNumber { get; set; }//身份证号或学号

        [Display(Name = "用户")]
        public virtual User User { get; set; }//用户

        [Display(Name = "真实姓名")]
        public string Name { get; set; } //真实姓名

        [Display(Name = "身份证正面或学生证信息页")]
        public virtual Material FrontIdCard { get; set; }//身份证正面或学生证专业信息页

        [Display(Name = "身份证反面或学生证注册页")]
        public virtual Material BackIdCard { get; set; }//身份证反面或学生证注册报到页

        [Display(Name = "贸大校友")]
        public bool InUIBE { get; set; }//是否为校友

        [Display(Name = "在校学生")]
        public bool IsStudent { get; set; }//是否为在校生

        [Display(Name = "认证时间")]
        public DateTime Time { get; set; }//时间戳

        [Display(Name = "审批状态")]
        public IdentityStatus Status { get; set; }//认证状态

    }

    /// <summary>
    /// 教育经历类,每条记录代表一次教育经历
    /// </summary>
    public class EducationRecord
    {
        public Guid Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "入学时间")]
        public int StartYear { get; set; }//开始年份

        [DataType(DataType.Date)]
        [Display(Name = "毕业时间")]
        public int EndYear { get; set; }//结束年份

        [Display(Name = "学校名称")]
        public string School { get; set; }//学校名称

        [Display(Name = "学历类别")]
        public int DegreeType { get; set; }//学历类别
    }

    /// <summary>
    /// 工作经历类,每条记录代表一次工作经历
    /// </summary>
    public class WorkRecord
    {
        public Guid Id { get; set; }

        [Display(Name ="入职时间")]
        public int StartYear { get; set; }//开始年份

        [Display(Name = "离职时间")]
        public int EndYear { get; set; }//结束年份

        [Display(Name = "公司名称")]
        public string Company { get; set; }//公司名称
    }

    [ComplexType]
    public class Profile
    {
        [Display(Name ="电子邮箱（默认为账户名，与认证邮箱不同）")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "手机号码")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "其他联系方式（请注明）")]
        public string Other { get; set; }

        [Display(Name = "对我的联系方式保密")]
        public bool InformationPrivacy { get; set; }

        [Display(Name = "允许在用户列表中搜索到我")]
        public bool Searchable { get; set; }
    }

    public enum IdentityStatus
    {
        [EnumDisplayName("无记录")]
        None,
        [EnumDisplayName("未通过")]
        Denied,
        [EnumDisplayName("待审批")]
        ToApprove,
        [EnumDisplayName("已通过")]
        Done
    }

    public class BaseDbContext : IdentityDbContext<User>
    {
        public BaseDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<BaseDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOptional(u => u.Project).WithRequired(p => p.Admin);
            //modelBuilder.Entity<User>().HasOptional(u => u.TeamRecord).WithRequired(t => t.Receiver);
            modelBuilder.Entity<TeamRecord>().HasRequired(t => t.Receiver).WithOptional(u => u.TeamRecord);
            modelBuilder.Entity<User>().HasOptional(u => u.IdentityRecord).WithRequired(i => i.User);
            modelBuilder.Entity<User>().HasOptional(u => u.Project).WithRequired(p => p.Admin);
            modelBuilder.Entity<Team>().HasOptional(t => t.Company);
            base.OnModelCreating(modelBuilder);
        }

        public static BaseDbContext Create()
        {
            return new BaseDbContext();
        }

        public virtual DbSet<EducationRecord> EducationRecords { get; set; }

        public virtual DbSet<WorkRecord> WorkRecords { get; set; }

        public virtual DbSet<ActivityOperation> ActivityOperations { get; set; }

        public virtual DbSet<ActivityRecord> ActivityRecords { get; set; }

        public virtual DbSet<CourseOperation> CourseOperations { get; set; }

        public virtual DbSet<CourseRecord> CourseRecords { get; set; }

        public virtual DbSet<RoomOperation> RoomOperations { get; set; }

        public virtual DbSet<RoomRecord> RoomRecords { get; set; }

        public virtual DbSet<Material> Materials { get; set; }

        public virtual DbSet<Article> Articles { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Team> Teams { get; set; }

        public virtual DbSet<TeamRecord> TeamRecords { get; set; }

        public virtual DbSet<Company> Companys { get; set; }

        public virtual DbSet<IdentityRecord> IdentityRecords { get; set; }

        public System.Data.Entity.DbSet<Web.Models.TeamProfileViewModel> TeamProfileViewModels { get; set; }
    }
}