using mCore.Radar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace mCore.Models
{
    [Serializable()]
    public class RadarSettings 
    {
        public List<RadarTab> Tabs { get; set; }
        public int ActiveTabIndex { get; set; }
        public RadarTab ActiveTab
        {
            get
            {
                return Tabs[ActiveTabIndex];
            }
        }

        public RadarDisplaySettings DisplaySettings { get; set; }
        public HouseScanningSettings HouseScanSettings { get; set; }
        public int RadarWindowWidth { get; set; }
        public int RadarWindowHeight { get; set; }
    }



    public enum ObjectCategory
    {
        HarvestableTree,
        FruitedTree,
        HarvestablePlant,
        Ore,
        Uprootable,
        EnemyPlayer,
        EnemyNPC,
        FriendlyPlayer,
        FriendlyNPC,
        TradePack,
        Butcherable,
        ThunderstruckTree,
        FishSchool,
        Treasure,
        ScarecrowFarm,
        ScarecrowGarden,
        SmallHousing,
        MediumHousing,
        LargeHousing,
        HousingWorkbench,
        Farmhouse,
        OtherDoodad,
        OtherCreature
    }
    public class RadarDisplaySettings : INotifyPropertyChanged
    {
        private double _radarOpacity;
        private int _drawDelay = 100;
        private bool _beepThunder = false;
        
        public double RadarOpacity { get { return _radarOpacity; } set { _radarOpacity = value; this.OnPropertyChanged("RadarOpacity"); } }
        public int DrawDelay { get { return _drawDelay; } set { _drawDelay = value; this.OnPropertyChanged("DrawDelay"); } }
        public bool BeepThunder { get { return _beepThunder; } set { _beepThunder = value; this.OnPropertyChanged("BeepThunder"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class HouseScanningSettings : INotifyPropertyChanged
    {
        public bool HouseScanningEnabled
        {
            get
            {
                return HouseScanner.HouseScannerEnabled;
            }
        }

        private bool _showRealEstate = false;
        private int _taxScanDelay = 5000; //minimum delay between UpdateTaxInfo calls
        private bool _ignore8x8 = true;
        private bool _predictTaxStatus = true;
        private bool _scanEnabled = false;
        private bool _meleeRangeScanOnly = true;

        public bool ScanEnabled
        {
            get { return _scanEnabled; }
            set { _scanEnabled = value;
            if (!_scanEnabled) _meleeRangeScanOnly = true;
                this.OnPropertyChanged(""); }
        }
        public bool MeleeScanOnly
        {
            get { return _meleeRangeScanOnly; }
            set { _meleeRangeScanOnly = value; this.OnPropertyChanged("MeleeScanOnly"); }
        }
        public bool Ignore8x8
        {
            get { return _ignore8x8; }
            set { _ignore8x8 = value; this.OnPropertyChanged("Ignore8x8"); }
        }

        public bool PredictTaxStatus
        {
            get { return _predictTaxStatus; }
            set { _predictTaxStatus = value; this.OnPropertyChanged("PredictTaxStatus"); }
        }

        public int TaxScanDelay
        {
            get { return _taxScanDelay; }
            set {
                _taxScanDelay = value;
                this.OnPropertyChanged("TaxScanDelay");
            }
        }
        public bool ShowRealEstate
        {
            get { return _showRealEstate; }
            set
            {
                _showRealEstate = value;
                this.OnPropertyChanged("ShowRealEstate");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class RadarTab : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public string Name { get; set; }
        private bool _displayNames;

        private bool _showEnemyPlayers;
        private bool _showFriendlyPlayers;
        private bool _showHarvestableTrees;
        private bool _showHarvestablePlants;
        private bool _showUprootable;
        private bool _showFriendlyNPCs;
        private bool _showEnemyNPCs;
        private bool _showTradePacks;
        private bool _showDoodads;
        private bool _scanTaxes;
        
        public bool DisplayNames
        {
            get { return _displayNames; }
            set { _displayNames = value; }
        }
        public bool ShowEnemyPlayers { get { return _showEnemyPlayers; } set {
            _showEnemyPlayers = value;
        } }
        public bool ShowAlliedPlayers { get { return _showFriendlyPlayers; } set { 
            _showFriendlyPlayers = value;
            this.OnPropertyChanged(null);
        } }

        public bool ShowHarvestableTrees { get { return _showHarvestableTrees; } set { 
            _showHarvestableTrees = value;
            this.OnPropertyChanged("ShowHarvestableTrees"); } }

        public bool ShowHarvestablePlants
        {
            get { return _showHarvestablePlants; }
            set {
                _showHarvestablePlants = value;
                this.OnPropertyChanged("ShowHarvestablePlants");
            }
        }

        public bool ShowUprootable { get { return _showUprootable; } set { 
            _showUprootable = value; 
            this.OnPropertyChanged(null); } }

        public bool ShowFriendlyNPCs { get { return _showFriendlyNPCs; } set {
            _showFriendlyNPCs = value;
            this.OnPropertyChanged(null); } }

        public bool ShowEnemyNPCs { get { return _showEnemyNPCs; } set { 
            _showEnemyNPCs = value; 
            this.OnPropertyChanged(null); } }
        public bool ShowTradePacks { get { return _showTradePacks; } set {
            _showTradePacks = value; 
            this.OnPropertyChanged("ShowTradePacks"); } }

        public bool ShowDoodads { get { return _showDoodads; } set { 
            _showDoodads = value;
            this.OnPropertyChanged("ShowDoodads"); } }

        public bool ScanForTaxes { get { return _scanTaxes; } set
            {
                _scanTaxes = value;
                this.OnPropertyChanged("ScanForTaxes");
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
