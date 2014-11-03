﻿using ArcheBuddy.Bot.Classes;
using mCore.Radar;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mCore.Shapes
{
    /// <summary>
    /// Interaction logic for Tree.xaml
    /// </summary>
    public partial class Farm : Canvas, IHousing
    {
        public Farm()
        {
            InitializeComponent();
        }

        public void UpdateHouse(Housing hc, HouseStatus status)
        {
            SmallHouse.UpdateHouseVector(rect, Data, hc, status);
        }

    }
}
