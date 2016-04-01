using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TeamEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual Team Team { get; set; }

        public string EventName { get; set; }

        public string EventContent { get; set; }

        [Column(TypeName ="Date")]
        public DateTime EventTime { get; set; }

        public DateTime AddTime { get; set; } = DateTime.Now;
    }

    public class TeamReport
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual Team Team { get; set; }

        public virtual Material ReportFile { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;
    }
}