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
    class RoomViewModel
    {
            [Display(Name = "场地名称")]
            public string Name { get; set; }
            [Display(Name = "开始时间")]
            public DateTime StartTime { get; set; }
            [Display(Name = "结束时间")]
            public DateTime EndTime { get; set; }
            [Display(Name ="是否可用")]
            public bool Usable { get; set; }
    }
    class RoomListViewModel
    {
        public List<RoomViewModel> RoomList { get; set; }
    }
    class RoomDetailsViewModel
    {
        [Display(Name = "场地名称")]
        public string Name { get; set; }
        [Display(Name = "管理员")]
        public User Creator { get; set; }
        [Display(Name = "开始时间")]
        public DateTime StartTime { get; set; }
        [Display(Name = "结束时间")]
        public DateTime EndTime { get; set; }
        [Display(Name = "是否可用")]
        public int Usable { get; set; }
        [Display(Name = "场地描述")]
        public string Content { get; set; }
    }
}

