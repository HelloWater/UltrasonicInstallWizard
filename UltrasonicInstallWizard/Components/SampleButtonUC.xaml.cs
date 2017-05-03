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

namespace UltrasonicInstallWizard.Components
{
    /// <summary>
    /// Interaction logic for SampleButtonUC.xaml
    /// </summary>
    public partial class SampleButtonUC : UserControl
    {
        private Brush DOWN_BRUSH = new SolidColorBrush(Colors.Blue);
        private Brush SELECT_BRUSH = new SolidColorBrush(Colors.LightBlue);
        private Brush UNSELECT_BRUSH = new SolidColorBrush(Colors.White);
        private Brush DISABLED_BRUSH = new SolidColorBrush(Colors.LightGray);

        public event MouseButtonEventHandler ClickEvent;

        public string Text
        {
            get { return m_TextBlock.Text; }
            set { m_TextBlock.Text = value; }
        }
        public double TextFontSize
        {
            get { return m_TextBlock.FontSize; }
            set { m_TextBlock.FontSize = value; }
        }
        public CornerRadius CornerRadius
        {
            get { return m_Border.CornerRadius; }
            set { m_Border.CornerRadius = value; }
        }

        public SampleButtonUC()
        {
            InitializeComponent();
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(this.IsEnabled == true)
            {
                m_Border.BorderBrush = UNSELECT_BRUSH;
                m_TextBlock.Foreground = UNSELECT_BRUSH;
            }
            else
            {
                m_Border.BorderBrush = DISABLED_BRUSH;
                m_TextBlock.Foreground = DISABLED_BRUSH;
            }
        }
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if(this.IsEnabled == true)
            {
                m_Border.BorderBrush = SELECT_BRUSH;
                m_TextBlock.Foreground = SELECT_BRUSH;
            }
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if(this.IsEnabled == true)
            {
                m_Border.BorderBrush = UNSELECT_BRUSH;
                m_TextBlock.Foreground = UNSELECT_BRUSH;
            }
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.IsEnabled == true)
            {
                m_Border.BorderBrush = DOWN_BRUSH;
                m_TextBlock.Foreground = DOWN_BRUSH;
                if (ClickEvent != null)
                {
                    ClickEvent(sender, e);
                }
            }
        }
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(this.IsEnabled == true)
            {
                m_Border.BorderBrush = UNSELECT_BRUSH;
                m_TextBlock.Foreground = UNSELECT_BRUSH;
            }
        }

    }
}
