using System;
using System.Data.Entity;
using System.Linq;

namespace Web.Models
{
    public class Article
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public ArticleStatus Status { get; set; }

        public DateTime Time { get; set; }
    }

    public class Message
    {
        public Guid ID { get; set; }

        public User Publisher { get; set; }

        public User Receiver { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public MessageType Type { get; set; }

        public bool HaveRead { get; set; }

        public DateTime Time { get; set; }

        public Message(string title,string content,User user,MessageType type)
        {
            ID = Guid.NewGuid();
            Publisher = Extensions.GetCurrentUser();
            Receiver = user;
            Title = title;
            Content = content;
            Type = type;
            HaveRead = false;
            Time = DateTime.Now;
        }

        public Message(User user,MessageType type,MessageTemplate template)
        {
            ID = Guid.NewGuid();
            Publisher = Extensions.GetCurrentUser();
            Receiver = user;
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

        public Message(User user, MessageType type, MessageTemplate template,string personal)
        {
            ID = Guid.NewGuid();
            Publisher = Extensions.GetCurrentUser();
            Receiver = user;
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
        System,
        Team,
        Person
    }

    public enum MessageTemplate
    {
        TeamFailure,
        TeamSuccess
    }

    public enum ArticleStatus
    {
        Enabled,
        Disabled
    }

}
