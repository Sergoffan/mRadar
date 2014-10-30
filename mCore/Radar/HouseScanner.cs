using ArcheBuddy.Bot.Classes;
using mCore.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        //constant to avoid sending too many taxinfo requests at once
        private List<HouseClone> RedHouses;
        public Dictionary<uint, HouseClone> AllHouses;

        public HouseScanner(RadarPlotter plotter, RadarWindow radar)
        {
            radarPlotter = plotter;
            radarWindow = radar;
            ArcheBuddyCore = radar.ArcheBuddyCore;

            AllHouses = new Dictionary<uint, HouseClone>();
        }

        public void Tick()
        {
            //code removed
            
        }

        public void ParseHouse(UIElement shape, Housing h)
        {
            //a lot of code removed
            HouseClone hc = new HouseClone(h, ArcheBuddyCore, shape);

            //update the house color or timeout text
            ((IHousing)shape).UpdateHouse(hc);
        }

        public void ForgetHouses()
        {
            foreach (HouseClone hc in AllHouses.Values) {
                radarPlotter.VectorCache.Remove(hc.objId);
                radarWindow.RadarCanvas.Children.Remove(hc.vector);
            }
            
            RedHouses = new List<HouseClone>();
            AllHouses = new Dictionary<uint, HouseClone>();
        }

        public void PrintRedHouses()
        {
            //code removed
        }
        public void DisposeAll()
        {
            //not sure if this is necessary, just being safe
            RedHouses = null;
            AllHouses = null;
        }
    }

    public class HouseClone
    {
        public uint uniqHousingId;
        public byte weeksWithoutPay;
        public ulong taxPayedTime;
        public uint plantZoneId;
        public HouseStatus Status = HouseStatus.Unknown;
        public DateTime timeEnding;
        public string name;
        public string zone;
        public uint housingId;
        public double X;
        public double Y;
        public double Z;
        public uint objId;
        //public bool taxPredicted = false;
        public UIElement vector;
        public HouseClone(Housing h, Core ArcheBuddyCore, UIElement v)
        {
            uniqHousingId = h.uniqHousingId;
            weeksWithoutPay = h.weeksWithoutPay;
            taxPayedTime = h.taxPayedTime;
            name = h.name;
            housingId = h.housingId;
            X = h.X;
            Y = h.Y;
            Z = h.Z;
            plantZoneId = h.plantZoneId;
            zone = ArcheBuddyCore.getCurrentTerritory().displayName;
            vector = v;
            objId = h.objId;
        }



    }

    
}
