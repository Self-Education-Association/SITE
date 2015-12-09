using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Project
    {
        public Guid Id { get; set; }

        public virtual User Admin { get; set; }

        [Display(Name = "项目名称")]
        public string Name { get; set; }

        [Display(Name = "目标行业")]
        public string Industry { get; set; }

        [Display(Name = "项目介绍")]
        public string Introduction { get; set; }

        [Display(Name = "预计产品")]
        public string Product { get; set; }

        [Display(Name = "产品特点")]
        public string Feature { get; set; }

        [Display(Name = "技术特点")]
        public string Tech { get; set; }

        [Display(Name = "项目进程")]
        public ProjectProgressType Progress { get; set; }

        [Display(Name = "专利所有")]
        public string Patent { get; set; }

        [Display(Name = "希望获得的支持")]
        public string Desire { get; set; }

        [Display(Name = "目标人群")]
        public string TargetCustomer { get; set; }

        [Display(Name = "项目预算")]
        public string ProjectBudget { get; set; }

        [Display(Name = "项目网页")]
        public string Webpage { get; set; }

        [Display(Name = "项目信息是否公开")]
        public bool Privacy { get; set; }

        [Display(Name = "管理员批复")]
        public string Note { get; set; }

        public DateTime Time { get; set; }

        public ProjectStatus Status { get; set; }

        public virtual Team Team { get; set; }

        public void NewProject(BaseDbContext db)
        {
            Id = Guid.NewGuid();
            Admin = db.Users.Find(Extensions.GetCurrentUser().Id);
            Time = DateTime.Now;
            Status = ProjectStatus.ToApprove;
        }
    }

    public class Team:IListPage
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public User Admin { get; set; }

        public DateTime Time { get; set; }

        public string Introduction { get; set; }

        public string Announcement { get; set; }

        public bool Searchable { get; set; }

        public virtual Company Company { get; set; }
        
        public virtual IQueryable<TeamRecord> Member { get; set; }

        public void NewTeam(ref Project project)
        {
            Id = Guid.NewGuid();
            Name = project.Name;
            Admin = project.Admin;
            Time = DateTime.Now;
            Introduction = "此处的信息将作为团队的对外介绍。";
            Announcement = "此处的信息将作为团队的内部公告";
            project.Team = this;
        }
    }

    public class TeamRecord : Remark
    {
        public virtual Team Team { get; set; }

        public TeamMemberStatus Status { get; set; }

        public TeamRecord(Team team):base()
        {
            Team = team;
            Status = TeamMemberStatus.Normal;
        }

        public TeamRecord(Team team,TeamMemberStatus status) : base()
        {
            Team = team;
            Status = status;
        }

        public TeamRecord(Team team, TeamMemberStatus status,User user) : base()
        {
            Team = team;
            Status = status;
            Receiver = user;
        }
    }

    public class Company:IListPage
    {
        [Display(Name ="公司申请编号")]
        public Guid Id { get; set; }

        [Display(Name = "公司创始人")]
        public User Admin { get; set; }

        [Display(Name = "公司名称")]
        public string Name { get; set; }

        [Display(Name = "资金来源")]
        public string SourcesOfFunds { get; set; }

        [Display(Name = "公司法人代表")]
        public string Corporation { get; set; }

        [Display(Name = "注册网站")]
        public string RegisterSite { get; set; }

        [Display(Name = "营业执照编号")]
        public string CodeOfBusinessLicense { get; set; }

        [Display(Name = "公司人数")]
        public int MembersCount { get; set; }

        [Display(Name = "注册资本")]
        public int AmountOfMoney { get; set; }

        [Display(Name = "近期营业额")]
        public int RecentTurnover { get; set; }

        [Display(Name = "公司资产")]
        public int CompanyValuation { get; set; }

        [Display(Name = "融资金额")]
        public int FinancingAmount { get; set; }

        [Display(Name = "已售出股份")]
        public double SharesSold { get; set; }

        [Display(Name = "融资时机")]
        public string FinancingTime { get; set; }

        [Display(Name = "投资计划")]
        public string Investment { get; set; }

        [Display(Name = "管理员批复")]
        public string Note { get; set; }

        [Display(Name = "申请时间")]
        public DateTime Time { get; set; }

        [Display(Name = "申请状态")]
        public CompanyStatus Status { get; set; }

        public void NewCompany(ref BaseDbContext db)
        {
            Id = Guid.NewGuid();
            Admin = db.Users.Find(Extensions.GetCurrentUser().Id);
            Time = DateTime.Now;
            Status = CompanyStatus.ToApprove;
        }
    }

    public enum ProjectStatus
    {
        None,
        Denied,
        ToApprove,
        Done
    }

    public enum CompanyStatus
    {
        None,
        Denied,
        ToApprove,
        Done
    }

    public enum TeamMemberStatus:int
    {
        Normal=0,
        Admin=1,
        Apply=2,
        Recruit=3
    }

    public enum ProjectProgressType
    {
        None=0,
        HaveTeam=1,
        Done=2,
        HaveCompany=3
    }
}