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
        public static bool HouseScannerEnabled = true;
        public RadarPlotter radarPlotter;
        public RadarWindow radarWindow;
        private Core ArcheBuddyCore;

        //constant to avoid sending too many taxinfo requests at once
        private List<DemolishingHouse> RedHouses;
        public Dictionary<uint, HouseStatus> HouseStatuses;

        public int TaxInfo_CD = 10000;
        public bool TaxInfo_Ticked = false;

        public int Prediction_CD = 2000;

        List<uint> activePlantZones = null;

        public HouseScanner(RadarPlotter plotter, RadarWindow radar)
        {
            radarPlotter = plotter;
            radarWindow = radar;
            ArcheBuddyCore = radar.ArcheBuddyCore;

            RedHouses = new List<DemolishingHouse>();
            HouseStatuses = new Dictionary<uint, HouseStatus>();

            activePlantZones = new List<uint>();
        }

        public void Tick()
        {
            //reset the ticker for taxinfo, call once per draw, not per house
            //using a logical clock because more multi-threading will give me a headache
            TaxInfo_Ticked = false;
            if (TaxInfo_CD >= 0) TaxInfo_CD -= radarWindow.CurrentSettings.DisplaySettings.DrawDelay;

            if (Prediction_CD >= 0) Prediction_CD -= radarWindow.CurrentSettings.DisplaySettings.DrawDelay;
            else
            {
                Prediction_CD = 2000;
                activePlantZones = radarWindow.ArcheBuddyCore.getDoodads().Where(d => d.plantZoneId > 0 && d.tempGrowthTime > 0).Select(d => d.plantZoneId).Distinct().ToList();
            }
        }

        public void ParseHouse(UIElement shape, Housing h)
        {
            HouseStatus status;
            //do we remember this house?
            if (HouseStatuses.ContainsKey(h.uniqHousingId))
            {
                //a previously seen house
                status = HouseStatuses[h.uniqHousingId];
                if (status == HouseStatus.Unknown || status == HouseStatus.ProbablyProtected)
                {
                    //are there any updates to taxInfo?
                    if (h.taxPayedTime > 0)
                    {
                        if (h.weeksWithoutPay > 0)
                        {
                            status = HouseStatus.Demolishing;
                            RedHouses.Add(new DemolishingHouse(h, ArcheBuddyCore, shape));
                        }
                        else
                        {
                            status = HouseStatus.TaxProtected;
                        }
                    }
                    else if (status == HouseStatus.Unknown && HouseHasGrowingPlants(h))
                    {
                        //does this house have active plants?
                        status = HouseStatus.ProbablyProtected;
                    }
                    else if (status == HouseStatus.Unknown || !radarWindow.CurrentSettings.HouseScanSettings.PredictTaxStatus) //consider scanning the house
                    {
                        if (radarWindow.CurrentSettings.HouseScanSettings.ScanEnabled && TaxInfo_CD < 0)
                        {
                            if (status == HouseStatus.ProbablyProtected && radarWindow.CurrentSettings.HouseScanSettings.PredictTaxStatus)
                            {
                                //do not scan if it's probably protected and we've chosen to respect predictions
                                return;
                            }

                            //if 8x8 and ignore 8x8
                            if (radarWindow.CurrentSettings.HouseScanSettings.Ignore8x8 &&
                                (h.housingId == 267 || h.housingId == 41 || h.housingId == 42 || h.housingId == 43 || h.housingId == 90))
                            {
                                return;
                            }

                            //eligible for scan, UpdateTaxInfo
                            if (radarWindow.CurrentSettings.HouseScanSettings.MeleeScanOnly)
                            {
                                //check distance to see if safe to scan
                                if (ArcheBuddyCore.me.dist(h.X, h.Y) > 15) return;
                            }

                            //safe, we are about to scan!
                            h.UpdateTaxInfo();
                            TaxInfo_CD = radarWindow.CurrentSettings.HouseScanSettings.TaxScanDelay;
                        }
                    }
                }
            }
            else
            {
                //this is a new house, let's remember it
                status = HouseStatus.Unknown;
            }

            HouseStatuses[h.uniqHousingId] = status;
            //update the house color or timeout text
            ((IHousing)shape).UpdateHouse(h, status);
        }

        private bool HouseHasGrowingPlants(Housing c)
        {
            if (Prediction_CD <= 0)
            {
                return activePlantZones.Contains(c.plantZoneId);
            }
            return false;
        }
        
        public void ForgetHouses()
        {
            foreach (uint objId in HouseStatuses.Keys) {
                var vec = radarPlotter.VectorCache[objId];
                if (vec != null) { 
                    radarPlotter.PurgeVector(objId);
                    radarWindow.RadarCanvas.Children.Remove(vec);
                }
            }
            
            RedHouses = new List<DemolishingHouse>();
            HouseStatuses = new Dictionary<uint, HouseStatus>();
        }

        public void PrintRedHouses()
        {
            ArcheBuddyCore.Log("**************************************************************");
            ArcheBuddyCore.Log("COPY AND PASTE THE FOLLOWING INTO A GOOGLE SPREADSHEET!");
            ArcheBuddyCore.Log("BETWEEN ****** LINES - OMIT HEADER IF YOU ALREADY HAVE ONE");
            ArcheBuddyCore.Log("**************************************************************");
            string header = "uID\tName\tType\tX\tY\tZ\tZone\tTimeDemolish\tTimeLeft\tTimeDemolishUTC(Verify)\tYear\tMonth\tDay\tHour\tMinute\tSecond\t";
            ArcheBuddyCore.Log(header);
            ArcheBuddyCore.Log("---------\t---------\t---------\t---------\t-----------\t----------\t------------\t-----------\t----------\t---------");
            foreach (DemolishingHouse h in RedHouses)
            {
                DateTime doneUTCTime = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(h.taxPayedTime);
                TimeSpan timeLeft = doneUTCTime - DateTime.UtcNow;
                DateTime doneLocalTime = DateTime.Now.Add(timeLeft);

                StringBuilder sb = new StringBuilder();
                sb.Append(h.uniqHousingId).Append("\t");        //A
                sb.Append(h.name).Append("\t");             //B

                if (h.housingId == 267) //Scarecrow Garden      //C
                {
                    sb.Append("Garden 8x8\t");
                }
                else if (h.housingId == 89) //Scarecrow Farm
                {
                    sb.Append("Farm 16x16\t");
                }
                else if (h.housingId == 171) //ThatchedFarmhouse
                {
                    sb.Append("Farmhouse 24x24\t");
                }
                else if (h.housingId >= 172 && h.housingId <= 181) //Small house
                {
                    sb.Append("House 16x16\t");
                }
                else if (h.housingId == 41 || h.housingId == 42 || h.housingId == 43 || h.housingId == 90)
                {
                    sb.Append("Workbench 8x8\t");
                }
                sb.Append(h.X).Append("\t");   //D
                sb.Append(h.Y).Append("\t");    //E
                sb.Append(h.Z).Append("\t");    //F
                sb.Append(h.zone).Append("\t"); //G
                sb.Append("=Date(K:K,L:L,M:M) + Time(N:N,O:O,P:P) \t"); //H
                sb.Append("=H:H - NOW() \t");               //I

                sb.Append(doneLocalTime.ToShortDateString()).Append(":").Append(doneLocalTime.ToLongTimeString()).Append("\t"); //J
                sb.Append(doneLocalTime.Year).Append("\t");     //K
                sb.Append(doneLocalTime.Month).Append("\t");        //L
                sb.Append(doneLocalTime.Day).Append("\t");      //M
                sb.Append(doneLocalTime.Hour).Append("\t");     //N
                sb.Append(doneLocalTime.Minute).Append("\t");       //O
                sb.Append(doneLocalTime.Second).Append("\t");       //P

                ArcheBuddyCore.Log(sb.ToString());
                //sb.Append("\n");
            }
            ArcheBuddyCore.Log("**************************************************************");
        }


        public void DisposeAll()
        {
            //not sure if this is necessary, just being safe
            RedHouses = null;
            HouseStatuses = null;
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
