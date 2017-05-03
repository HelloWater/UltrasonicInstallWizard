using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UltrasonicInstallWizard
{
    /// <summary>
    /// Interaction logic for Page_Sys_Env.xaml
    /// </summary>
    public partial class Page_Sys_Env:BasePage
    {
        public Page_Sys_Env()
        {
            InitializeComponent();
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
            ParentWindow.NavigationCall("Page_Version"); //navigate
        }
    }
}
