using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TutorInformation
    {
        [Display(Name = "唯一编号")]
        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual User Tutor { get; set; }

        public virtual Material Avatar { get; set; }

        [Display(Name = "导师简介")]
        [DataType(DataType.MultilineText)]
        public string Introduction { get; set; }

        [Display(Name = "导师职位")]
        public string Position { get; set; }
    }

    public class TutorHelper
    {
        public void CreateTutorInformation(User tutor, ITutor information)
        {
            Message = "";
            Succeed = false;
            using (var db = new BaseDbContext())
            {
                var contextUser = db.Users.Find(tutor.Id);
                if (contextUser == null)
                {
                    Message = "未能从数据库中找到用户";
                    return;
                }
                var info = new TutorInformation {
                    Tutor = contextUser,
                    Introduction = information.TutorIntroduction,
                    Position = information.TutorPosition,
                    Avatar = information.TutorAvatar };
                db.TutorInformations.Add(info);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    Message = "出现错误";
                    return;
                }

                Succeed = true;
                return;
            }
        }

        public string Message { get; private set; } = "";

        public bool Succeed { get; private set; } = false;
    }

    public interface ITutor
    {
        string TutorName { get; set; }

        Material TutorAvatar { get; set; }

        string TutorIntroduction { get; set; }

        string TutorPosition { get; set; }
    }
}