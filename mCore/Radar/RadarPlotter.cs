using ArcheBuddy.Bot.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using mCore.Models;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using mCore.Shapes;


namespace mCore.Radar
{
    public class RadarPlotter
    {
        Core ArcheBuddyCore;
        Canvas RadarCanvas;
        FrameworkElement RadarArea;
        Ellipse PlayerDot;
        Polygon PlayerViewingDirection;
        RadarWindow radarWindow;
        public HouseScanner houseScanner;

        public Dictionary<uint, UIElement> VectorCache;
        public Dictionary<uint, UIElement> TextCache;
        public Dictionary<uint, UIElement> DotCache;
        
        public bool UseSimpleDots = false;

        private Dictionary<uint, bool> ThunderBeeped;

        public RadarPlotter(RadarWindow _radarWindow)
        {
            radarWindow = _radarWindow;
            RadarCanvas = radarWindow.RadarCanvas;
            RadarArea = radarWindow.RadarArea;
            PlayerDot = radarWindow.PlayerDot;
            PlayerViewingDirection = radarWindow.PlayerViewingDirection;
            
            VectorCache = new Dictionary<uint, UIElement>();
            TextCache = new Dictionary<uint, UIElement>();
            DotCache = new Dictionary<uint, UIElement>();

            houseScanner = new HouseScanner(this, radarWindow);

            ThunderBeeped = new Dictionary<uint, bool>();
            
            ArcheBuddyCore = radarWindow.ArcheBuddyCore;
            if (ArcheBuddyCore != null)
            {
                DrawPlayer();

                ArcheBuddyCore.onDoodadRemoved += RemoveObjectFromRadar;
                ArcheBuddyCore.onCreatureRemoved += RemoveObjectFromRadar;
            }
            
        }

        public void Draw(List<ClassifiedObject> doodads, List<ClassifiedObject> creatures)
        {
            //hide everything
            foreach (UIElement ele in RadarCanvas.Children)
            {
                if (!(ele is IHousing) || !radarWindow.CurrentSettings.HouseScanSettings.ShowRealEstate)
                    ele.Visibility = Visibility.Hidden;
            }

            if (doodads != null)
            {
                foreach (ClassifiedObject co in doodads)
                {
                    DrawObjectSymbol(co);

                    if (radarWindow.CurrentSettings.DisplaySettings.BeepThunder && co.category == ObjectCategory.ThunderstruckTree && !ThunderBeeped.ContainsKey(co.obj.objId))
                    {
                        radarWindow.Beep(null,null);
                        ThunderBeeped[co.obj.objId] = true;
                    }

                    //draw text 
                    if (radarWindow.CurrentSettings.ActiveTab.DisplayNames)
                        DrawText(co.obj.objId, ((DoodadObject)co.obj).name, co.obj.X, co.obj.Y, co.category.Color());
                }
            }

            if (creatures != null)
            {
                foreach (ClassifiedObject co in creatures)
                {
                    DrawObjectSymbol(co);

                    //draw text 
                    if (radarWindow.CurrentSettings.ActiveTab.DisplayNames && !(co.obj is Housing) && !(co.obj is Slave))
                        DrawText(co.obj.objId, ((Creature)co.obj).name, co.obj.X, co.obj.Y, co.category.Color());
                }
            }
            houseScanner.Tick();

            UpdatePlayer();
            
        }

        private void DrawObjectSymbol(ClassifiedObject CObj)
        {
            SpawnObject o = CObj.obj;
            //if (c > MAX_NUMBER) break;

            if (UseSimpleDots)
            {
                DrawDot(o.objId, o.X, o.Y, CObj.category.Color());
            }
            else
            {
                UIElement shape = DrawShape(CObj);

                UpdateShapeData(shape, CObj);
            }
        }
        public void DrawPlayer()
        {
            radarWindow.PlayerX = ArcheBuddyCore.me.X;
            radarWindow.PlayerY = ArcheBuddyCore.me.Y;

        }

