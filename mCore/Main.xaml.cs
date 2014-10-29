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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Xaml;
using ArcheBuddy.Bot.Classes;
using mCore.Radar;

namespace mCore
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : MetroWindow
    {
        RadarWindow radar = null;
        Core ArcheBuddyCore = null;
        public Main(Core core)
        {
            this.ArcheBuddyCore = core;
            InitializeComponent();
            
            this.RadarButton.Click += RadarButton_Click;
        }

        void RadarButton_Click(object sender, RoutedEventArgs e)
        {
            if (radar != null && radar.IsLoaded)
            {
                radar.Focus();
            }
            else {             
                radar = new RadarWindow(ArcheBuddyCore);
                radar.Top = this.Top;
                radar.Left = this.Left + this.ActualWidth;
                radar.Show();
                radar.Focus();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (radar != null) { 
                radar.Close();
                radar = null;
            }
            
            base.OnClosing(e);
            //if (radar != null && radar.IsLoaded)
            //{
            //   radar.Close();
            //}
        }
    }
}
