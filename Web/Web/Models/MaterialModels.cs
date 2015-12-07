using System;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    }

    public enum MaterialType
    {
        Download=0,
        Identity=1,
        Slider=2
    }
}