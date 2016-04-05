using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Configuration;
using System.Web.Configuration;

namespace Web.Models
{
    public class AppSettings
    {
        public Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
        public bool Installed
        {
            get
            {
                return GetValue(bool.Parse, () => false);
            }
            set
            {
                SetValue(value);
            }
        }

        public DateTime ReportStartTime
        {
            get
            {
                return GetValue(DateTime.Parse, () => DateTime.MinValue);
            }
            set
            {
                SetValue(value);
            }
        }

        public DateTime ReportEndTime
        {
            get
            {
                return GetValue(DateTime.Parse, () => DateTime.MaxValue);
            }
            set
            {
                SetValue(value);
            }
        }

        public string ReportRoundName
        {
            get
            {
                return GetValue(x => x, () => string.Empty);
            }
            set
            {
                SetValue(value);
            }
        }

        public string DefaultConnection
        {
            get
            {
                return config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }

        private T GetValue<T>(Func<string, T> parseFunc, Func<T> defaultTValueFunc,
    [CallerMemberName]string key = "", string supressKey = "")
        {
            if (!string.IsNullOrWhiteSpace(supressKey))
            {
                key = supressKey;
            }

            var node = config.AppSettings.Settings[key].Value;
            return !string.IsNullOrEmpty(node) ? parseFunc(node) : defaultTValueFunc();
        }

        private void SetValue<T>(T value, [CallerMemberName]string key = "", string supressKey = "")
        {
            if (!string.IsNullOrWhiteSpace(supressKey))
            {
                key = supressKey;
            }

            config.AppSettings.Settings[key].Value = value.ToString();
            config.Save();
        }
    }
}