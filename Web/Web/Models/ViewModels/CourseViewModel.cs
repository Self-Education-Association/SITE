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
    class CourseViewModel
    {
            [Display(Name = "课程名")]
            public string Name { get; set; }
            [Display(Name = "教师")]
            public User Creator { get; set; }
            [Display(Name = "开始时间")]
            public DateTime StartTime { get; set; }
            [Display(Name = "结束时间")]
            public DateTime EndTime { get; set; }
            [Display(Name = "课程描述")]
            public string Content { get; set; }
            [Display(Name = "当前人数")]
            public int Count { get; set; }
            [Display(Name = "人数上限")]
            public int Limit { get; set; }
            [Display(Name = "课程类别")]
            public string Status { get; set; }
    }
    class CourseListViewModel
    {
        public List<CourseViewModel> CourseList { get; set; }
    }
    class CourseDetailsViewModel
    {
        [Display(Name = "课程名")]
        public string Name { get; set; }
        [Display(Name = "教师")]
        public User Creator { get; set; }
        [Display(Name = "开始时间")]
        public DateTime StartTime { get; set; }
        [Display(Name = "结束时间")]
        public DateTime EndTime { get; set; }
        [Display(Name = "课程描述")]
        public string Content { get; set; }
        [Display(Name = "当前人数")]
        public int Count { get; set; }
        [Display(Name = "人数上限")]
        public int Limit { get; set; }
        [Display(Name = "课程类别")]
        public string Status { get; set; }
        [Display(Name = "位置")]
        public string Location { get; set; }
    }
}

