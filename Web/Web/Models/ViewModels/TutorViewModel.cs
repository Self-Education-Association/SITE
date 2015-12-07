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
    class CourseOperateViewModel
    {
        [Required]
        [Display(Name = "课程名")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "开始时间")]
        public DateTime StartTime { get; set; }
        [Required]
        [Display(Name = "结束时间")]
        public DateTime EndTime { get; set; }
        [Required]
        [Display(Name = "课程描述")]
        public string Content { get; set; }
        [Required]
        [Display(Name = "人数上限")]
        public int Limit { get; set; }
        [Required]
        [Display(Name = "课程类别")]
        public string Status { get; set; }
        [Required]
        [Display(Name = "位置")]
        public string Location { get; set; }
    }
    class RemarkViewModel
    {
        [Display(Name ="学生名")]
        public User Receiver { get; set; }
        [Required]
        [Display(Name = "评价内容")]
        public string RecordContent { get; set; }
        [Required]
        [Display(Name = "评分")]
        public int RecordRate { get; set; }
    }
}

