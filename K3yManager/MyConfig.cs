using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace K3yManager
{
    class MyConfig
    {
        public MyConfig()
        {

        }

        public  string GetValue(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
                return "";
            else
                return config.AppSettings.Settings[key].Value;
        }

        public  void SetValue(string key, string value)
        {
          
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings"); 
        }

    }
}
