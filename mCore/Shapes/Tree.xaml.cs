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

namespace mCore.Shapes
{
    /// <summary>
    /// Interaction logic for Tree.xaml
    /// </summary>
    public partial class Tree : Canvas
    {
        public Tree(bool IsFruited)
        {
            if (Fruit != null)
            {
                if (IsFruited) Fruit.Visibility = Visibility.Visible;
                else Fruit.Visibility = Visibility.Hidden;
            }
            InitializeComponent();
        }

        public void ThunderStrike()
        {
            TreeShape.Fill = Brushes.Magenta;
            Trunk.Fill = Brushes.White;
        }
        public void HideFruit()
        {
            Fruit.Visibility = Visibility.Hidden;
        }
    }
}
