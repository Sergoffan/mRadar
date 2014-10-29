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
using mCore.Models;
using ArcheBuddy.Bot.Classes;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media.Animation;
using System.Media;

namespace mCore.Radar
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class RadarWindow : MetroWindow, INotifyPropertyChanged
    {
        public RadarSettings CurrentSettings {get; set;}
        public ObservableCollection<RadarTab> rTabs { get; set; }
        internal Core ArcheBuddyCore;
        private RadarPlotter Plotter;
        private int radarWidth = 500;
        private int radarHeight = 500;

        private double _playerX = 59;
        private double _playerY = 59;
        private double _playerAngleDegrees = 180;
        public double PlayerX { get { return _playerX; } set { _playerX = value; } }
        public double PlayerY { get { return _playerY; } set { _playerY = value; } }
        public double PlayerAngleDegrees { get { return _playerAngleDegrees; } set { _playerAngleDegrees = value; } }

        private BackgroundWorker worker = new BackgroundWorker();
        public bool WorkerIsDone = false;

        public RadarWindow(Core core)
        {
            InitializeComponent();

            CurrentSettings = LoadUserRadarData();
            rTabs = new ObservableCollection<RadarTab>(CurrentSettings.Tabs);

            //RadarSettingsTabs.ItemsSource =  rTabs;

            this.ArcheBuddyCore = core;
            this.AllowsTransparency = true;

            Plotter = new RadarPlotter(this);
            
            //Log("Radar Loaded");
            worker.WorkerSupportsCancellation = true;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            radarWidth = (int)RadarArea.ActualWidth;
            radarHeight = (int)RadarArea.ActualHeight;

            this.MouseWheel += RadarWindow_MouseWheel;
            if (ArcheBuddyCore != null) {
                worker.WorkerReportsProgress = true;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                base.OnContentRendered(e);

                worker.RunWorkerAsync();
            }
            else
            {
                Plotter.DrawTestCase();
                WorkerIsDone = true;
            }

            this.DumpRedRealEstate.Click += DumpRedRealEstate_Click;
            this.ForgetAllHouses.Click += ForgetAllHouses_Click;
        }

        void RadarWindow_MouseWheel(object sender, MouseWheelEventArgs e) {
            CanvasZoomSlider.Value += (e.Delta / 500.0);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                //progress changed is called when the radar has new data that requires update
                //we use the UI thread to do the update
                if (ArcheBuddyCore != null && Plotter != null)
                    Plotter.Draw(doodads, creatures);

            }
            catch (Exception ex)
            {
                string st = ex.StackTrace.Length > 300 ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
                ArcheBuddyCore.Log("Radar Thread threw exception while drawing.");
                ArcheBuddyCore.Log(ex.Message);
                ArcheBuddyCore.Log(st);
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerIsDone = true;
            worker.Dispose();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try {
                while (!worker.CancellationPending)
                {
                    Scan();

                    worker.ReportProgress(1);
                    Thread.Sleep(CurrentSettings.DisplaySettings.DrawDelay);
                }
            }
            catch (ThreadAbortException tae)
            {
                //ignore this thing until I can implement it better
                ArcheBuddyCore.Log("Radar Background Worker threw ThreadAbortException.");
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                ArcheBuddyCore.Log("Radar Background Worker threw exception.");
                ArcheBuddyCore.Log(ex.Message);
                string st = ex.StackTrace.Length > 300 ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
                ArcheBuddyCore.Log(ex.StackTrace.Substring(0,300));
            }
            //e.Cancel = true;
        }


        void DumpRedRealEstate_Click(object sender, RoutedEventArgs e)
        {
            Plotter.houseScanner.PrintRedHouses();
        }
        void ForgetAllHouses_Click(object sender, RoutedEventArgs e)
        {
            Plotter.houseScanner.ForgetHouses();
        }
        private List<ClassifiedObject> doodads = null;
        private List<ClassifiedObject> creatures = null;
        void Scan()
        {
            //Scan() runs in a BackgroundWorker thread, so it CANNOT access UI elements!

            //not running Archeage, no data
            if (ArcheBuddyCore == null) return;

            RadarTab settings = CurrentSettings.ActiveTab;

            if (settings.ShowDoodads || settings.ShowHarvestableTrees || settings.ShowHarvestablePlants || settings.ShowUprootable || settings.ShowTradePacks)
            {
                doodads = RadarScanner.ScanDoodads(settings, ArcheBuddyCore);
            } else {
                doodads = null;
            }

            if (settings.ShowAlliedPlayers || settings.ShowEnemyNPCs || settings.ShowEnemyPlayers || settings.ShowRealEstate || settings.ShowFriendlyNPCs)
            {
                creatures = RadarScanner.ScanCreatures(settings, ArcheBuddyCore);
            }
            else
            {
                creatures = null;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (worker != null) { 
                worker.CancelAsync();
            }

            Plotter.DisposeAll();

            //ArcheBuddyCore.Log("Dictionary has " + Plotter.VectorCache.Count + " objects");
            //ArcheBuddyCore.Log("Canvas has " + RadarCanvas.Children.Count + " children");
            Plotter = null;

            base.OnClosing(e);
            

            //when the radar window is closed, save our settings
            //string json = JsonConvert.SerializeObject(CurrentSettings);

            //mCore.Properties.Settings.Default.RadarSettings = json;
        }

        public RadarSettings LoadUserRadarData() {
            //string json = mCore.Properties.Settings.Default.RadarSettings;

            //if (true) //string.IsNullOrWhiteSpace(json))
            //{
                //create a default radar settings for first timers
                return new RadarSettings
                {
                    Tabs = new List<RadarTab>(new RadarTab[] { 
                            new RadarTab { ShowHarvestableTrees = true, ShowAlliedPlayers = true, ShowEnemyPlayers = true, Index = 1,  Name = "1", DisplayNames = true }//,
                            //new RadarTab { ShowAll = true, Index = 2, Name = "2" }
                        }),
                    ActiveTabIndex = 0,
                    DisplaySettings = new Models.RadarDisplaySettings { RadarOpacity = 0.8 }
                };
            //}
            //else
            //{
                //RadarSettings items = JsonConvert.DeserializeObject<RadarSettings>(json);
                //return items;
            //}
        }

        private void Log(string text)
        {
            if (ArcheBuddyCore != null)
                ArcheBuddyCore.Log(text);
        }

        private void DebugNearestDoodadClick(Object sender, EventArgs e)
        {
            //find the nearest visible object
            DoodadObject d = GetNearestDoodad();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(d))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(d);
                ArcheBuddyCore.Log(string.Format("Debug({2}) : {0} = {1}", name, value, d.name));
            }

            foreach (Skill k in d.getUseSkills())
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(k))
                {
                    string name = descriptor.Name;
                    object value = descriptor.GetValue(k);
                    ArcheBuddyCore.Log(string.Format("Debug({2}) : Skill({3}) : {0} = {1}", name, value, d.name, k.name));
                }    
            }
            
        }
        private void DebugTargetCreatureClick(Object sender, EventArgs e)
        {
            //find the nearest visible object
            Creature d = ArcheBuddyCore.me.target;

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(d))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(d);
                ArcheBuddyCore.Log(string.Format("Debug({2}) : {0} = {1}", name, value, d.name));
            }
        }
        private DoodadObject GetNearestDoodad()
        {
            DoodadObject nearest = null;
            double D2 = double.PositiveInfinity;
            foreach (DoodadObject d in ArcheBuddyCore.getDoodads())
            {
                double d2 = d.distNoZ(ArcheBuddyCore.me);
                if (d2 < D2)
                {
                    nearest = d;
                    D2 = d2;
                }
            }

            return nearest;
        }

        private ClassifiedObject GetNearestVisibleDoodad()
        {
            //find the nearest visible object
            if (doodads != null && doodads.Any())
            {
                double D2 = double.PositiveInfinity;
                ClassifiedObject nearest = null;
                foreach (ClassifiedObject cd in doodads)
                {
                    double d2 = cd.obj.distNoZ(ArcheBuddyCore.me);
                    //double dx = (cd.doodad.X - ArcheBuddyCore.me.X);
                    //double dy = (cd.doodad.Y - ArcheBuddyCore.me.Y);
                    //double d2 = dx * dx + dy * dy;
                    if (d2 < D2)
                    {
                        nearest = cd;
                        D2 = d2;
                    }
                }

                return nearest;
            }
            return null;
        }

        public bool SettingsVisible = true;
        DoubleAnimation shrink = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
        DoubleAnimation grow = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
        public void ToggleSettings(object sender, RoutedEventArgs e)
        {
            if (SettingsVisible)
            {
                SettingsArea.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, shrink);

                SettingsVisible = false;
            }
            else
            {   
                SettingsArea.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, grow);

                SettingsVisible = true;
            }
        }

        public void Beep()
        {
            Uri uri = new Uri(@"Resources/beep.wav",UriKind.Relative);
            var player = new MediaPlayer();
            player.Open(uri);
            player.Play();

        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class RoundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round((double)value);
        }
    }
    public class NegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return -(double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return -(double)value;
        }
    }
    
    public class CenterTranslateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || values[0] == null || values[1] == null) return 0;

            double playerC = System.Convert.ToDouble(values[0]);
            double RadarAreaC = System.Convert.ToDouble(values[1]);
            return (RadarAreaC / 2) - playerC;
            /*string XorY = (string)parameter;


            if (XorY.Equals("X")) {
                //return playerC - (RadarAreaC/2);
                
            } else {
                return (RadarAreaC / 2) - playerC;
            }
             */
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BackgroundOpacityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Color.FromArgb((byte)values[0], 72, 72, 72));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ClassifiedObject
    {
        public ObjectCategory category;
        public SpawnObject obj;
    }
}
