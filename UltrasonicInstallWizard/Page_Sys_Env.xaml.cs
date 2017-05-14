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
using System.IO;
using Microsoft.Win32;
using System.Configuration;
using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Threading;

namespace UltrasonicInstallWizard
{
    /// <summary>
    /// Interaction logic for Page_Sys_Env.xaml
    /// </summary>
    public partial class Page_Sys_Env:BasePage
    {
        private bool m_bAccessInstalled = false;
        private bool m_bVS2010Installed = false;
        private bool m_bVS2013Installed = false;

        private delegate void DelegateAccessProgress();
        private bool m_stopAccessFlag = false;

        private delegate void DelegateVs2010Progress();
        private bool m_stopVs2010Flag = false;

        private delegate void DelegateVs2013Progress();
        private bool m_stopVs2013Flag = false;

        public Page_Sys_Env()
        {
            InitializeComponent();

            GetSysEnvState();
        }

        private void btnMinimize_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            ParentWindow.WindowState = WindowState.Minimized;
        }
        private void btnClose_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            m_stopAccessFlag = true;
            m_stopVs2010Flag = true;
            m_stopVs2013Flag = true;

            ParentWindow.Close();
            //Application.Current.Shutdown();
            //Environment.Exit(0);
        }
        private void btnNextStep_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Next Step");

            if (m_bAccessInstalled && m_bVS2010Installed && m_bVS2013Installed)
            {
                CheckStartup();
                ParentWindow.NavigationCall("Page_Version"); //navigate
            }
        }

        private void GetSysEnvState()
        {
            RegistryKey machinelocalItem;
            RegistryKey softwareItem;
            RegistryKey microSoftItem;
            RegistryKey windowsItem;
            RegistryKey currentVersionItem;
            RegistryKey uninstallItem;

            machinelocalItem = Registry.LocalMachine;
            softwareItem = machinelocalItem.OpenSubKey("SOFTWARE");
            microSoftItem = softwareItem.OpenSubKey("Microsoft");
            windowsItem = microSoftItem.OpenSubKey("Windows");
            currentVersionItem = windowsItem.OpenSubKey("CurrentVersion");
            uninstallItem = currentVersionItem.OpenSubKey("Uninstall");

            RegistryKey access2010;
            access2010 = uninstallItem.OpenSubKey("{90140000-00D1-0804-1000-0000000FF1CE}");
            if (access2010 != null)
            {
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconright.png", UriKind.Relative));
                m_accessicon.Source = imgSource;

                m_accesstext.Text = "已安装 Microsoft Access Runtime 2010";
                m_bAccessInstalled = true;
            }
            else
            {
                RegistryKey access2013;
                access2013 = uninstallItem.OpenSubKey("{90150000-001C-0000-1000-0000000FF1CE}");
                if (access2013 != null)
                {
                    BitmapImage imgSource = new BitmapImage(new Uri("Images/iconright.png", UriKind.Relative));
                    m_accessicon.Source = imgSource;

                    m_accesstext.Text = "已安装 Microsoft Access Runtime 2013";
                    m_bAccessInstalled = true;
                }
                else
                {
                    InstallAccessEngine();
                }
            }

            RegistryKey vs2010Item;
            vs2010Item = uninstallItem.OpenSubKey("{DA5E371C-6333-3D8A-93A4-6FD5B20BCC6E}");
            if (vs2010Item != null)
            {
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconright.png", UriKind.Relative));
                m_vs2010icon.Source = imgSource;

                m_vs2010text.Text = "已安装 Microsoft Visual C++ 2010 x64 Runtime";
                m_bVS2010Installed = true;
            }
            else
            {
                InstallVs2010Runtime();
            }

            RegistryKey vs2013Item;
            vs2013Item = uninstallItem.OpenSubKey("{A749D8E6-B613-3BE3-8F5F-045C84EBA29B}");
            if (vs2013Item != null)
            {
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconright.png", UriKind.Relative));
                m_vs2013icon.Source = imgSource;

                m_vs2013text.Text = "已安装 Microsoft Visual C++ 2013 x64 Runtime";
                m_bVS2013Installed = true;
            }
            else
            {
                InstallVs2013Runtime();
            }
        }
        private void CheckStartup()
        {
            string sStartup = Environment.GetFolderPath(System.Environment.SpecialFolder.Startup);
            //Get All Startup Item  
            List<string> files = new List<string>(Directory.GetFiles(sStartup,"*.lnk"));
            foreach(var ink in files)
            {
                if(System.IO.File.Exists(ink))
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(ink);
                    string sDesPath = shortcut.TargetPath;
                    string sWorkingPath = shortcut.WorkingDirectory;

                    string sTail = sDesPath.Substring(sDesPath.Length-8,8);
                    if(sTail.Equals("demo.exe"))
                    {
                        var iPathLen = sDesPath.Length;
                        if(iPathLen>25)
                        {
                            string sHead = sDesPath.Substring(3, 13);
                            if (!sHead.Equals("Program Files"))
                            {
                                Directory.Delete(sWorkingPath, true);
                                System.IO.File.Delete(ink);
                            }
                        }
                        else
                        {
                            Directory.Delete(sWorkingPath, true);
                            System.IO.File.Delete(ink);
                        }
                    }
                }
            }
        }

        private void InstallAccessEngine()
        {
            try
            {
                string sCurPath = System.Environment.CurrentDirectory;
                sCurPath += "\\AccessDatabaseEngine_2010_X64.exe";
                System.Diagnostics.Process.Start(sCurPath, "/quiet");

                Process[] myProgress;
                myProgress = Process.GetProcessesByName("AccessDatabaseEngine_2010_X64");

                if(myProgress.Length > 0)
                {
                    Thread thread = new Thread(new ThreadStart(refreshAccessProgress));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void refreshAccessProgress()
        {
            while (!m_stopAccessFlag)
            {
                //向UI界面更新显示
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new DelegateAccessProgress(setCurAccessProgress));
            }
        }

        private void setCurAccessProgress()
        {
            Process[] myProgress;
            myProgress = Process.GetProcessesByName("AccessDatabaseEngine_2010_X64");

            if (myProgress.Length > 0)
            {
                System.DateTime currentTime = System.DateTime.Now;
                int iMilliSecond = currentTime.Millisecond;
                iMilliSecond /= 80;
                iMilliSecond %= 12;
                iMilliSecond += 1;

                string sImg = String.Format("Images/progress_{0}.png", iMilliSecond);
                BitmapImage imgSource = new BitmapImage(new Uri(sImg, UriKind.Relative));
                m_accessicon.Source = imgSource;

                m_accesstext.Text = "正在安装 Microsoft Access Runtime 2010";
            }
            else
            {
                m_stopAccessFlag = true;
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconright.png", UriKind.Relative));
                m_accessicon.Source = imgSource;

                m_accesstext.Text = "已安装 Microsoft Access Runtime 2010";
                m_bAccessInstalled = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void InstallVs2010Runtime()
        {
            try
            {
                string sCurPath = System.Environment.CurrentDirectory;
                sCurPath += "\\vcredist_2010_x64.exe";
                System.Diagnostics.Process.Start(sCurPath, "/q");

                Process[] myProgress;
                myProgress = Process.GetProcessesByName("vcredist_2010_x64");

                if (myProgress.Length > 0)
                {
                    Thread thread = new Thread(new ThreadStart(refreshVs2010Progress));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void refreshVs2010Progress()
        {
            while (!m_stopVs2010Flag)
            {
                //向UI界面更新显示
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new DelegateVs2010Progress(setCurVs2010Progress));
            }
        }

        private void setCurVs2010Progress()
        {
            Process[] myProgress;
            myProgress = Process.GetProcessesByName("vcredist_2010_x64");

            if (myProgress.Length > 0)
            {
                System.DateTime currentTime = System.DateTime.Now;
                int iMilliSecond = currentTime.Millisecond;
                iMilliSecond /= 80;
                iMilliSecond %= 12;
                iMilliSecond += 1;

                string sImg = String.Format("Images/progress_{0}.png", iMilliSecond);
                BitmapImage imgSource = new BitmapImage(new Uri(sImg, UriKind.Relative));
                m_vs2010icon.Source = imgSource;

                m_vs2010text.Text = "正在安装 Microsoft Visual C++ 2010 x64 Runtime";
            }
            else
            {
                m_stopVs2010Flag = true;
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconright.png", UriKind.Relative));
                m_vs2010icon.Source = imgSource;

                m_vs2010text.Text = "已安装 Microsoft Visual C++ 2010 x64 Runtime";
                m_bVS2010Installed = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void InstallVs2013Runtime()
        {
            try
            {
                string sCurPath = System.Environment.CurrentDirectory;
                sCurPath += "\\vcredist_2013_x64.exe";
                System.Diagnostics.Process.Start(sCurPath, "/quiet");

                Process[] myProgress;
                myProgress = Process.GetProcessesByName("vcredist_2013_x64");

                if (myProgress.Length > 0)
                {
                    Thread thread = new Thread(new ThreadStart(refreshVs2013Progress));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void refreshVs2013Progress()
        {
            while (!m_stopVs2013Flag)
            {
                //向UI界面更新显示
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new DelegateVs2013Progress(setCurVs2013Progress));
            }
        }

        private void setCurVs2013Progress()
        {
            Process[] myProgress;
            myProgress = Process.GetProcessesByName("vcredist_2013_x64");

            if (myProgress.Length > 0)
            {
                System.DateTime currentTime = System.DateTime.Now;
                int iMilliSecond = currentTime.Millisecond;
                iMilliSecond /= 80;
                iMilliSecond %= 12;
                iMilliSecond += 1;

                string sImg = String.Format("Images/progress_{0}.png", iMilliSecond);
                BitmapImage imgSource = new BitmapImage(new Uri(sImg, UriKind.Relative));
                m_vs2013icon.Source = imgSource;

                m_vs2013text.Text = "正在安装 Microsoft Visual C++ 2013 x64 Runtime";
            }
            else
            {
                m_stopVs2013Flag = true;
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconright.png", UriKind.Relative));
                m_vs2013icon.Source = imgSource;

                m_vs2013text.Text = "已安装 Microsoft Visual C++ 2013 x64 Runtime";
                m_bVS2013Installed = true;
            }
        }
        
    }//end class
}
