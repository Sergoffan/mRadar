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
    
    /// </summary>
    public partial class Galleon : Canvas, ITurnable
    {

        public static SolidColorBrush Friendly = new SolidColorBrush(Color.FromArgb(255,0,196,0));
        public static SolidColorBrush FriendlyFill = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));


        public static SolidColorBrush Enemy = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        public static SolidColorBrush EnemyFill = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        bool isfriendly;
        public Galleon(bool IsFriendly)
        {
            isfriendly = IsFriendly;
            InitializeComponent();

            if (!IsFriendly)
            {
                Flag.Stroke = EnemyFill;
            }
            else
            {
                Flag.Stroke = FriendlyFill;
            }
        }

        public void Turn(byte turnAngle)
        {
            double angleAdjust = turnAngle * -360 / 128 + 30;
            rotation.Angle = angleAdjust;
        }
    }
}

