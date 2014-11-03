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
    public partial class PlayerShape : Canvas, IStealthable, ITurnable, IKillable
    {
        public static SolidColorBrush FriendlyStealthed = new SolidColorBrush(Color.FromArgb(96, 0, 255, 255));
        public static SolidColorBrush FriendlyStealthedFill = new SolidColorBrush(Color.FromArgb(96, 0, 255, 255));

        public static SolidColorBrush Friendly = new SolidColorBrush(Color.FromArgb(255,0,196,0));
        public static SolidColorBrush FriendlyFill = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        public static SolidColorBrush EnemyStealthed = new SolidColorBrush(Color.FromArgb(196, 208, 44, 168));
        public static SolidColorBrush EnemyStealthedFill = new SolidColorBrush(Color.FromArgb(64, 255, 0, 228));

        public static SolidColorBrush Enemy = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        public static SolidColorBrush EnemyFill = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        bool isfriendly;
        public PlayerShape(bool IsFriendly)
        {
            isfriendly = IsFriendly;
            InitializeComponent();

            if (!IsFriendly)
            {
                rect.Stroke = Enemy;
                rect.Fill = EnemyFill;
            }

            DeathMark.Visibility = Visibility.Hidden;
        }

        public void Kill(bool isAlive)
        {
            if (isAlive)
            {
                DeathMark.Visibility = Visibility.Hidden;
                rect.Visibility = Visibility.Visible;
            }
            else
            {
                DeathMark.Visibility = Visibility.Visible;
                rect.Visibility = Visibility.Hidden;
            }
        }

        public void Stealth(bool stealthed)
        {
            if (isfriendly)
            {
                rect.Fill = stealthed ? FriendlyStealthedFill : FriendlyFill;
                rect.Stroke = stealthed ? FriendlyStealthed : Friendly;
            }
            else
            {
                rect.Fill = stealthed ? EnemyStealthedFill : EnemyFill;
                rect.Stroke = stealthed ? EnemyStealthed : Enemy;
            }
        }

        public void Turn(byte turnAngle)
        {
            double angleAdjust = turnAngle * -360 / 128 + 50;
            ((RotateTransform)rect.RenderTransform).Angle = angleAdjust;
        }

        public void ShowTradePack(bool hasTradePack)
        {
            if (hasTradePack) tradePack.Visibility = Visibility.Visible;
            else tradePack.Visibility = Visibility.Hidden;
        }
    }
}

