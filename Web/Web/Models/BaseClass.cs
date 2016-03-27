using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class Remark : IListPage
    {
        [Display(Name = "唯一编号")]
        public Guid Id { get; set; }

        [Display(Name = "评分对象")]
        public virtual User Receiver { get; set; }

        [Display(Name = "活动时间")]
        public DateTime ActionTime { get; set; }

        [Display(Name = "评分内容")]
        public string RemarkContent { get; set; }

        [Display(Name = "评分分数")]
        public RemarkType RemarkRate { get; set; }

        [Display(Name = "评分时间")]
        public DateTime Time { get; set; }

        public Remark()
        {
            Id = Guid.NewGuid();
            ActionTime = DateTime.Now;
        }
    }

    public class Operation : IListPage
    {
        [Display(Name = "唯一编号")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "创建用户")]
        public User Creator { get; set; }

        [Display(Name = "创建时间")]
        public DateTime Time { get; set; }

        [Required]
        [Display(Name = "开始时间")]
        [DataType(DataType.DateTime)]
        [DayRange(0, 60)]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "结束时间")]
        [DataType(DataType.DateTime)]
        [DayRange(0, 60)]
        public DateTime EndTime { get; set; }

        [NotMapped]
        [Display(Name = "描述")]
        public string Content
        {
            get { return ContentStored; }
            set
            {
                ContentStored = value;
                //将content和shortcontent从模型上挂钩，并把null值的判定放在模型层，减少出错的可能
                if (value != null)
                {
                    if (Extensions.ReplaceHtmlTag(value).Length <= 50)
                    {
                        ShortContent = Extensions.ReplaceHtmlTag(value);
                    }
                    else
                        ShortContent = Extensions.ReplaceHtmlTag(value).Substring(0, 48) + "..";
                }
                else
                    ShortContent = null;
            }
        }

        [Display(Name = "描述")]
        public string ContentStored { get; set; }

        [NotMapped]
        [Display(Name = "摘要")]
        public string ShortContent
        {
            get { return ShortContentStored; }
            set { ShortContentStored = value; }
        }

        [Display(Name = "摘要")]
        [MaxLength(50)]
        public string ShortContentStored { get; set; }

        [Display(Name = "启用中")]
        public bool Enabled { get; set; }
    }

    public enum RemarkType
    {
        [EnumDisplayName("未评价")]
        None,
        [EnumDisplayName("非常糟糕")]
        PrettyBad,
        [EnumDisplayName("差评")]
        Bad,
        [EnumDisplayName("一般")]
        Normal,
        [EnumDisplayName("好评")]
        Good,
        [EnumDisplayName("非常良好")]
        PrettyGood
    }
}