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

            m_stopFlag = !m_stopFlag;
            if(!m_stopFlag)
            {
                Thread thread = new Thread(new ThreadStart(refreshProgress));
                thread.Start();
            }
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
            string currentTime = System.DateTime.Now.ToString();
            m_sProgress.Text = currentTime;
        }
    }
}
