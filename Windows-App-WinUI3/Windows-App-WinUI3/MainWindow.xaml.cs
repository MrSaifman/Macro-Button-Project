// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

using System.Windows;
using Microsoft.UI.Xaml.Interop;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Win32;
using Windows.Storage;

using System.Diagnostics;
using System.Collections.ObjectModel;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_App_WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            PopulateProgramsListBox();
        }


        private Button _selectedButton;

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            // Remove selection effect from previously selected button
            if (_selectedButton != null)
            {
                _selectedButton.BorderBrush = new SolidColorBrush(Colors.Transparent);
                _selectedButton.Foreground = new SolidColorBrush(Colors.White);
                _selectedButton.BorderThickness = new Thickness(0);
            }

            // Set the clicked button as the selected button
            _selectedButton = clickedButton;

            // Show the rectangle corresponding to the clicked button, and lower the opacity of the others
            if (clickedButton == btnDashboard)
            {
                rectDashboard.Opacity = 1;
                rectRageMode.Opacity = 0;
                rectMacroMode.Opacity = 0;
                rectLightning.Opacity = 0;

                DashboardScreen.Visibility = Visibility.Visible;
                RageModeScreen.Visibility = Visibility.Collapsed;
                MacroModeScreen.Visibility = Visibility.Collapsed;
                LightningScreen.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == btnRageMode)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 1;
                rectMacroMode.Opacity = 0;
                rectLightning.Opacity = 0;

                DashboardScreen.Visibility = Visibility.Collapsed;
                RageModeScreen.Visibility = Visibility.Visible;
                MacroModeScreen.Visibility = Visibility.Collapsed;
                LightningScreen.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == btnMacroMode)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 0;
                rectMacroMode.Opacity = 1;
                rectLightning.Opacity = 0;

                DashboardScreen.Visibility = Visibility.Collapsed;
                RageModeScreen.Visibility = Visibility.Collapsed;
                MacroModeScreen.Visibility = Visibility.Visible;
                LightningScreen.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == btnLightning)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 0;
                rectMacroMode.Opacity = 0;
                rectLightning.Opacity = 1;

                DashboardScreen.Visibility = Visibility.Collapsed;
                RageModeScreen.Visibility = Visibility.Collapsed;
                MacroModeScreen.Visibility = Visibility.Collapsed;
                LightningScreen.Visibility = Visibility.Visible;
            }
        }

        private void GreetingToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (GreetingToggleSwitch.IsOn)
            {
                GreetingTextBlock.Text = "Disable Current Application Close";
            }
            else
            {
                GreetingTextBlock.Text = "Enable Current Application Close";
            }
        }

        private void NavButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button hoveredButton = sender as Button;

            // Change the button text color to orange if it's not the selected button
            if (_selectedButton != hoveredButton)
            {
                hoveredButton.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x6b, 0x27));
            }
        }

        private void NavButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button hoveredButton = sender as Button;

            // Change the button text color to white if it's not the selected button
            if (_selectedButton != hoveredButton)
            {
                hoveredButton.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private async void PopulateProgramsListBox()
        {
            ObservableCollection<Program> programs = new ObservableCollection<Program>();

            // Define the uninstall keys
            string[] uninstallKeys =
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            // Check both HKEY_LOCAL_MACHINE and HKEY_CURRENT_USER
            RegistryKey[] baseKeys =
            {
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64),
                RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
            };

            foreach (var baseKey in baseKeys)
            {
                foreach (var keyPath in uninstallKeys)
                {
                    using (RegistryKey key = baseKey.OpenSubKey(keyPath))
                    {
                        if (key != null)
                        {
                            ProcessKeyForPrograms(key, programs);
                        }
                    }
                }
            }

            ProgramsListBox.ItemsSource = programs;
        }

        public class Program
        {
            public string Name { get; set; }
            public BitmapImage Icon { get; set; }
        }

        private async void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Get the search term from the AutoSuggestBox
            string searchTerm = sender.Text.ToLowerInvariant();

            ObservableCollection<Program> programs = new ObservableCollection<Program>();

            // Define the uninstall keys
            string[] uninstallKeys =
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            // Check both HKEY_LOCAL_MACHINE and HKEY_CURRENT_USER
            RegistryKey[] baseKeys =
            {
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64),
                RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)
            };

            foreach (var baseKey in baseKeys)
            {
                foreach (var keyPath in uninstallKeys)
                {
                    using (RegistryKey key = baseKey.OpenSubKey(keyPath))
                    {
                        if (key != null)
                        {
                            ProcessKeyForPrograms(key, programs, searchTerm);
                        }
                    }
                }
            }

            // Set the ItemsSource of the ListBox to the filtered list of programs
            ProgramsListBox.ItemsSource = programs;
        }

        private void ProcessKeyForPrograms(RegistryKey key, ObservableCollection<Program> programs, string searchTerm = null)
        {
            foreach (string subKeyName in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                {
                    try
                    {
                        string name = subKey.GetValue("DisplayName") as string;
                        string iconPath = subKey.GetValue("DisplayIcon") as string;

                        if (!string.IsNullOrEmpty(name) && (searchTerm == null || name.ToLowerInvariant().Contains(searchTerm)))
                        {
                            // Try to resolve the icon file path
                            if (!string.IsNullOrEmpty(iconPath))
                            {
                                iconPath = Path.GetFullPath(iconPath);
                            }

                            // Create a new Program object with the name and icon of the installed program
                            BitmapImage icon = null;
                            if (!string.IsNullOrEmpty(iconPath))
                            {
                                icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
                            }
                            programs.Add(new Program { Name = name, Icon = icon });
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
