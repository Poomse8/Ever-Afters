﻿#pragma checksum "D:\Nicolas\Documents\HOWEST - 3NMCT\Semester 5\S5 - Startup\Ever-Afters\Ever Afters\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DE7DCD9D5D6AEB6FAA167028DB54C36F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ever_Afters
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)(target);
                    #line 8 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Page)element1).KeyDown += this.Page_KeyDown;
                    #line default
                }
                break;
            case 2:
                {
                    global::Windows.UI.Xaml.Controls.Grid element2 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 10 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)element2).PointerReleased += this.OnPointerReleased;
                    #line default
                }
                break;
            case 3:
                {
                    this.txtError = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4:
                {
                    this.mediaPlayer = (global::Windows.UI.Xaml.Controls.MediaElement)(target);
                    #line 28 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.MediaElement)this.mediaPlayer).CurrentStateChanged += this.state;
                    #line default
                }
                break;
            case 5:
                {
                    this.btnReader0 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 32 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnReader0).Click += this.btnReader_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.btnReader1 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 33 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnReader1).Click += this.btnReader_Click;
                    #line default
                }
                break;
            case 7:
                {
                    this.btnReader2 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 34 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnReader2).Click += this.btnReader_Click;
                    #line default
                }
                break;
            case 8:
                {
                    this.btnReader3 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 35 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnReader3).Click += this.btnReader_Click;
                    #line default
                }
                break;
            case 9:
                {
                    this.btnReader4 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 36 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnReader4).Click += this.btnReader_Click;
                    #line default
                }
                break;
            case 10:
                {
                    this.txtNfcInput = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

