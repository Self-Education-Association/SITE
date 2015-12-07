using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class Calendar
    {

        public List<ActivityOperation> CalendarActivity { get; set; }

        public int Date { get; set; }
    }

    public class HomeIndexViewModel
    {

        public List<ActivityOperation> LatestActivitys { get; set; }

        public List<Calendar> CalendarList { get; set; }

        public List<Material> Sliders { get; set; }
    }

}