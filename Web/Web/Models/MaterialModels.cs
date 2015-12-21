using System;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Web.Models
{
    public class Material:IListPage
    {
        public Guid Id { get; set; }

        [Display(Name = "文件名")]
        public string Name { get; set; }

        [Display(Name="文件描述")]
        public string Description { get; set; }

        [Display(Name = "创建时间")]
        public DateTime Time { get; set; }

        [Display(Name="用途分类")]
        public MaterialType Type { get; set; }

        public Material(string name,string description,MaterialType type)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Time = DateTime.Now;
            Type = type;
        }

        public Material()
        {

        }

        public string GetUrl()
        {
            switch (Type)
            {
                case MaterialType.Download:
                case MaterialType.Slider:
                    return "~/UserUpload/Administrator/" + Name;
                case MaterialType.Identity:
                    return "~/UserUpload/Identity/" + Name;
                case MaterialType.Avatar:
                    return "~/UserUpload/Avatar/" + Name;
                default:
                    return "";
            }
        }

        public string GetPath()
        {
            switch (Type)
            {
                case MaterialType.Download:
                case MaterialType.Slider:
                    return HttpContext.Current.Server.MapPath("~/") + "UserUpload/Administrator/" + Name;
                case MaterialType.Identity:
                    return HttpContext.Current.Server.MapPath("~/") + "UserUpload/Identity/" + Name;
                default:
                    return "";
            }
        }
    }

    public enum MaterialType
    {
        [EnumDisplayName("下载文件")]
        Download,
        [EnumDisplayName("认证图片")]
        Identity,
        [EnumDisplayName("海报图片")]
        Slider,
        [EnumDisplayName("头像")]
        Avatar
    }
}