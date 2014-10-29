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
using ArcheBuddy;
using mCore;

namespace mDebug
{
    /// <summary>d
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Window main;

        public MainWindow()
        {
            //Load the main window from mCore project
            //main = new mCore.Main(null);
            main = new mCore.Radar.RadarWindow(null);
            main.Show();

            //close the window for this project because it's empty
            Close();
        }

    }
}
