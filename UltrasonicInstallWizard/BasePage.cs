using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UltrasonicInstallWizard
{
    public class BasePage : UserControl
    {
        #region _ParentWnd
        private MainWindow _parentWin;
        public MainWindow ParentWindow
        {
            get { return _parentWin; }
            set { _parentWin = value; }
        }
        #endregion
    }
}
