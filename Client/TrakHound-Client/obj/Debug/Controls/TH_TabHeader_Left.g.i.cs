﻿#pragma checksum "..\..\..\Controls\TH_TabHeader_Left.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0C19F9029064A654B719818814989E41"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TrakHound_Client.Controls;


namespace TrakHound_Client.Controls {
    
    
    /// <summary>
    /// TH_TabHeader_Left
    /// </summary>
    public partial class TH_TabHeader_Left : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 89 "..\..\..\Controls\TH_TabHeader_Left.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle content_image;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TrakHound-Client;component/controls/th_tabheader_left.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\TH_TabHeader_Left.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 42 "..\..\..\Controls\TH_TabHeader_Left.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.Border_MouseEnter);
            
            #line default
            #line hidden
            
            #line 42 "..\..\..\Controls\TH_TabHeader_Left.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.Border_MouseLeave);
            
            #line default
            #line hidden
            
            #line 42 "..\..\..\Controls\TH_TabHeader_Left.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Border_MouseUp);
            
            #line default
            #line hidden
            return;
            case 2:
            this.content_image = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 3:
            
            #line 124 "..\..\..\Controls\TH_TabHeader_Left.xaml"
            ((System.Windows.Controls.Border)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TabItemClose_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
