﻿#pragma checksum "F:\Rage-Quit-Button-Project\Windows-App-WinUI3-old\Windows-App-WinUI3\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A36E950D090F21CFA37E1A038FEC0C3B13E49EBA6524328BE86700A1C2829EE2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Windows_App_WinUI3
{
    partial class MainWindow : 
        global::Microsoft.UI.Xaml.Window, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 1.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainWindow.xaml line 107
                {
                    this.DashboardScreen = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 3: // MainWindow.xaml line 122
                {
                    this.RageModeScreen = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 4: // MainWindow.xaml line 252
                {
                    this.MacroModeScreen = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 5: // MainWindow.xaml line 258
                {
                    this.LightingScreen = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 6: // MainWindow.xaml line 432
                {
                    this.HelpScreen = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 7: // MainWindow.xaml line 438
                {
                    this.SettingsScreen = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 8: // MainWindow.xaml line 414
                {
                    this.btnDurationSlider = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Slider>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Slider)this.btnDurationSlider).ValueChanged += this.Slider_ValueChanged;
                }
                break;
            case 9: // MainWindow.xaml line 398
                {
                    this.patternSlider = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Slider>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Slider)this.patternSlider).ValueChanged += this.Slider_ValueChanged;
                }
                break;
            case 10: // MainWindow.xaml line 382
                {
                    this.brightnessSlider = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Slider>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Slider)this.brightnessSlider).ValueChanged += this.Slider_ValueChanged;
                }
                break;
            case 11: // MainWindow.xaml line 352
                {
                    this.ColorPickerControl = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ColorPicker>(target);
                }
                break;
            case 12: // MainWindow.xaml line 348
                {
                    global::Microsoft.UI.Xaml.Controls.Button element12 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element12).Click += this.ColorButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)element12).PointerEntered += this.Button_PointerEntered1;
                    ((global::Microsoft.UI.Xaml.Controls.Button)element12).PointerExited += this.Button_PointerExited1;
                }
                break;
            case 13: // MainWindow.xaml line 344
                {
                    global::Microsoft.UI.Xaml.Controls.Button element13 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element13).Click += this.ColorButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)element13).PointerEntered += this.Button_PointerEntered1;
                    ((global::Microsoft.UI.Xaml.Controls.Button)element13).PointerExited += this.Button_PointerExited1;
                }
                break;
            case 14: // MainWindow.xaml line 340
                {
                    global::Microsoft.UI.Xaml.Controls.Button element14 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element14).Click += this.ColorButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)element14).PointerEntered += this.Button_PointerEntered1;
                    ((global::Microsoft.UI.Xaml.Controls.Button)element14).PointerExited += this.Button_PointerExited1;
                }
                break;
            case 15: // MainWindow.xaml line 336
                {
                    this.FrameColor1_Btn = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.FrameColor1_Btn).Click += this.ColorButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.FrameColor1_Btn).PointerEntered += this.Button_PointerEntered1;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.FrameColor1_Btn).PointerExited += this.Button_PointerExited1;
                }
                break;
            case 16: // MainWindow.xaml line 310
                {
                    this.lightUpPatternComboBox = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ComboBox>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ComboBox)this.lightUpPatternComboBox).SelectionChanged += this.ComboBox_SelectionChanged;
                }
                break;
            case 17: // MainWindow.xaml line 288
                {
                    this.Button3 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button3).Click += this.LightingModeChange;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button3).PointerEntered += this.Button_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button3).PointerExited += this.Button_PointerExited;
                }
                break;
            case 18: // MainWindow.xaml line 289
                {
                    this.Underline3 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Shapes.Rectangle>(target);
                }
                break;
            case 19: // MainWindow.xaml line 284
                {
                    this.Button2 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button2).Click += this.LightingModeChange;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button2).PointerEntered += this.Button_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button2).PointerExited += this.Button_PointerExited;
                }
                break;
            case 20: // MainWindow.xaml line 285
                {
                    this.Underline2 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Shapes.Rectangle>(target);
                }
                break;
            case 21: // MainWindow.xaml line 280
                {
                    this.Button1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button1).Click += this.LightingModeChange;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button1).PointerEntered += this.Button_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.Button1).PointerExited += this.Button_PointerExited;
                }
                break;
            case 22: // MainWindow.xaml line 281
                {
                    this.Underline1 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Shapes.Rectangle>(target);
                }
                break;
            case 23: // MainWindow.xaml line 236
                {
                    this.WhiteListBox = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ListBox>(target);
                }
                break;
            case 24: // MainWindow.xaml line 246
                {
                    global::Microsoft.UI.Xaml.Controls.Button element24 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element24).Click += this.RemoveFromWhitelist_Click;
                }
                break;
            case 26: // MainWindow.xaml line 179
                {
                    global::Microsoft.UI.Xaml.Controls.AutoSuggestBox element26 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.AutoSuggestBox>(target);
                    ((global::Microsoft.UI.Xaml.Controls.AutoSuggestBox)element26).TextChanged += this.SearchBox_TextChanged;
                }
                break;
            case 27: // MainWindow.xaml line 195
                {
                    this.ProgramsListBox = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ListBox>(target);
                }
                break;
            case 28: // MainWindow.xaml line 225
                {
                    global::Microsoft.UI.Xaml.Controls.Button element28 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element28).Click += this.AddToBlacklist_Click;
                }
                break;
            case 29: // MainWindow.xaml line 226
                {
                    global::Microsoft.UI.Xaml.Controls.Button element29 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element29).Click += this.AddToWhitelist_Click;
                }
                break;
            case 30: // MainWindow.xaml line 213
                {
                    global::Microsoft.UI.Xaml.Controls.Button element30 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element30).Click += this.InstalledProgramsButton_Click;
                }
                break;
            case 31: // MainWindow.xaml line 214
                {
                    global::Microsoft.UI.Xaml.Controls.Button element31 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element31).Click += this.ActiveProgramsButton_Click;
                }
                break;
            case 33: // MainWindow.xaml line 164
                {
                    this.BlackListBox = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ListBox>(target);
                }
                break;
            case 34: // MainWindow.xaml line 174
                {
                    global::Microsoft.UI.Xaml.Controls.Button element34 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)element34).Click += this.RemoveFromBlacklist_Click;
                }
                break;
            case 36: // MainWindow.xaml line 151
                {
                    this.GreetingToggleSwitch = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ToggleSwitch>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ToggleSwitch)this.GreetingToggleSwitch).Toggled += this.FocusedAppToggleSwitch_Toggled;
                }
                break;
            case 37: // MainWindow.xaml line 156
                {
                    this.GreetingTextBlock = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 38: // MainWindow.xaml line 54
                {
                    this.gridDashboard = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 39: // MainWindow.xaml line 62
                {
                    this.gridRageMode = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 40: // MainWindow.xaml line 70
                {
                    this.gridMacroMode = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 41: // MainWindow.xaml line 78
                {
                    this.gridLighting = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 42: // MainWindow.xaml line 86
                {
                    this.gridHelp = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 43: // MainWindow.xaml line 94
                {
                    this.gridSettings = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 44: // MainWindow.xaml line 99
                {
                    this.btnSettings = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnSettings).Click += this.NavButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnSettings).PointerEntered += this.NavButton_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnSettings).PointerExited += this.NavButton_PointerExited;
                }
                break;
            case 45: // MainWindow.xaml line 97
                {
                    this.settingsImage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                }
                break;
            case 46: // MainWindow.xaml line 91
                {
                    this.btnHelp = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnHelp).Click += this.NavButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnHelp).PointerEntered += this.NavButton_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnHelp).PointerExited += this.NavButton_PointerExited;
                }
                break;
            case 47: // MainWindow.xaml line 89
                {
                    this.helpImage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                }
                break;
            case 48: // MainWindow.xaml line 83
                {
                    this.btnLighting = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnLighting).Click += this.NavButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnLighting).PointerEntered += this.NavButton_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnLighting).PointerExited += this.NavButton_PointerExited;
                }
                break;
            case 49: // MainWindow.xaml line 81
                {
                    this.lightingImage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                }
                break;
            case 50: // MainWindow.xaml line 75
                {
                    this.btnMacroMode = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnMacroMode).Click += this.NavButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnMacroMode).PointerEntered += this.NavButton_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnMacroMode).PointerExited += this.NavButton_PointerExited;
                }
                break;
            case 51: // MainWindow.xaml line 73
                {
                    this.macroModeImage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                }
                break;
            case 52: // MainWindow.xaml line 67
                {
                    this.btnRageMode = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnRageMode).Click += this.NavButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnRageMode).PointerEntered += this.NavButton_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnRageMode).PointerExited += this.NavButton_PointerExited;
                }
                break;
            case 53: // MainWindow.xaml line 65
                {
                    this.rageModeImage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                }
                break;
            case 54: // MainWindow.xaml line 59
                {
                    this.btnDashboard = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnDashboard).Click += this.NavButton_Click;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnDashboard).PointerEntered += this.NavButton_PointerEntered;
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.btnDashboard).PointerExited += this.NavButton_PointerExited;
                }
                break;
            case 55: // MainWindow.xaml line 57
                {
                    this.dashboardImage = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Image>(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 1.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

