using ArcheBuddy.Bot.Classes;
using mCore.Radar;
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
    public partial class SmallHouse : Canvas, IHousing
    {
        public static SolidColorBrush GreenFill = new SolidColorBrush(Color.FromArgb(38,38,128,38));
        public static SolidColorBrush GreenStroke = new SolidColorBrush(Color.FromArgb(128,0,128,0));
        
        public static SolidColorBrush ProbablyFill = new SolidColorBrush(Color.FromArgb(64, 128, 128, 0));
        public static SolidColorBrush ProbablyStroke = new SolidColorBrush(Color.FromArgb(128, 128, 128, 0));

        public static SolidColorBrush RedFill = new SolidColorBrush(Color.FromArgb(64, 255, 0, 0));
        public static SolidColorBrush RedStroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        public static SolidColorBrush GrayFill = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
        public static SolidColorBrush GrayStroke = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));    

        public SmallHouse()
        {
            InitializeComponent();
        }

        public void UpdateHouse(Housing house, HouseStatus status)
        {
            UpdateHouseVector(rect, Data, house, status);
        }
        public static void UpdateHouseVector(Shape rect, TextBlock data, Housing house, HouseStatus status)
        {
            if (status == HouseStatus.Unknown)
            {
                data.Text = "?";
                rect.Stroke = GrayStroke;
                rect.Fill = GrayFill;
            }
            else if (status == HouseStatus.ProbablyProtected)
            {
                data.Text = "";
                rect.Stroke = ProbablyStroke;
                rect.Fill = ProbablyFill;
            }
            else if (status == HouseStatus.Demolishing)
            {
                data.Text = TimeLeftString(house.taxPayedTime);
                rect.Stroke = RedStroke;
                rect.Fill = RedFill;
            }
            else
            {
                //house is tax protected
                data.Text = "";
                rect.Stroke = SmallHouse.GreenStroke;
                rect.Fill = SmallHouse.GreenFill;
            }
        }
        private static DateTime CalculateEndTime(ulong taxPayedTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(taxPayedTime);
        }

        private static string TimeLeftString(ulong taxPayedTime)
        {
            DateTime timeEnding = CalculateEndTime(taxPayedTime);
            if (timeEnding < DateTime.UtcNow) return "?";

            TimeSpan timeLeft = timeEnding - DateTime.UtcNow;

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
