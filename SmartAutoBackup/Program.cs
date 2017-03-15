using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.ServiceProcess;

namespace SmartAutoBackup
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            if (Environment.UserInteractive )
            {
                //Logger.Init();
                //MonitorHistory.Init();
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new FLogin());


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                BonusSkins.Register();
                SkinManager.EnableFormSkins();
                UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CHS");
                Application.Run(new MainForm());

            }
            else
            {
                ServiceBase.Run(new SmartAutoBackupService());
            }

            //有下面这些主题可以选择（具体可以看dev的Demon里）
            //"|DevExpress Style|Caramel|Money Twins|DevExpress Dark Style|
            //iMaginary|Lilian|Black|Blue|Office 2010 Blue|Office 2010 Black|Office 2010 Silver|
            //Office 2007 Blue|Office 2007 Black|Officmetre 2007 Silver|Office 2007 Green|Office 2007 Pink|
            //Seven|Seven Classic|Darkroom|McSkin|Sharp|Sharp Plus|Foggy|Dark Side|Xmas (Blue)|
            //Springtime|Summer|Pumpkin|Valentine|Stardust|Coffee|Glass Oceans|High Contrast|Liquid Sky|London Liquid Sky|The Asphalt World|Blueprint|"
        }
    }
}
