using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using SmartAutoBackup.Data;
using SmartAutoBackup.Job;

namespace SmartAutoBackup
{
    public partial class AddDBJob_Form : DevExpress.XtraEditors.XtraForm
    {
        private MainForm mainForm;

        private JobModel model;
        private long? Id { get; set; }
        private JobService jobServ;
        public AddDBJob_Form()
        {
            InitializeComponent();
            jobServ = new JobService();
        }

        public AddDBJob_Form(MainForm mainForm,long? id)
        {
            this.mainForm = mainForm;
            this.Id = id;
            jobServ = new JobService();
            InitializeComponent();
        }

        private void AddDBJob_Form_Load(object sender, EventArgs e)
        {
            if (Id.HasValue)
            {
                model = jobServ.GetById(Id.Value);
                txtJobName.Text = model.Name;
                txtUserName.Text = model.UserName;
                txtPassword.Text = model.Password;
                txtJobServer.Text = model.Server;
                txtPort.Text = model.FtpPort.ToString();
                chkIsCompression.Checked = model.IsCompression;
                combDBList.Text = model.DBName;
                timeWhen.Time = model.LoopTime;
                chkUpload.Checked = model.IsUpload;
                txtFtpHost.Text = model.FtpHost;
                txtFtpUser.Text = model.FtpUserName;
                txtFtpPassword.Text = model.FtpPassword;
                txtFtpPath.Text = model.FtpPath;
            }
            else
            {
                model = new JobModel();
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            model.Name = txtJobName.Text;
            model.UserName = txtUserName.Text;
            model.Password = txtPassword.Text;
            model.FtpPort = Convert.ToInt32(txtPort.Text);
            model.Server = txtJobServer.Text;
            model.DBName =combDBList.Text;
            model.IsCompression = chkIsCompression.Checked;
            model.LoopTime = timeWhen.Time;
            model.IsUpload = chkUpload.Checked;
            model.FtpHost = txtFtpHost.Text;
            model.FtpUserName = txtFtpUser.Text;
            model.FtpPassword = txtFtpPassword.Text;
            model.FtpPath = txtFtpPath.Text;
            if (Id.HasValue)
            {
                if (jobServ.GetCount($"where [name]='{model.Name}' and Id <> {Id.Value}") != 0)
                {
                    XtraMessageBox.Show("任务名称重复,请更换!", "错误");
                    return;
                }
                jobServ.Edit(model);
            }
            else
            {
                if (jobServ.GetByColName("name", model.Name) != null)
                {
                    XtraMessageBox.Show("任务名称重复,请更换!","错误");
                    return;
                }
                jobServ.Add(model);
            }
            mainForm.LoadList();
            XtraMessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void AddDBJob_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            model = null;
            jobServ = null;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (JobBackup.TestConnection(txtJobServer.Text, txtUserName.Text, txtPassword.Text))
            {
                var list = JobBackup.GetAllDataBase(txtJobServer.Text, txtUserName.Text, txtPassword.Text);
                combDBList.Properties.Items.Clear();
                foreach (var item in list)
                {
                    combDBList.Properties.Items.Add(item);
                }
                //combDBList.Text
                XtraMessageBox.Show("数据库连接成功!", "提示");
            }
            else
            {
                XtraMessageBox.Show("数据库连接失败...","提示");
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                int port = Convert.ToInt32(txtPort.Text);
                if (JobBackup.TestFtpConnection(txtFtpHost.Text, port, txtFtpUser.Text, txtFtpPassword.Text))
                {
                    XtraMessageBox.Show("FTP连接成功!", "提示");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"FTP连接失败\r\n原因:{ex.Message}", "提示");
            }
        }

    }
}