        private void UpdatePlayer()
        {
            //Canvas.SetTop(PlayerViewingDirection, ArcheBuddyCore.me.Y + 3);
            //Canvas.SetLeft(PlayerViewingDirection, ArcheBuddyCore.me.X + 3);
            
            double turnAdjust = ArcheBuddyCore.me.turnAngle * 360 / 128 - 28;
            //((RotateTransform)PlayerViewingDirection.RenderTransform).Angle = turnAdjust;

            PlayerDot.Visibility = Visibility.Visible;
            PlayerViewingDirection.Visibility = Visibility.Visible;

            //update the viewmodel with updated player information
            radarWindow.PlayerAngleDegrees = turnAdjust;
            radarWindow.PlayerX = ArcheBuddyCore.me.X;
            radarWindow.PlayerY = ArcheBuddyCore.me.Y;
            radarWindow.NotifyPropertyChanged("");
        }

        private void RemoveObjectFromRadar(SpawnObject o)
        {
            //do not remove housing from radar, since they don't move
            if (o.type == BotTypes.Housing) return;

            UIElement vec = LookupVector(o.objId);
            UIElement txt = LookupText(o.objId);
            UIElement dot = LookupDot(o.objId);

            if (vec != null) RadarCanvas.Children.Remove(vec);
            if (txt != null) RadarCanvas.Children.Remove(txt);
            if (dot != null) RadarCanvas.Children.Remove(dot);

            PurgeVector(o.objId);
            PurgeText(o.objId);
            PurgeDot(o.objId);
        }
        internal void PurgeVector(uint key) { PurgeObject(key, VectorCache); }
        internal void PurgeText(uint key) { PurgeObject(key, TextCache); }
        internal void PurgeDot(uint key) { PurgeObject(key, DotCache); }
        private void PurgeObject(uint key, Dictionary<uint, UIElement> cache)
        {
            if (cache != null && cache.ContainsKey(key)) cache.Remove(key);
        }
        internal UIElement LookupDot(uint key) { return LookupObject(key, DotCache); }
        internal UIElement LookupText(uint key) { return LookupObject(key, TextCache); }
        internal UIElement LookupVector(uint key) { return LookupObject(key, VectorCache); }
        
        private UIElement LookupObject(uint key, Dictionary<uint, UIElement> cache)
        {
            if (cache != null && cache.ContainsKey(key))
            {
                return cache[key];
            }

            return null;
        }
        private void StoreDot(uint key, UIElement vector) { StoreObject(key, vector, DotCache); }
        private void StoreText(uint key, UIElement vector) { StoreObject(key, vector, TextCache); }
        private void StoreVector(uint key, UIElement vector) { StoreObject(key, vector, VectorCache); }
       
        private void StoreObject(uint key, UIElement vector, Dictionary<uint, UIElement> cache)
        {
            if (cache != null)
                cache[key] = vector;
        }
        private UIElement DrawShape(ClassifiedObject CObj)
        {
            SpawnObject o = CObj.obj;
            uint id = o.objId;
            double x = o.X;
            double y = o.Y;

            UIElement shape = LookupVector(id);
            if (shape == null)
            {
                shape = CObj.category.Shape();
                StoreVector(id, shape);
                RadarCanvas.Children.Add(shape);
            }

            Canvas.SetTop(shape, y);
            Canvas.SetLeft(shape, x);
            shape.Visibility = Visibility.Visible;
                
            return shape;
        }
        private UIElement DrawDot(uint id, double x, double y, Brush color)
        {
            UIElement dot = LookupDot(id);
            if (dot == null)
            {
                dot = new Ellipse(){
                    Fill = color,
                    Width = 5,
                    Height = 5
                };
                StoreDot(id, dot);
                RadarCanvas.Children.Add(dot);
            }

            Canvas.SetTop(dot, y);
            Canvas.SetLeft(dot, x);
            dot.Visibility = Visibility.Visible;

            return dot;
        }

