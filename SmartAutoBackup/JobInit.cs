using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using SmartAutoBackup.Job;
using SmartAutoBackup.Model;
using SmartAutoBackup.Data;

namespace SmartAutoBackup
{
    public class JobInit
    {
        public static IScheduler scheduler;
        public static IJobDetail scanJob;
        #region 创建计划任务

        public static void CreateJob()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scanJob = JobBuilder.Create<ScanJob>().WithIdentity("ScanJob").Build();

            scheduler.Context["service"] = new JobService();

            var scanJobBuilder = TriggerBuilder.Create();

            var scanJobTrigger = scanJobBuilder.WithSimpleSchedule(action =>
            {
                action.WithIntervalInSeconds(GlobalVar.Config.Interval).RepeatForever();
            }).Build();


            scheduler.ScheduleJob(scanJob, scanJobTrigger);
            scheduler.Start();

        }
        #endregion
    }
}
