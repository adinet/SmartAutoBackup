using DevExpress.XtraEditors;
using SmartAutoBackup.Data;
using SmartAutoBackup.Job;
using SmartAutoBackup.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace SmartAutoBackup
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        JobService serv = new JobService();
        public MainForm()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            GlobalVar.Init();
            //JobInit.CreateJob();
            //GlobalVar.JobListCache = serv.FindAll();
            LoadList();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            AddDBJob_Form addForm = new AddDBJob_Form(this, null);
            addForm.ShowDialog();
        }

        public void LoadList()
        {
            var list = serv.FindAll();
            gridJobList.DataSource = list;
        }

        private void gridJobList_DoubleClick(object sender, EventArgs e)
        {
            if (gridJobDetailView.SelectedRowsCount != 0)
            {
                var handle = gridJobDetailView.FocusedRowHandle;
                var row = (JobModel)gridJobDetailView.GetRow(handle);

                AddDBJob_Form addForm = new AddDBJob_Form(this, row.Id);
                addForm.ShowDialog();
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (gridJobDetailView.SelectedRowsCount != 0)
            {
                if (MessageBox.Show("确定要删除该任务吗?\r\n 删除后无法恢复哟!", "警告!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var handle = gridJobDetailView.FocusedRowHandle;
                    var row = (JobModel)gridJobDetailView.GetRow(handle);
                    serv.Delete(row.Id);
                    gridJobDetailView.DeleteRow(handle);
                }
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            ServiceController serviceController = new ServiceController(GlobalVar.ServiceName);
            try
            {
                serviceController.ExecuteCommand(Model.CommandType.UpdateList);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"向服务发送命令失败\r\n{ex.Message}");
            }
            LoadList();
        }

        private void simpleButton5_Click_1(object sender, EventArgs e)
        {
            //if (gridJobDetailView.SelectedRowsCount != 0)
            //{
            //    var handle = gridJobDetailView.FocusedRowHandle;
            //    var row = (JobModel)gridJobDetailView.GetRow(handle);
            //    JobBackup.Backup(row);
            //}
        }

        private void installService_Click(object sender, EventArgs e)
        {
            InstallService(GlobalVar.ServiceName);
        }

        private void uninstallService_Click(object sender, EventArgs e)
        {

        }

        private void InstallService(string serviceName)
        {
            string[] args = new string[]
            {
                "SmartAutoBackup.exe"
            };
            ServiceController serviceController = new ServiceController(serviceName);
            if (!this.ServiceIsExisted(serviceName))
            {
                try
                {
                    ManagedInstallerClass.InstallHelper(args);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
            else
            {
                XtraMessageBox.Show("该服务已经存在，不用重复安装。");
            }
        }

        private bool ServiceIsExisted(string svcName)
        {
            ServiceController[] services = ServiceController.GetServices();
            ServiceController[] array = services;
            bool result;
            for (int i = 0; i < array.Length; i++)
            {
                ServiceController serviceController = array[i];
                if (serviceController.ServiceName == svcName)
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }

        public void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController serviceController = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds((double)timeoutMilliseconds);
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                GlobalVar.Logger.Error(ex, $"服务停止失败:{ex.Message}");
            }
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds((double)timeoutMilliseconds);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                GlobalVar.Logger.Error(ex, $"服务启动失败:{ex.Message}");
                XtraMessageBox.Show($"重启失败，请手工重启SmartAutoBackupService服务。\r\n{ex.Message}" );
            }
        }

        public void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController serviceController = new ServiceController(serviceName);

            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds((double)timeoutMilliseconds);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                GlobalVar.Logger.Error(ex, $"服务启动失败:{ex.Message}");
                XtraMessageBox.Show($"重启失败，请手工重启{GlobalVar.ServiceName}服务。\r\n{ex.Message}");
            }
        }

        public void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController serviceController = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds((double)timeoutMilliseconds);
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                GlobalVar.Logger.Error(ex, $"服务停止失败:{ex.Message}");
            }
        }

        private void btnStartServ_Click(object sender, EventArgs e)
        {
            StartService(GlobalVar.ServiceName, 4000);
        }

        private void btnStopServ_Click(object sender, EventArgs e)
        {
            StopService(GlobalVar.ServiceName, 4000);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //RestartService(GlobalVar.ServiceName, 4000);
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            RestartService(GlobalVar.ServiceName, 3000);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                using (ServiceController _serviceController = new ServiceController(GlobalVar.ServiceName))
                {
                    barServState.Caption = "服务状态：" + _serviceController.Status.ToString();
                }
            }
            catch (Exception ex) { }
        }
    }

}
