using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Management;
using System.IO;
using Microsoft.Win32;
using System.Configuration;

namespace UltrasonicInstallWizard
{
    /// <summary>
    /// Interaction logic for Page_Progress.xaml
    /// </summary>
    public partial class Page_Progress : BasePage
    {
        private delegate void DelegateSetProgress();
        private bool m_stopFlag = false;

        public Page_Progress()
        {
            InitializeComponent();

            if (!m_stopFlag)
            {
                Thread thread = new Thread(new ThreadStart(refreshProgress));
                thread.Start();
            }
        }

        private void btnMinimize_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            ParentWindow.WindowState = WindowState.Minimized;
        }
        private void btnClose_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            m_stopFlag = true;
            ParentWindow.Close();
            //Application.Current.Shutdown();
            //Environment.Exit(0);
        }
        private void btnNextStep_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Next Step");
            //ParentWindow.NavigationCall("Page_Sys_Env"); //navigate

            string sCurPath = "";
            string sDesDisk = "";
            sCurPath = System.Environment.CurrentDirectory;
            sDesDisk = CheckDisk();
            sDesDisk += "\\Data";
            SetRegistKey(sCurPath, sDesDisk);

            var ini = new IniFile(sCurPath + "\\SystemConfig.ini");
            ini.IniWritevalue("SYSTEM","Preset",sDesDisk+"\\PRESET\\");
            ini.IniWritevalue("SYSTEM","PatientDB",sDesDisk+"\\Patient.mdb");
            ini.IniWritevalue("SYSTEM","MediaSavePath",sDesDisk+"\\Media");

            ParentWindow.Close();
        }

        private void refreshProgress()
        {
            while(!m_stopFlag)
            {
                //向UI界面更新显示
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new DelegateSetProgress(setCurProgress));
            }
        }
        private void setCurProgress()
        {
            string sCurPath = "";
            string sDesDisk = "";

//             if(ParentWindow.IfUpdate)
//             {
//                 
//             }
//             else //Install
//             {
//                 
//             }

            sCurPath = System.Environment.CurrentDirectory;

            sDesDisk = CheckDisk();
            sDesDisk += "\\Data";
            if (Directory.Exists(sCurPath + "\\Data"))
            {
                MoveFolder(sCurPath + "\\Data", sDesDisk, false);
                Directory.Delete(sCurPath + "\\Data", true);
            } 

            stateIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Images/iconright.png"));
            
            m_stopFlag = true;
            
            //string currentTime = System.DateTime.Now.ToString();
        }
        private string CheckDisk()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_LogicalDisk");
                string sDisc = ""; //盘符
                string sSize = "";
                
                foreach (ManagementObject mo in searcher.Get())
                {
                    string disc = mo["Name"].ToString().Trim();
                    string size = mo["Size"].ToString().Trim();
                    string DriveType = mo["DriveType"].ToString().Trim();
      
                    if(string.Compare(DriveType,"3") == 0)
                    {
                        if (string.Compare(size, sSize) > 0)
                        {
                            sDisc = disc;
                            sSize = size;
                        }
                    }
                }
                return sDisc;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// MoveFolder
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="a_bIfReplace">是否替换</param>
        public static void MoveFolder(string sourcePath, string destPath, bool a_bIfReplace)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    //目标目录不存在则创建  
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }
                //获得源文件下所有文件  
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(c));
                    //如果目标文件已存在
                    if (File.Exists(destFile))
                    {
                        //替换
                        if(a_bIfReplace)
                        {
                            File.Delete(destFile);
                            File.Move(c, destFile);
                        }
                        else
                        {
                            //不替换
                        }
                        
                        
                    }
                    else //如果目标文件不存在
                    {
                        File.Move(c, destFile);
                    }
                });
                //获得源文件下所有目录文件  
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));

                folders.ForEach(c =>
                {
                    string destDir = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(c));
                    //Directory.Move必须要在同一个根目录下移动才有效，不能在不同卷中移动。  
                    //Directory.Move(c, destDir);  

                    //采用递归的方法实现  
                    MoveFolder(c, destDir,a_bIfReplace);
                });
            }
            else
            {
                throw new DirectoryNotFoundException("源目录不存在！");
            }
        } 
        private void SetRegistKey(string programfile, string datapath)
        {
            RegistryKey machinelocalItem;
            RegistryKey softwareItem;
            machinelocalItem = Registry.LocalMachine;
            softwareItem = machinelocalItem.OpenSubKey("SOFTWARE",true);

            RegistryKey shawayItem = softwareItem.OpenSubKey("Shaway",true);
            if (shawayItem == null)
            {
                shawayItem = softwareItem.CreateSubKey("Shaway"); 
            }
            string sNewVersion = ConfigurationManager.AppSettings["NewVersion"];

            shawayItem.SetValue("ProgramFile", programfile);
            shawayItem.SetValue("DataPath", datapath);
            shawayItem.SetValue("Version", sNewVersion);

            AddRegedit(programfile, datapath, sNewVersion);
        }
        /// <summary>
        /// 注册应用程序
        /// </summary>
        /// <param name="setupPath"></param>
        private void AddRegedit(string setupPath, string datapath, string version)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall",true);
                RegistryKey software = key.CreateSubKey("Shaway");
                // 图标
                software.SetValue("DisplayIcon", setupPath + "\\UltrasonicInstallWizard.exe");

                // 显示名
                software.SetValue("DisplayName", "Ultrasound");

                // 版本
                software.SetValue("DisplayVersion", version);

                // 程序发行公司
                software.SetValue("Publisher", "shaway");

                // 安装位置
                software.SetValue("InstallLocation", setupPath);

                // 安装源
                software.SetValue("InstallSource", setupPath);

                // 帮助电话
                // software.SetValue("HelpTelephone", "123456789");

                // 卸载路径
                software.SetValue("UninstallString", datapath + "\\uninstallUS.exe");
                software.Close();
                key.Close();
            }
            catch(Exception)
            {

            }
        }
    }
}
