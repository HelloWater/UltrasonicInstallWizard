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
using Microsoft.Win32;
using System.Configuration;

namespace UltrasonicInstallWizard
{
    /// <summary>
    /// Interaction logic for Page_Version.xaml
    /// </summary>
    public partial class Page_Version : BasePage
    {
        public Page_Version()
        {
            InitializeComponent();

            ReadVersionInfo();
        }

        private void ReadVersionInfo()
        {
            RegistryKey machinelocalItem;
            RegistryKey softwareItem;
            machinelocalItem = Registry.LocalMachine;
            softwareItem = machinelocalItem.OpenSubKey("SOFTWARE");

            RegistryKey shawayItem = softwareItem.OpenSubKey("Shaway");
            if (shawayItem != null)
            {
                object version = shawayItem.GetValue("Version");
                textOldVersion.Text = "当前版本"+version.ToString();
                iconOldVersion.Source = new BitmapImage(new Uri("pack://application:,,,/Images/iconwarning.png"));
            }

            string sNewVersion = ConfigurationManager.AppSettings["NewVersion"];
            textNewVersion.Text = "最新版本"+sNewVersion;
            
            string sUpdateInfo_1 = ConfigurationManager.AppSettings["UpdateInfo_1"];
            if(sUpdateInfo_1.Length > 0)
            {
                textUpdate1.Text = sUpdateInfo_1;
                iconUpdate1.Source = new BitmapImage(new Uri("pack://application:,,,/Images/iconinfo.png"));
                //iconUpdate1.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/Res/test.png"));
            }

            string sUpdateInfo_2 = ConfigurationManager.AppSettings["UpdateInfo_2"];
            if (sUpdateInfo_2.Length > 0)
            {
                textUpdate2.Text = sUpdateInfo_2;
                iconUpdate2.Source = new BitmapImage(new Uri("pack://application:,,,/Images/iconinfo.png"));
            }

            string sUpdateInfo_3 = ConfigurationManager.AppSettings["UpdateInfo_3"];
            if (sUpdateInfo_3.Length > 0)
            {
                textUpdate3.Text = sUpdateInfo_3;
                iconUpdate3.Source = new BitmapImage(new Uri("pack://application:,,,/Images/iconinfo.png"));
            }

            string sUpdateInfo_4 = ConfigurationManager.AppSettings["UpdateInfo_4"];
            if (sUpdateInfo_4.Length > 0)
            {
                textUpdate4.Text = sUpdateInfo_4;
                iconUpdate4.Source = new BitmapImage(new Uri("pack://application:,,,/Images/iconinfo.png"));
            }

            string sUpdateInfo_5 = ConfigurationManager.AppSettings["UpdateInfo_5"];
            if (sUpdateInfo_5.Length > 0)
            {
                textUpdate5.Text = sUpdateInfo_5;
                iconUpdate5.Source = new BitmapImage(new Uri("pack://application:,,,/Images/iconinfo.png"));
            }
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
            VerifyVersion();
            ParentWindow.NavigationCall("Page_Progress"); //navigate
        }
        private void VerifyVersion()
        {
            RegistryKey machinelocalItem;
            RegistryKey softwareItem;
            machinelocalItem = Registry.LocalMachine;
            softwareItem = machinelocalItem.OpenSubKey("SOFTWARE");

            RegistryKey shawayItem = softwareItem.OpenSubKey("Shaway");
            if (shawayItem != null)
            {
                ParentWindow.IfUpdate = true;
            }
            else
            {
                ParentWindow.IfUpdate = false;
            }
        }
    }
}