        private UIElement DrawText(uint id, string text, double x, double y, Brush color)
        {
            UIElement txt = LookupText(id);
            if (txt == null)
            {
                //remove useless adjectives that clutter the screen
                text = text.Replace("Fruited ", "").Replace("Nonnative ", "").Replace("Mature ", "");
                txt = new TextBlock()
                {
                    Text = text,
                    Foreground = color,
//                    FontWeight = FontWeights.Light,
                    FontSize = 8,
                    RenderTransform = new ScaleTransform(1,-1)
                };
                StoreText(id, txt);
                RadarCanvas.Children.Add(txt);
            }

            Canvas.SetTop(txt, y + 8);
            Canvas.SetLeft(txt, x + 8); 
            txt.Visibility = Visibility.Visible;

            return txt;
        }
        private void UpdateShapeData(UIElement shape, ClassifiedObject obj)
        {
            if (shape is ITurnable)
            {
                ((ITurnable)shape).Turn(obj.obj.turnAngle);
            }

            //this is ugly, can there be a solution using WPF MVVM Databinding?
            if (obj.obj.type == BotTypes.Housing && shape is IHousing)
            {
                houseScanner.ParseHouse(shape, (Housing)obj.obj);
            }
            else if (obj.category == ObjectCategory.ThunderstruckTree && shape is Tree)
            {
                //this should rarely happen - if a tree turns Thunderstruck while it's on radar, it might keep the same objId
                //so we might need to redraw it
                ((Tree)shape).ThunderStrike();
            }
            else if (obj.category == ObjectCategory.HarvestableTree && shape is Tree)
            {
                //ensure that a fruited tree is not shown as harvestable
                ((Tree)shape).HideFruit();
            }
            else if (obj.obj.type == BotTypes.DoodadObject)
            {
                DoodadObject d = (DoodadObject)obj.obj;
                if (d.id == 1671) {
                    UIElement u = LookupText(obj.obj.objId);
                    if (u != null)
                    {
                        ((TextBlock)u).Text = d.name;
                    }
                }
            }
            else if (obj.obj.type == BotTypes.Player)
            {
                Player p = (Player)obj.obj;

                if (shape is IKillable)
                {
                    ((IKillable)shape).Kill(p.isAlive());
                }
                if (shape is IStealthable) { 
                    bool IsStealthed = false;
                    foreach (Buff b in p.getBuffs())
                    {
                        if (b.name.StartsWith("Stealth"))
                        {
                            IsStealthed = true;
                            break;
                        }
                    }

                    //this player is stealthed, update their vector
                    ((IStealthable)shape).Stealth(IsStealthed);
                }

                //check for trade pack
                ((PlayerShape)shape).ShowTradePack(p.getAllEquipedItems().Any(item => item.equipCell == EquipItemPlace.Unknown && RadarScanner.TradePackNames.Any(item.name.Contains)));
               
            }
            else if (obj.obj.type == BotTypes.Npc && shape is IStealthable)
            {
                Npc p = (Npc)obj.obj;
                bool IsStealthed = false;
                foreach (Buff b in p.getBuffs())
                {
                    if (b.name.StartsWith("Stealth"))
                    {
                        IsStealthed = true;
                        break;
                    }
                }

                //this player is stealthed, update their vector
                ((IStealthable)shape).Stealth(IsStealthed);
            }
            
        }

        public void DrawTestCase()
        {
            //does some stuff on canvas when archeage is not running
            DrawDot(1, 50, 50, Brushes.GhostWhite);
            DrawDot(2, 100, 100, Brushes.GhostWhite);
            DrawDot(3, 170, 170, Brushes.Green);
            DrawDot(4, 200, 200, Brushes.GhostWhite);
            DrawDot(5, 250, 250, Brushes.Red);
            
            radarWindow.PlayerX = 150;
            radarWindow.PlayerY = 150;
            radarWindow.NotifyPropertyChanged("");
        }

        public void DisposeAll()
        {
            houseScanner.DisposeAll(); 
            houseScanner = null;
            RadarCanvas = null; //.Children.RemoveRange(0, RadarCanvas.Children.Count);
            VectorCache = null;
        }
    }

}
