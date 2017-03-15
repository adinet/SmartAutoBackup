using SmartAutoBackup.Data;
using SmartAutoBackup.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SmartAutoBackup
{
    partial class SmartAutoBackupService : ServiceBase
    {
        private JobService serv;
        public SmartAutoBackupService()
        {
            serv = new JobService();
            GlobalVar.Init();
            GlobalVar.JobListCache = serv.FindAll();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            GlobalVar.Logger.Info("自动备份服务启动...");
            JobInit.CreateJob();
            // TODO: 在此处添加代码以启动服务。
        }

        protected override void OnStop()
        {
            GlobalVar.Logger.Info("自动备份服务停止...");
            if (JobInit.scheduler != null)
            {
                JobInit.scheduler.Shutdown();
            }
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
        }

        protected override void OnPause()
        {
            GlobalVar.Logger.Info("自动备份服务暂停...");
            if (JobInit.scheduler != null)
            {
                JobInit.scheduler.PauseAll();
            }
            //base.OnPause();
        }

        protected override void OnContinue()
        {
            GlobalVar.Logger.Info("自动备份服务继续...");
            if (JobInit.scheduler != null)
            {
                JobInit.scheduler.ResumeAll();
            }
            else
            {
                JobInit.CreateJob();
            }
            //base.OnContinue();
        }

        protected override void OnCustomCommand(int command)
        {
            GlobalVar.Logger.Info($"服务 接收到命令:{command}");
            if (command == Model.CommandType.UpdateList)
            {
                GlobalVar.Pause = true;
                GlobalVar.JobListCache = serv.FindAll();
                GlobalVar.Pause = false;
            }
            base.OnCustomCommand(command);
        }
    }
}
