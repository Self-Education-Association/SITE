using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Web.Models
{
    public class ActivityViewModel
    {
        [Display(Name = "活动名")]
        public string Name { get; set; }
        [Display(Name = "发起人")]
        public User Creator { get; set; }
        [DayRange(3,30)]
        [Display(Name = "开始时间")]
        public DateTime StartTime { get; set; }
        [DayRange(3,30)]
        [Display(Name = "结束时间")]
        public DateTime EndTime { get; set; }
        [Display(Name = "活动描述")]
        public string Content { get; set; }
        [Display(Name = "当前人数")]
        public int Count { get; set; }
        [Display(Name = "人数上限")]
        public int Limit { get; set; }
    }
    class ActivityListViewModel
    {
        public List<ActivityViewModel> ActivityList { get; set; }
    }
    class ActivityDetailsViewModel
    {
        [Display(Name = "活动名")]
        public string Name { get; set; }
        [Display(Name = "发起人")]
        public User Creator { get; set; }
        [Display(Name = "开始时间")]
        public DateTime StartTime { get; set; }
        [Display(Name = "结束时间")]
        public DateTime EndTime { get; set; }
        [Display(Name = "活动描述")]
        public string Content { get; set; }
        [Display(Name = "当前人数")]
        public int Count { get; set; }
        [Display(Name = "人数上限")]
        public int Limit { get; set; }
    }
}

