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
using mCore;
using mCore.Models;
using System.Reflection;

namespace mCore.Radar
{
    /// <summary>
    /// Interaction logic for RadarTabSettings.xaml
    /// </summary>
    public partial class RadarTabSettings : UserControl
    {
        public RadarTabSettings()
        {
            InitializeComponent();
            this.ShowAllButton.Click += ShowAllButton_Click;
            this.HideAllButton.Click += HideAllButton_Click;
        }

        void HideAllButton_Click(object sender, RoutedEventArgs e)
        {
            RadarTab tab = (RadarTab)this.DataContext;
            foreach (PropertyInfo boolP in tab.GetType().GetProperties().Where(prop => prop.Name.StartsWith("Show")))
            {
                boolP.SetValue(tab, false);
            }
            

        }

        void ShowAllButton_Click(object sender, RoutedEventArgs e)
        {
            RadarTab tab = (RadarTab)this.DataContext;
            foreach (PropertyInfo boolP in tab.GetType().GetProperties().Where(prop => prop.Name.StartsWith("Show")))
            {
                boolP.SetValue(tab, true);
                
            }
        }
    }
}
