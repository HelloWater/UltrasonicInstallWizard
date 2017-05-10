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

            if(ParentWindow.IfUpdate)
            {
                
            }
            else //Install
            {
                sCurPath = System.Environment.CurrentDirectory;

                sDesDisk = CheckDisk();
                sDesDisk += "\\Data";
                if (Directory.Exists(sCurPath + "\\Data"))
                {
                    MoveFolder(sCurPath + "\\Data", sDesDisk);
                    Directory.Delete(sCurPath + "\\Data", true);
                }
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
                    if (string.Compare(size, sSize) > 0)
                    {
                        sDisc = disc;
                        sSize = size;
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
        public static void MoveFolder(string sourcePath, string destPath)
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
                    //覆盖模式  
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }
                    File.Move(c, destFile);
                });
                //获得源文件下所有目录文件  
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));

                folders.ForEach(c =>
                {
                    string destDir = System.IO.Path.Combine(destPath, System.IO.Path.GetFileName(c));
                    //Directory.Move必须要在同一个根目录下移动才有效，不能在不同卷中移动。  
                    //Directory.Move(c, destDir);  

                    //采用递归的方法实现  
                    MoveFolder(c, destDir);
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

            RegistryKey shawayItem = softwareItem.OpenSubKey("Shaway");
            if (shawayItem == null)
            {
                shawayItem = softwareItem.CreateSubKey("Shaway");
            }
           
            string sNewVersion = ConfigurationManager.AppSettings["NewVersion"];

            shawayItem.SetValue("ProgramFile", programfile);
            shawayItem.SetValue("DataPath", datapath);
            shawayItem.SetValue("Version",sNewVersion);
        }
    }
}
