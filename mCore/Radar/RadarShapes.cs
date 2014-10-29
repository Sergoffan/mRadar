using mCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using mCore.Shapes;
using System.Windows.Controls;

namespace mCore.Radar
{
    public static class RadarShapes
    {
        public static UIElement Shape(this ObjectCategory self)
        {
            Canvas shape;
            switch (self)
            {
                case ObjectCategory.ThunderstruckTree: shape = new Thunderstruck(); break;

                case ObjectCategory.HarvestableTree: shape = new Tree(); break;

                case ObjectCategory.Uprootable: shape = new Up(); break;

                case ObjectCategory.ScarecrowGarden: shape = new Garden(); break;

                case ObjectCategory.SmallHousing: shape = new SmallHouse(); break;

                case ObjectCategory.Farmhouse: shape = new Farmhouse(); break;

                case ObjectCategory.HarvestablePlant: shape = new Leaf(); break;
                case ObjectCategory.ScarecrowFarm: shape = new Farm(); break;
                case ObjectCategory.HousingWorkbench: shape = new HousingWorkbench(); break;

                case ObjectCategory.FriendlyPlayer: shape = new PlayerShape(true); break;

                case ObjectCategory.EnemyPlayer: shape = new PlayerShape(false); break;

                case ObjectCategory.FriendlyNPC: shape = new NpcShape(true); break;

                case ObjectCategory.EnemyNPC: shape = new NpcShape(false); break;

                default:
                    Ellipse dot = DrawSimpleDot();
                    dot.Fill = self.Color();
                    return dot;       
            }

            //need to flip the shape because our canvas is inverted (due to game coordinate system)
            return Flip(shape);
        }

        public static UIElement Flip(UIElement c)
        {
            c.RenderTransform = new ScaleTransform(1, -1);
            return c;
        }
        public static Ellipse DrawSimpleDot()
        {
            Ellipse dot = new Ellipse();
            dot.Width = 6;
            dot.Height = 6;
            return dot;
        }
        /// <summary>
        /// Returns a Brush color that represents an ObjectCategory, used when drawing simple dots and text names
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Brush Color(this ObjectCategory self)
        {
            switch (self)
            {
                case ObjectCategory.HarvestableTree:
                    return Brushes.DarkSeaGreen;

                case ObjectCategory.HarvestablePlant:
                    return Brushes.SeaGreen;

                case ObjectCategory.Ore:
                    return Brushes.MediumSeaGreen;

                case ObjectCategory.EnemyPlayer:
                    return Brushes.Red;

                case ObjectCategory.EnemyNPC:
                    return Brushes.DarkRed;

                case ObjectCategory.FriendlyPlayer:
                    return Brushes.Green;
                case ObjectCategory.FriendlyNPC:
                    return Brushes.DarkGreen;

                default:
                    return Brushes.SlateGray;
            }
        }
    }
}
