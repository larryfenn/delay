using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Delay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool dragStarted = false;
        private double delay = 4.7;
        private DelayTool dt;

        public MainWindow()
        {
            InitializeComponent();
            dt = new DelayTool(sourceList, outList);
        }

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            set(((Slider)sender).Value);
            this.dragStarted = false;
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragStarted = true;
        }

        private void Slider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
                set(e.NewValue);
        }

        private void set(double val)
        {
            delay = val;
            try {
                label.Content = Math.Round(100 * delay) + "ms";
            } catch
            {

            }
        }

        private void go_Click(object sender, RoutedEventArgs e)
        {
            dt.start(delay);
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            dt.stop();
        }
    }
}
