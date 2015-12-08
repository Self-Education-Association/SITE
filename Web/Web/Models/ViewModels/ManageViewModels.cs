using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;

namespace Web.Models
{
    public class ManageIndexViewModel
    {
        public bool HasIdentited { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class UserProfileViewModel:Profile
    {

    }

    public class UserIdentityViewModel:IdentityRecord
    {
        
    }

    public class ProjectViewModel:ProjectInformation
    {
        [Display(Name = "提交时间")]
        public DateTime Time { get; set; }
    }

    public class TeamProfileViewModel
    {
        [Display(Name="团队编号")]
        public Guid Id { get; set; }

        [Display(Name="团队名称")]
        public string Name { get; set; }

        [Display(Name = "创始人")]
        public string Administrator { get; set; }

        [Display(Name = "创立时间")]
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }

        [Display(Name = "团队简介")]
        [StringLength(100)]
        public string Introduction { get; set; }

        [Display(Name = "团队公告")]
        [StringLength(100)]
        public string Announcement { get; set; }

        [Display(Name = "公开团队招募")]
        public bool Searchable { get; set; }
    }

    public class CompanyViewModel:CompanyInformation
    {
    }
}