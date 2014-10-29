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
    class HouseScanner
    {
        private RadarPlotter radarPlotter;
        private RadarWindow radarWindow;
        private Core ArcheBuddyCore;

        //constant to avoid sending too many taxinfo requests at once
        private List<HouseClone> RedHouses;
        private Dictionary<uint, bool> HouseIsGreen;
        public int TaxInfo_CD = 10000;
        public bool TaxInfo_Ticked = false;


        public HouseScanner(RadarPlotter plotter, RadarWindow radar)
        {
            radarPlotter = plotter;
            radarWindow = radar;
            ArcheBuddyCore = radar.ArcheBuddyCore;

            RedHouses = new List<HouseClone>();
            HouseIsGreen = new Dictionary<uint, bool>();
        }

        public void Tick()
        {
            //reset the ticker for taxinfo, call once per draw, not per house
            //using a logical clock because more multi-threading will give me a headache
            TaxInfo_Ticked = false;
        }

        public void ParseHouse(UIElement shape, Housing h)
        {
            //code removed
            ((ITimedObject)shape).UpdateTime(h.weeksWithoutPay, h.taxPayedTime);
        }

        public void ForgetHouses()
        {
            RedHouses = new List<HouseClone>();
            HouseIsGreen = new Dictionary<uint, bool>();
        }

        public void PrintRedHouses()
        {
            ArcheBuddyCore.Log("**************************************************************");
            ArcheBuddyCore.Log("code was removed");
            ArcheBuddyCore.Log("**************************************************************");

        }

        public void DisposeAll()
        {
            RedHouses = null;
        }
    }

    class HouseClone
    {
        public uint uniqHousingId;
        public byte weeksWithoutPay;
        public ulong taxPayedTime;
        public string name;
        public string zone;
        public uint housingId;
        public double X;
        public double Y;
        public double Z;
        public HouseClone(Housing h, Core ArcheBuddyCore)
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
        }
    }
}
