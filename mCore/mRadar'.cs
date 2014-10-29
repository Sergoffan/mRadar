using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcheBuddy.Bot.Classes;
using mCore;
using System.Windows;
using System.Threading;

namespace mCore
{
    public class mRadar : Core
    {
        public static string GetPluginAuthor()
        {
            return "Tempura";
        }

        public static string GetPluginDescription()
        {

            return "mRadar Basic";
        }

        public static string GetPluginVersion()
        {
            return "0.5";
        }

        public mRadar()
        {

        }

        Window window = null;
        Thread thread = null;
        public void PluginRun()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Log("Starting ArcheCore");
            thread = new Thread(Start);
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();

            thread.Join();
            Log("ArcheCore Exit");

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log("GENERAL ERROR");
            Exception ex = (Exception)e.ExceptionObject;
            string st = ex.StackTrace.Length > 300 ? ex.StackTrace.Substring(0, 300) : ex.StackTrace;
            Log(st);
            Log(ex.Message);
        }

        public void Start()
        {
            window = new mCore.Radar.RadarWindow(this);
            window.Show();

            window.Closed += (sender2, e2) =>
            {
                if (window != null) window.Dispatcher.InvokeShutdown();
            };

            System.Windows.Threading.Dispatcher.Run();
        }
    }
}
