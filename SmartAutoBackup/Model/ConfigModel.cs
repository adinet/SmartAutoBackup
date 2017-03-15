using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAutoBackup.Model
{
    public class ConfigModel
    {
        public string SavePath { get; set; }
        public int Interval { get; set; }
    }
}
