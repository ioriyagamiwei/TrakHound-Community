﻿// Copyright (c) 2017 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using TrakHound;

namespace TrakHound_Dashboard.Pages.About.License
{
    /// <summary>
    /// Interaction logic for Page.xaml
    /// </summary>
    public partial class Page : UserControl, IPage
    {
        public Page()
        {
            InitializeComponent();

            DataContext = this;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LICENSE.txt");
            if (File.Exists(path)) LicenseText = File.ReadAllText(path);

            PageContent = this;
        }

        public string Title { get { return "License"; } }

        public Uri Image { get { return new Uri("pack://application:,,,/TrakHound-Dashboard;component/Pages/About/License/Key_03.png"); } }

        public bool ZoomEnabled { get { return false; } }

        public void SetZoom(double zoomPercentage) { }

        public void Opened() { }
        public bool Opening() { return true; }

        public void Closed() { }
        public bool Closing() { return true; }

        public event SendData_Handler SendData;

        public void GetSentData(EventData data) { }


        public string LicenseText
        {
            get { return (string)GetValue(LicenseTextProperty); }
            set { SetValue(LicenseTextProperty, value); }
        }

        public static readonly DependencyProperty LicenseTextProperty =
            DependencyProperty.Register("LicenseText", typeof(string), typeof(Page), new PropertyMetadata(null));


        public object PageContent { get; set; }

    }
}
