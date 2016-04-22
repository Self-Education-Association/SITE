using System;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.IO;

namespace Web.Models
{
    public class Material : IListPage
    {
        public Guid Id { get; set; }

        [Display(Name = "文件名")]
        public string Name { get; set; }

        [Display(Name = "文件描述")]
        public string Description { get; set; }

        [Display(Name = "创建时间")]
        public DateTime Time { get; set; }

        [Display(Name = "用途分类")]
        public MaterialType Type { get; set; }

        private Material(string name, string description, MaterialType type)
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
                case MaterialType.Identity:
                    return "~/UserUpload/Identity/" + Name;
                case MaterialType.Avatar:
                    return "~/UserUpload/Avatar/" + Name;
                case MaterialType.Management:
                    return "~/UserUpload/Management/" + Name;
                default:
                    return "~/UserUpload/Administrator/" + Name; ;
            }
        }

        public string GetPath()
        {
            switch (Type)
            {
                case MaterialType.Identity:
                    return HttpContext.Current.Server.MapPath("~/") + "UserUpload/Identity/" + Name;
                case MaterialType.Avatar:
                    return HttpContext.Current.Server.MapPath("~/") + "UserUpload/Avatar/" + Name;
                case MaterialType.Management:
                    return HttpContext.Current.Server.MapPath("~/") + "UserUpload/Management/" + Name;
                default:
                    return HttpContext.Current.Server.MapPath("~/") + "UserUpload/Administrator/" + Name;
            }
        }

        public static Material Create(string description, MaterialType type, HttpPostedFileBase file, BaseDbContext db)
        {
            if (!type.Match(file))
                return null;
            string uploadFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            string absolutFileName;
            switch (type)
            {
                case MaterialType.Identity:
                    absolutFileName = HttpContext.Current.Server.MapPath("~/UserUpload/") + "Identity/" + uploadFileName;
                    break;
                case MaterialType.Avatar:
                    absolutFileName = HttpContext.Current.Server.MapPath("~/UserUpload/") + "Avatar/" + uploadFileName;
                    break;
                case MaterialType.Management:
                    absolutFileName = HttpContext.Current.Server.MapPath("~/UserUpload/") + "Management/" + uploadFileName;
                    break;
                default:
                    absolutFileName = HttpContext.Current.Server.MapPath("~/UserUpload/") + "Administrator/" + uploadFileName;
                    break;
            }

            if (!Directory.Exists(Path.GetDirectoryName(absolutFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(absolutFileName));
            }
            //执行上传
            if (File.Exists(absolutFileName))
            {
                File.Delete(absolutFileName);
            }
            file.SaveAs(absolutFileName);
            Material material = new Material(uploadFileName, description, type);
            //添加Material记录
            db.Materials.Add(material);
            //保存更改
            db.SaveChanges();
            return db.Materials.Find(material.Id);
        }

        public static Material ChangeFile(Guid id, HttpPostedFileBase file, BaseDbContext db)
        {
            Material material = db.Materials.Find(id);
            if (!material.Type.Match(file))
                return null;
            string uploadFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            string absolutFileName;
            switch (material.Type)
            {
                case MaterialType.Identity:
                    absolutFileName = HttpContext.Current.Server.MapPath("~/UserUpload/") + "Identity/" + uploadFileName;
                    break;
                case MaterialType.Avatar:
                    absolutFileName = HttpContext.Current.Server.MapPath("~/UserUpload/") + "Avatar/" + uploadFileName;
                    break;
                case MaterialType.Management:
                    absolutFileName = HttpContext.Current.Server.MapPath("~/UserUpload/") + "Management/" + uploadFileName;
                    break;
                default:
                    absolutFileName = HttpContext.Current.Server.MapPath("~/UserUpload/") + "Administrator/" + uploadFileName;
                    break;
            }

            if (!Directory.Exists(Path.GetDirectoryName(absolutFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(absolutFileName));
            }
            //执行上传
            file.SaveAs(absolutFileName);
            material.Name = uploadFileName;

            //保存更改
            db.Materials.Attach(material);
            db.Entry(material).State = EntityState.Modified;
            db.SaveChanges();
            return db.Materials.Find(material.Id);
        }


    }

    public enum MaterialType
    {
        [EnumDisplayName("模板文件")]
        Download,
        [EnumDisplayName("活动资料")]
        Activity,
        [EnumDisplayName("认证图片")]
        Identity,
        [EnumDisplayName("海报图片")]
        Slider,
        [EnumDisplayName("头像图片")]
        Avatar,
        [EnumDisplayName("管理用文件")]
        Management
    }

    public enum DefaultMaterial
    {
        News,
        Avatar,
        Points
    }
}