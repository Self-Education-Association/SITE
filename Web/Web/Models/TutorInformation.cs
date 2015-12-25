using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TutorInformation
    {
        [Display(Name ="唯一编号")]
        public Guid Id { get; set; }

        public User Tutor { get; set; }

        public Material Avatar { get; set; }

        [Display(Name ="导师简介")]
        [DataType(DataType.MultilineText)]
        public string Introduction { get; set; }

        [Display(Name ="导师职位")]
        public string Position { get; set; }
    }
}