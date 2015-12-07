using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.ViewModel
{
    public class CourseListByCreatorViewModel
    {
        public User CourseCreator { get; set; }
        public List<CourseOperation> CO { get; set; }
    }

}