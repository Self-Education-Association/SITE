using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models.ViewModels
{
    public class InstallIndexViewModel
    {
        [Required]
        [Remote("CheckConnectionString","Install",ErrorMessage ="连接字符串不可用")]
        public string ConnectionString { get; set; }
    }
}