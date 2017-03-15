using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAutoBackup.Model
{
    public class CommandType
    {
        //应用程序定义的命令标志，指示要执行的自定义命令。此值必须介于 128 和 256 之间（均含）。
        public static int UpdateList
        {
            get { return 128; }
        }
        public static int Stop
        {
            get { return 129; }
        }
        public static int Start
        {
            get { return 130; }
        }
        public static int Pause
        {
            get { return 131; }
        }
        public static int Resume
        {
            get { return 132; }
        }
    }
}
