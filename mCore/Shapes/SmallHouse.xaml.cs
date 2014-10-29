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
    public partial class SmallHouse : Canvas, ITimedObject
    {
        public static SolidColorBrush GreenFill = new SolidColorBrush(Color.FromArgb(38,38,128,38));
        public static SolidColorBrush GreenStroke = new SolidColorBrush(Color.FromArgb(128,0,128,0));
        public static SolidColorBrush RedFill = new SolidColorBrush(Color.FromArgb(64, 255, 0, 0));
        public static SolidColorBrush RedStroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        public static SolidColorBrush GrayFill = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
        public static SolidColorBrush GrayStroke = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));    

        public SmallHouse()
        {
            InitializeComponent();
        }

        public void UpdateTime(byte weeksWithoutPay, ulong timeleft)
        {
            UpdateTime(rect, Data, weeksWithoutPay, timeleft);
        }
        public static void UpdateTime(Shape rect, TextBlock data, byte weeksWithoutPay, ulong timeleft)
        {
            if (weeksWithoutPay > 0) { 
                data.Text = CalculateTimeLeft(timeleft);
                rect.Stroke = Brushes.Red;
                rect.Fill = new SolidColorBrush(Color.FromArgb(64, 255, 0, 0));
            }
            else
            {
                if (timeleft <= 0)
                {
                    //unknown time left
                    rect.Stroke = Brushes.Gray;
                    rect.Fill = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
                    data.Text = "?";
                }
                else
                {
                    //house is not set for Demolition
                    data.Text = "";
                    rect.Stroke = SmallHouse.GreenStroke;
                    rect.Fill = SmallHouse.GreenFill;
                }
            }
        }

        public static string CalculateTimeLeft(ulong time)
        {
            if (time == 0) return "?";

            DateTime DT = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(time);

            TimeSpan timeLeft = DT - DateTime.UtcNow;

            if (timeLeft.Days > 0)
                return string.Format("{0}d", timeLeft.Days);
            else if (timeLeft.Hours > 0)
                return string.Format("{0}h", timeLeft.Hours);
            else if (timeLeft.Minutes > 0)
                return string.Format("{0}m", timeLeft.Minutes);
            else
                return string.Format("{0}s", timeLeft.Seconds);
        }
    }
}
