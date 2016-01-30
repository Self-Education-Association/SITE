using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Web.Models
{
    public class AppSettings
    {
        UrlHelper url = new UrlHelper();
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

        public string DefaultConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }

        private T GetValue<T>(Func<string, T> parseFunc, Func<T> defaultTValueFunc,
    [CallerMemberName]string key = "", string supressKey = "")
        {
            if (!string.IsNullOrWhiteSpace(supressKey))
            {
                key = supressKey;
            }

            var node = ConfigurationManager.AppSettings[key];
            return !string.IsNullOrEmpty(node) ? parseFunc(node) : defaultTValueFunc();
        }

        private void SetValue<T>(T value, [CallerMemberName]string key = "", string supressKey = "")
        {
            if (!string.IsNullOrWhiteSpace(supressKey))
            {
                key = supressKey;
            }

            ConfigurationManager.AppSettings.Set(key, value.ToString());
            Refresh();
        }

        private void Refresh()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}