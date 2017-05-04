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
using System.Reflection;

namespace UltrasonicInstallWizard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.ResizeMode = ResizeMode.NoResize;

            Navigate("PageWelcome");
        }

        private void Navigate(string path)
        {
            string uri = "UltrasonicInstallWizard." + path;
            Type type = Type.GetType(uri);
            if (type != null)
            {
                object obj = type.Assembly.CreateInstance(uri);
                UserControl control = obj as UserControl;
                this.MainFrame.Content = control;
                PropertyInfo[] infos = type.GetProperties();
                foreach (PropertyInfo info in infos)
                {
                    if (info.Name == "ParentWindow")
                    {
                        info.SetValue(control, this, null);
                        break;
                    }
                }
            }
        }

        public void NavigationCall(string pageName)
        {
            //MessageBox.Show(pageName);
            Navigate(pageName);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
