namespace SmartAutoBackup
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this._serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this._serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // _serviceProcessInstaller
            // 
            this._serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this._serviceProcessInstaller.Password = null;
            this._serviceProcessInstaller.Username = null;
            // 
            // _serviceInstaller
            // 
            this._serviceInstaller.Description = "数据库自动备份服务";
            this._serviceInstaller.DisplayName = "SmartAutoBackupService";
            this._serviceInstaller.ServiceName = "SmartAutoBackupService";
            this._serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this._serviceProcessInstaller,
            this._serviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller _serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller _serviceInstaller;
    }
}