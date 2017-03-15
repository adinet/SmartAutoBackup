using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace SmartAutoBackup.Model
{
    public static class GlobalVar
    {
        public static string ServiceName
        {
            get
            {
                return "SmartAutoBackupService";
            }
        }

        public static Logger Logger
        {
            get
            {
                return NLog.LogManager.GetCurrentClassLogger();
            }
        }
        private static string _basePath = string.Empty;
        public static string BasePath
        {
            get
            {
                if (string.IsNullOrEmpty(_basePath))
                {
                    var path = Assembly.GetExecutingAssembly().Location;
                    _basePath = Path.GetDirectoryName(path);

                }
                return _basePath;
            }
        }

        public static void Init()
        {
            //Config = 
            var json = File.ReadAllText(Path.Combine(BasePath, "Config.json"));
            Config = JsonConvert.DeserializeObject<ConfigModel>(json);
        }

        public static bool Pause { get; set; }

        public static List<JobModel> JobListCache { get; set; }

        public static ConfigModel Config { get; set; }
    }
}
