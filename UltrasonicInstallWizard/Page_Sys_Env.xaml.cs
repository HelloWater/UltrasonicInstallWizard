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
            ParentWindow.Close();
            //Application.Current.Shutdown();
            //Environment.Exit(0);
        }
        private void btnNextStep_ClickEvent(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Next Step");

            CheckStartup();
            ParentWindow.NavigationCall("Page_Version"); //navigate
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


            RegistryKey accessMUI;
            accessMUI = uninstallItem.OpenSubKey("{90150000-001C-0000-1000-0000000FF1CE}");
            if(accessMUI != null)
            {
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconright.png",UriKind.Relative));
                m_accessicon.Source = imgSource;

                m_accesstext.Text = "已安装 Microsoft Access Runtime 2013";
                m_bAccessInstalled = true;
            }
            else
            {
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconwarning.png", UriKind.Relative));
                m_accessicon.Source = imgSource;

                m_accesstext.Text = "未安装 Microsoft Access Runtime 2013";
                m_bAccessInstalled = false;
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
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconwarning.png", UriKind.Relative));
                m_vs2010icon.Source = imgSource;

                m_vs2010text.Text = "未安装 Microsoft Visual C++ 2010 x64 Runtime";
                m_bVS2010Installed = false;
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
                BitmapImage imgSource = new BitmapImage(new Uri("Images/iconwarning.png", UriKind.Relative));
                m_vs2013icon.Source = imgSource;

                m_vs2013text.Text = "未安装 Microsoft Visual C++ 2013 x64 Runtime";
                m_bVS2013Installed = false;
            }

            if(m_bAccessInstalled && m_bVS2010Installed && m_bVS2013Installed)
            {
                m_btnNextStep.IsEnabled = true;
            }
            else
            {
                m_btnNextStep.IsEnabled = false;
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
        
    }//end class
}
