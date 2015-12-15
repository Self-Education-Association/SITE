using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Web.Models
{
    public class Article : IListPage
    {
        [Display(Name ="唯一编号")]
        public Guid ID { get; set; }

        [Display(Name = "文章标题")]
        public string Title { get; set; }

        [Display(Name = "文章内容")]
        public string Content { get; set; }

        [Display(Name = "文章状态")]
        public ArticleStatus Status { get; set; }

        [Display(Name = "文章类别")]
        public ArticleClass Class { get; set; }

        [Display(Name = "创建时间")]
        public DateTime Time { get; set; }

        public void NewArticle()
        {
            ID = Guid.NewGuid();
            Time = DateTime.Now;
            Status = ArticleStatus.Enabled;
        }
    }

    public class Message
    {
        [Display(Name = "唯一编号")]
        public Guid ID { get; set; }

        [Display(Name = "发送方")]
        public User Publisher { get; set; }

        [Display(Name = "接收方")]
        public User Receiver { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "消息内容")]
        public string Content { get; set; }

        [Display(Name = "消息类别")]
        public MessageType Type { get; set; }

        [Display(Name = "阅读状态")]
        public bool HaveRead { get; set; }

        [Display(Name = "创建时间")]
        public DateTime Time { get; set; }

        public Message(string title, string content, string userId, MessageType type, BaseDbContext db)
        {
            ID = Guid.NewGuid();
            Publisher = Extensions.GetContextUser(db);
            Receiver = db.Users.Find(userId);
            Title = title;
            Content = content;
            Type = type;
            HaveRead = false;
            Time = DateTime.Now;
        }

        public Message(string userId, MessageType type, MessageTemplate template, BaseDbContext db)
        {
            ID = Guid.NewGuid();
            Publisher = Extensions.GetContextUser(db);
            Receiver = db.Users.Find(userId);
            Type = type;
            HaveRead = false;
            Time = DateTime.Now;
            switch (template)
            {
                case MessageTemplate.TeamFailure:
                    Title = "";
                    Content = "";
                    break;
                default:
                    Title = "";
                    Content = "";
                    break;
            }
        }

        public Message(string userId, MessageType type, MessageTemplate template, string personal, BaseDbContext db)
        {
            ID = Guid.NewGuid();
            Publisher = Extensions.GetContextUser(db);
            Receiver = db.Users.Find(userId);
            Type = type;
            HaveRead = false;
            Time = DateTime.Now;
            switch (template)
            {
                case MessageTemplate.TeamFailure:
                    Title = "项目申请被驳回";
                    Content = "很遗憾，你的项目申请被管理员驳回，请返回项目申请页面查看，并对照管理员批复予以修改。";
                    break;
                case MessageTemplate.TeamApply:
                    Title = "新成员申请加入团队";
                    Content = "有新成员申请加入你的团队，请及时处理。";
                    break;
                case MessageTemplate.TeamApplySuccess:
                    Title = "您已成功加入团队";
                    Content = "您申请的团队已通过您的申请！请在我的团队页面查看。";
                    break;
                case MessageTemplate.TeamRecruit:
                    Title = "您已成功创建团队招募";
                    Content = "您申请的团队招募已通过！请等待他人的加入申请。";
                    break;
                case MessageTemplate.TeamApplyFail:
                    Title = "您未能加入团队";
                    Content = "您申请的团队拒绝了你的申请，详情请向团队管理员咨询。";
                    break;
                default:
                    Title = "";
                    Content = "";
                    break;
            }
        }

        public bool Publish()
        {
            try
            {
                using (BaseDbContext db = new BaseDbContext())
                {
                    db.Messages.Add(this);
                    db.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public enum MessageType
    {
        [EnumDisplayName("系统消息")]
        System,
        [EnumDisplayName("团队消息")]
        Team,
        [EnumDisplayName("私聊消息")]
        Person
    }

    public enum MessageTemplate
    {
        TeamFailure,
        TeamApply,
        TeamRecruit,
        TeamApplySuccess,
        TeamApplyFail,
        ProjectFailure,
        ProjectSuccess,
        CompanyFailure,
        CompanySuccess,
    }

    public enum ArticleStatus
    {
        [EnumDisplayName("启用")]
        Enabled,
        [EnumDisplayName("禁用")]
        Disabled
    }

    public enum ArticleClass
    {
        //随意加了用的，删除即可
        a,
        b
    }
}
