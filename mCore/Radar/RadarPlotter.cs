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
    class RadarPlotter
    {
        Core ArcheBuddyCore;
        Canvas RadarCanvas;
        FrameworkElement RadarArea;
        Ellipse PlayerDot;
        Polygon PlayerViewingDirection;
        RadarWindow radarWindow;
        public HouseScanner houseScanner;

        public Dictionary<string, UIElement> VectorCache;
        
        public bool UseSimpleDots = false;

        private Dictionary<uint, bool> ThunderBeeped;

        public RadarPlotter(RadarWindow _radarWindow)
        {
            radarWindow = _radarWindow;
            RadarCanvas = radarWindow.RadarCanvas;
            RadarArea = radarWindow.RadarArea;
            PlayerDot = radarWindow.PlayerDot;
            PlayerViewingDirection = radarWindow.PlayerViewingDirection;
            VectorCache = new Dictionary<string, UIElement>();
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
                ele.Visibility = Visibility.Hidden;
            }

            if (doodads != null)
            {
                foreach (ClassifiedObject co in doodads)
                {
                    DrawObjectSymbol(co);

                    //draw text 
                    if (radarWindow.CurrentSettings.ActiveTab.DisplayNames)
                        DrawText(co.obj.objId.ToString(), ((DoodadObject)co.obj).name, co.obj.X, co.obj.Y, co.category.Color());
                }
            }

            if (creatures != null)
            {
                foreach (ClassifiedObject co in creatures)
                {
                    DrawObjectSymbol(co);

                    if (radarWindow.CurrentSettings.DisplaySettings.BeepThunder && co.category == ObjectCategory.ThunderstruckTree && !ThunderBeeped[co.obj.objId]) {
                        radarWindow.Beep();
                        ThunderBeeped[co.obj.objId] = true;
                    }
                    //draw text 
                    if (radarWindow.CurrentSettings.ActiveTab.DisplayNames && !(co.obj is Housing))
                        DrawText(co.obj.objId.ToString(), ((Creature)co.obj).name, co.obj.X, co.obj.Y, co.category.Color());
                }
            }
            UpdatePlayer();
        }

        private void DrawObjectSymbol(ClassifiedObject CObj)
        {
            SpawnObject o = CObj.obj;
            //if (c > MAX_NUMBER) break;

            if (UseSimpleDots)
            {
                DrawDot(o.objId.ToString(), o.X, o.Y, CObj.category.Color());
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
            UIElement vec = LookupVector(o.objId.ToString());
            UIElement txt = LookupVector("text-" + o.objId.ToString());

            RadarCanvas.Children.Remove(vec);
            RadarCanvas.Children.Remove(txt);

            PurgeVector(o.objId.ToString());
            PurgeVector("text-" + o.objId.ToString());
        }
        private void PurgeVector(string key)
        {
            if (VectorCache != null && VectorCache.ContainsKey(key))
                VectorCache.Remove(key);
        }
        private UIElement LookupVector(string key)
        {
            if (key != null && VectorCache != null && VectorCache.ContainsKey(key))
            {
                return VectorCache[key];
            }

            return null;
        }
        private void StoreVector(string key, UIElement vector)
        {
            if (VectorCache != null)
                VectorCache[key] = vector;
        }
        private UIElement DrawShape(ClassifiedObject CObj)
        {
            SpawnObject o = CObj.obj;
            string id = o.objId.ToString();
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
        private UIElement DrawDot(string id, double x, double y, Brush color)
        {
            UIElement dot = LookupVector(id);
            if (dot == null)
            {
                dot = new Ellipse(){
                    Fill = color,
                    Width = 5,
                    Height = 5
                };
                StoreVector(id, dot);
                RadarCanvas.Children.Add(dot);
            }

            Canvas.SetTop(dot, y);
            Canvas.SetLeft(dot, x);
            dot.Visibility = Visibility.Visible;

            return dot;
        }

        private UIElement DrawText(string id, string text, double x, double y, Brush color)
        {
            UIElement txt = LookupVector("text-" + id);
            if (txt == null)
            {
                txt = new TextBlock()
                {
                    Text = text,
                    Foreground = color,
                    RenderTransform = new ScaleTransform(1,-1)
                };
                StoreVector("text-" + id, txt);
                RadarCanvas.Children.Add(txt);
            }

            Canvas.SetTop(txt, y + 12);
            Canvas.SetLeft(txt, x + 10); 
            txt.Visibility = Visibility.Visible;

            return txt;
        }
        private void UpdateShapeData(UIElement shape, ClassifiedObject obj)
        {
            //this is ugly, can there be a solution using WPF MVVM Databinding?
            if (obj.obj.type == BotTypes.Housing && shape is ITimedObject)
            {
                houseScanner.ParseHouse(shape, (Housing)obj.obj);
            }
            else if (obj.obj.type == BotTypes.Player)
            {
                Player p = (Player)obj.obj;
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

                if (shape is ITurnable)
                {
                    ((ITurnable)shape).Turn(obj.obj.turnAngle);
                }
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
            DrawDot("center", 50, 50, Brushes.GhostWhite);
            DrawDot("2", 100, 100, Brushes.GhostWhite);
            DrawDot("3", 170, 170, Brushes.Green);
            DrawDot("4", 200, 200, Brushes.GhostWhite);
            DrawDot("5", 250, 250, Brushes.Red);
            
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
