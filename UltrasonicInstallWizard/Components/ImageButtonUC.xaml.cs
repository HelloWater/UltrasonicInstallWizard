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

namespace UltrasonicInstallWizard
{
    /// <summary>
    /// Interaction logic for ImageButtonUC.xaml
    /// </summary>
    public partial class ImageButtonUC : UserControl
    {
        private Brush DOWN_BRUSH = new SolidColorBrush(Colors.Blue);
        private Brush UP_BRUSH = new SolidColorBrush(Colors.LightBlue);
        private Brush DISABLED_BRUSH = new SolidColorBrush(Colors.LightGray);

        public event MouseButtonEventHandler ClickEvent;

        public ImageSource MyImage
        {
            get { return m_ImageBrush.ImageSource; }
            set { m_ImageBrush.ImageSource = value; }
        }

        public ImageButtonUC()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsEnabled == true)
            {
                //m_Border.Background = DOWN_BRUSH;
               
                if (ClickEvent != null)
                {
                    ClickEvent(sender, e);
                }
            }
        }
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsEnabled == true)
            {
                //m_Border.Background = UP_BRUSH;
            }
        }
    }
}
