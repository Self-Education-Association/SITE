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

        public DateTime EventTime { get; set; }

        public DateTime AddTime { get; set; } = DateTime.Now;
    }

    public class TeamReport
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual Team Team { get; set; }

        public virtual Material ReportFile { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public string Round { get; set; }
    }

    public class TeamReportRound
    {
        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool Enabled
        {
            get
            {
                return (DateTime.Now >= StartTime && DateTime.Now <= EndTime);
            }
        }

        public TeamReportRound()
        {
            var appSettings = new AppSettings();
            Name = appSettings.ReportRoundName;
            StartTime = appSettings.ReportStartTime;
            EndTime = appSettings.ReportEndTime;
        }
    }
}