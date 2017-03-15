using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAutoBackup
{
    public class JobModel
    {
        [DataField]
        public long Id { get; set; }
        [DataField]
        public string Name { get; set; }
        [DataField]
        public string Server { get; set; }
        [DataField]
        public string UserName { get; set; }
        [DataField]
        public string Password { get; set; }
        [DataField]
        public string DBName { get; set; }
        [DataField]
        public bool IsCompression { get; set; }
        [DataField]
        public string LoopType { get; set; }
        [DataField]
        public DateTime LoopTime { get; set; }
        [DataField]
        public int State { get; set; }

        [DataField]
        public bool IsUpload { get; set; }
        [DataField]
        public string FtpHost { get; set; }
        [DataField]
        public int FtpPort { get; set; }
        [DataField]
        public string FtpUserName { get; set; }
        [DataField]
        public string FtpPassword { get; set; }
        [DataField]
        public string FtpPath { get; set; }

        [DataField]
        public DateTime CreateDate { get; set; }
        [DataField]
        public DateTime? LastBackupTime { get; set; }

        public string LoopStr
        {
            get
            {
                return string.Concat(LoopType, " / ", LoopTime.ToString("HH:mm"));
            }
        }

        public string FtpInfo
        {
            get
            {
                return string.Concat(FtpUserName, "@", FtpHost, ":", FtpPort, FtpPath);
            }
        }

        public string DBNameAndComp
        {
            get
            {
                return string.Concat(DBName, "@", IsCompression ? "压缩" : "不压缩");
            }
        }
    }
}
