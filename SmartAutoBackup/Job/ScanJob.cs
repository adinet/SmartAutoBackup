using Quartz;
using SmartAutoBackup.Data;
using SmartAutoBackup.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAutoBackup.Job
{

    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ScanJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (GlobalVar.Pause)
            {
                return;
            }
            var timeStr = DateTime.Now.ToString("HH:mm");
            foreach (var item in GlobalVar.JobListCache)
            {
                if (item.LoopTime.ToString("HH:mm") == timeStr &&
                    (!item.LastBackupTime.HasValue || item.LastBackupTime.Value.Date < DateTime.Today))
                {
                    JobBackup.Backup(item);
                    item.LastBackupTime = DateTime.Now;
                    try
                    {
                        var serv = context.Scheduler.Context["service"] as JobService;
                        serv.Edit(item);
                    }
                    catch (Exception ex)
                    {
                        GlobalVar.Logger.Error(ex, $"任务[{item.Name}] 保存最后备份日期时发生错误. 原因:{ex.Message} | {ex.StackTrace}");
                    }
                }
            }
        }
    }
}
