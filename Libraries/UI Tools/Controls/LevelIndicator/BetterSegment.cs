﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI_Tools.LevelIndicator
{
    public class Segment : Control
    {
        static Segment()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Segment), new FrameworkPropertyMetadata(typeof(Segment)));
        }
    }
}
