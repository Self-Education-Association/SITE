﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
namespace Web.ViewModel
{
    public class CourseListByStatusViewModel
    {
        public string status { get; set; }
        public List<CourseOperation> COS { get; set; }
    }
}