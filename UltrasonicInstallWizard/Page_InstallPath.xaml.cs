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
using System.Windows.Forms;

namespace UltrasonicInstallWizard
{
    /// <summary>
    /// Interaction logic for Page_InstallPath.xaml
    /// </summary>
    public partial class Page_InstallPath : BasePage
    {
        public Page_InstallPath()
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
            ParentWindow.NavigationCall("Page_Progress"); //navigate
        }
        private void btnInstallPath(object sender, MouseButtonEventArgs e)
        {
            //文件选择
//             var openFileDialog = new Microsoft.Win32.OpenFileDialog()
//             {
//                 Filter = "Excel Files(*.sql)|*.sql"
//             };
//             var result = openFileDialog.ShowDialog();
//             if(result == true)
//             {
//                 this.m_InstallPath.Text = openFileDialog.FileName;
//             }

            //文件夹选择
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            this.m_InstallPath.Text = m_Dir;
        }
        private void btnDatasavePath(object sender,MouseButtonEventArgs e)
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            this.m_DatasavePath.Text = m_Dir;
        }
    }
}
