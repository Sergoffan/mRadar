using ArcheBuddy.Bot.Classes;
using mCore.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace mCore.Radar
{
    public enum HouseStatus
    {
        Unknown,
        TaxProtected,
        ProbablyProtected,
        Demolishing
    }

    public class HouseScanner
    {
        public static bool HouseScannerEnabled = false;
        public RadarPlotter radarPlotter;
        public RadarWindow radarWindow;
        private Core ArcheBuddyCore;

        public HouseScanner(RadarPlotter plotter, RadarWindow radar)
        {
            radarPlotter = plotter;
            radarWindow = radar;
            ArcheBuddyCore = radar.ArcheBuddyCore;
        }

        public void Tick()
        {
        }

        public void ParseHouse(UIElement shape, Housing h)
        {
            HouseStatus status = HouseStatus.Unknown;
            
            //are there any updates to taxInfo?
            if (h.taxPayedTime > 0)
            {
                if (h.weeksWithoutPay > 0)
                {
                    status = HouseStatus.Demolishing;
                }
                else
                {
                    status = HouseStatus.TaxProtected;
                }
            }

            //update the house color or timeout text
            ((IHousing)shape).UpdateHouse(h, status);
        }

        public void ForgetHouses()
        {

        }

        public void PrintRedHouses()
        {


        }


        public void DisposeAll()
        {

        }
    }

    public class DemolishingHouse
    {
        public uint uniqHousingId;
        public byte weeksWithoutPay;
        public ulong taxPayedTime;
        public DateTime timeEnding;
        public string name;
        public string zone;
        public uint housingId;
        public double X;
        public double Y;
        public double Z;
        public uint objId;
        //public bool taxPredicted = false;
        public DemolishingHouse(Housing h, Core ArcheBuddyCore, UIElement v)
        {
            uniqHousingId = h.uniqHousingId;
            weeksWithoutPay = h.weeksWithoutPay;
            taxPayedTime = h.taxPayedTime;
            name = h.name;
            housingId = h.housingId;
            X = h.X;
            Y = h.Y;
            Z = h.Z;
            zone = ArcheBuddyCore.getCurrentTerritory().displayName;
            objId = h.objId;
        }
    }

    
}
