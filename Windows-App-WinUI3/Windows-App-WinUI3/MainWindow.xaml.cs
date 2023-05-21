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
using System.Text.Json;

using Windows_App_WinUI3.FileHandlers;
using System.Drawing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_App_WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private JsonManager jsonManager;
        private USBDeviceManager deviceManager;

        public MainWindow()
        {
            this.InitializeComponent();

            deviceManager = new USBDeviceManager();
            deviceManager.DataReceived += deviceManager.DeviceManager_DataReceived; // Subscribe to the event here

            jsonManager = new JsonManager();
            jsonManager.EnsureDefaultSettingsExist();
            jsonManager.currentLightingMode = "IdleLighting";
            
            PopulateProgramsListBox();
            PopulateBlackAndWhiteLists();
            PopulateLightSettings("IdleLighting");

            InitializeDevice();

        }

        private async void InitializeDevice()
        {
            await deviceManager.InitializeDeviceAsync();

            byte[] reportData = new byte[]
            {
                0x00, 0x02, 0x07, 0x06, 0xFF, 0xFF, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            };

            await deviceManager.ReadWriteToHidDevice(reportData);
        }

        private Button _selectedButton;
        private Button _currentColorButton;

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
                rectLighting.Opacity = 0;

                DashboardScreen.Visibility = Visibility.Visible;
                RageModeScreen.Visibility = Visibility.Collapsed;
                MacroModeScreen.Visibility = Visibility.Collapsed;
                LightingScreen.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == btnRageMode)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 1;
                rectMacroMode.Opacity = 0;
                rectLighting.Opacity = 0;

                DashboardScreen.Visibility = Visibility.Collapsed;
                RageModeScreen.Visibility = Visibility.Visible;
                MacroModeScreen.Visibility = Visibility.Collapsed;
                LightingScreen.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == btnMacroMode)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 0;
                rectMacroMode.Opacity = 1;
                rectLighting.Opacity = 0;

                DashboardScreen.Visibility = Visibility.Collapsed;
                RageModeScreen.Visibility = Visibility.Collapsed;
                MacroModeScreen.Visibility = Visibility.Visible;
                LightingScreen.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == btnLighting)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 0;
                rectMacroMode.Opacity = 0;
                rectLighting.Opacity = 1;

                DashboardScreen.Visibility = Visibility.Collapsed;
                RageModeScreen.Visibility = Visibility.Collapsed;
                MacroModeScreen.Visibility = Visibility.Collapsed;
                LightingScreen.Visibility = Visibility.Visible;
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
                hoveredButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xff, 0x6b, 0x27));
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
            public string Path { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                var program = (Program)obj;
                return Name == program.Name && Icon.UriSource == program.Icon.UriSource && Path == program.Path;
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Icon.UriSource, Path);
            }
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
                        string appPath = subKey.GetValue("InstallLocation") as string; // Get the install location

                        if (!string.IsNullOrEmpty(name) && (searchTerm == null || name.ToLowerInvariant().Contains(searchTerm)))
                        {
                            // Try to resolve the icon file path
                            if (!string.IsNullOrEmpty(iconPath))
                            {
                                iconPath = Path.GetFullPath(iconPath);
                            }

                            // Create a new Program object with the name, icon, and path of the installed program
                            BitmapImage icon = null;
                            if (!string.IsNullOrEmpty(iconPath))
                            {
                                icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
                            }
                            programs.Add(new Program { Name = name, Icon = icon, Path = appPath });
                        }
                    }
                    catch { }
                }
            }
        }

        private void AddToBlacklist_Click(object sender, RoutedEventArgs e)
        {
            Program selectedProgram = ProgramsListBox.SelectedItem as Program;
            if (selectedProgram != null)
            {
                string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
                string dataDirectory = Path.Combine(projectDirectory, "Data");
                string blacklistFilePath = Path.Combine(dataDirectory, "blacklist.json");

                // Ensure the Data directory exists
                Directory.CreateDirectory(dataDirectory);

                ObservableCollection<Program> blacklist;

                // Read the existing blacklist file if it exists
                if (File.Exists(blacklistFilePath))
                {
                    string jsonString = File.ReadAllText(blacklistFilePath);
                    blacklist = JsonSerializer.Deserialize<ObservableCollection<Program>>(jsonString);
                }
                else
                {
                    // If it doesn't exist, create a new list
                    blacklist = new ObservableCollection<Program>();
                }

                // Add the selected program to the list
                if (!blacklist.Any(p => p.Name == selectedProgram.Name))
                {
                    blacklist.Add(selectedProgram);
                }

                // Save the list back to the file
                string newJsonString = JsonSerializer.Serialize(blacklist);

                File.WriteAllText(blacklistFilePath, newJsonString);

                // Update the ListBox
                BlackListBox.ItemsSource = blacklist;
            }
        }

        private void AddToWhitelist_Click(object sender, RoutedEventArgs e)
        {
            Program selectedProgram = ProgramsListBox.SelectedItem as Program;
            if (selectedProgram != null)
            {
                string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
                string dataDirectory = Path.Combine(projectDirectory, "Data");
                string whitelistFilePath = Path.Combine(dataDirectory, "whitelist.json");

                // Ensure the Data directory exists
                Directory.CreateDirectory(dataDirectory);

                ObservableCollection<Program> whitelist;

                // Read the existing whitelist file if it exists
                if (File.Exists(whitelistFilePath))
                {
                    string jsonString = File.ReadAllText(whitelistFilePath);
                    whitelist = JsonSerializer.Deserialize<ObservableCollection<Program>>(jsonString);
                }
                else
                {
                    // If it doesn't exist, create a new list
                    whitelist = new ObservableCollection<Program>();
                }

                // Add the selected program to the list
                if (!whitelist.Any(p => p.Name == selectedProgram.Name))
                {
                    whitelist.Add(selectedProgram);
                }

                // Save the list back to the file
                string newJsonString = JsonSerializer.Serialize(whitelist);

                File.WriteAllText(whitelistFilePath, newJsonString);
                // Update the ListBox
                WhiteListBox.ItemsSource = whitelist;
            }
        }

        private void PopulateBlackAndWhiteLists()
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, "Data");

            string blacklistFilePath = Path.Combine(dataDirectory, "blacklist.json");
            string whitelistFilePath = Path.Combine(dataDirectory, "whitelist.json");

            // Ensure the Data directory exists
            Directory.CreateDirectory(dataDirectory);

            if (File.Exists(blacklistFilePath))
            {
                string jsonString = File.ReadAllText(blacklistFilePath);
                BlackListBox.ItemsSource = JsonSerializer.Deserialize<ObservableCollection<Program>>(jsonString);
            }

            if (File.Exists(whitelistFilePath))
            {
                string jsonString = File.ReadAllText(whitelistFilePath);
                WhiteListBox.ItemsSource = JsonSerializer.Deserialize<ObservableCollection<Program>>(jsonString);
            }
        }

        private void RemoveFromWhitelist_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, "Data");
            string whitelistFilePath = Path.Combine(dataDirectory, "whitelist.json");

            // Get the selected program
            Program selectedProgram = (Program)WhiteListBox.SelectedItem;

            if (selectedProgram != null)
            {
                // Load the current whitelist
                string jsonString = File.ReadAllText(whitelistFilePath);
                List<Program> whitelist = JsonSerializer.Deserialize<List<Program>>(jsonString);

                // Remove the selected program
                whitelist.RemoveAll(p => p.Name == selectedProgram.Name);

                // Save the list back to the file
                string newJsonString = JsonSerializer.Serialize(whitelist);
                File.WriteAllText(whitelistFilePath, newJsonString);

                // Update the ListBox
                WhiteListBox.ItemsSource = whitelist;
            }
        }

        private void RemoveFromBlacklist_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, "Data");
            string blacklistFilePath = Path.Combine(dataDirectory, "blacklist.json");

            // Get the selected program
            Program selectedProgram = (Program)BlackListBox.SelectedItem;

            if (selectedProgram != null)
            {
                // Load the current blacklist
                string jsonString = File.ReadAllText(blacklistFilePath);
                List<Program> blacklist = JsonSerializer.Deserialize<List<Program>>(jsonString);

                // Remove the selected program
                blacklist.RemoveAll(p => p.Name == selectedProgram.Name);

                // Save the list back to the file
                string newJsonString = JsonSerializer.Serialize(blacklist);
                File.WriteAllText(blacklistFilePath, newJsonString);

                // Update the ListBox
                BlackListBox.ItemsSource = blacklist;
            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            _currentColorButton = (Button)sender;
            ColorPickerControl.Color = ((SolidColorBrush)_currentColorButton.Background).Color;
            ColorPickerControl.ColorChanged += ColorPickerControl_ColorChanged;
            ColorPickerControl.Visibility = Visibility.Visible;
        }

        private void ColorPickerControl_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_currentColorButton != null)
            {
                _currentColorButton.Background = new SolidColorBrush(sender.Color);
            }
        }
        private void ColorPickerControl_LostFocus(object sender, RoutedEventArgs e)
        {
            ColorPickerControl.Visibility = Visibility.Collapsed;
            ColorPickerControl.ColorChanged -= ColorPickerControl_ColorChanged;
            // Update setting in JSON file
            if (_currentColorButton != null)
            {
                string settingName = _currentColorButton.Tag.ToString();
                jsonManager.UpdateSetting(jsonManager.currentLightingMode, settingName, ColorPickerControl.Color.ToString());
            }
        }

        private void LightingModeChange(object sender, RoutedEventArgs e)
        {
            // Reset all buttons to black
            Button1.Background = new SolidColorBrush(Colors.Black);
            Button2.Background = new SolidColorBrush(Colors.Black);
            Button3.Background = new SolidColorBrush(Colors.Black);

            // Change the clicked button to orange
            Button button = sender as Button;
            if (button != null)
            {
                button.Background = new SolidColorBrush(Colors.Orange);

                if (jsonManager.lightingModeMapping.TryGetValue(button.Content.ToString(), out string mode))
                {
                    PopulateLightSettings(mode);
                    jsonManager.currentLightingMode = mode;
                }
            }
        }

        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (jsonManager == null)
            {
                // Initialize jsonManager here, or return if it's not supposed to be null at this point.
                return;
            }

            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            string selectedPattern = (string)selectedItem.Content;

            jsonManager.LightUpPattern = selectedPattern;
            jsonManager.UpdateSetting(jsonManager.currentLightingMode, "LightUpPattern", selectedPattern);
        }

        public void Slider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (jsonManager == null)
            {
                // Initialize jsonManager here, or return if it's not supposed to be null at this point.
                return;
            }

            Slider slider = (Slider)sender;
            double value = slider.Value;

            string settingName = (string)slider.Tag;

            jsonManager.UpdateSetting(jsonManager.currentLightingMode, settingName, value.ToString());
        }

        public static Windows.UI.Color ConvertStringToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length != 9 || !hex.StartsWith("#"))
            {
                throw new FormatException("The hex color string is not in the correct format.");
            }

            hex = hex.Substring(1);  // remove the hashtag

            var a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            var r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            var g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            var b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            return Windows.UI.Color.FromArgb(a, r, g, b);
        }

        public void PopulateLightSettings(string lightingCategory)
        {
            // Validate the input
            List<string> validCategories = new List<string> { "IdleLighting", "ButtonPressLighting", "LidLiftLighting" };
            if (!validCategories.Contains(lightingCategory))
            {
                throw new ArgumentException($"{lightingCategory} is not a valid lighting category. Valid categories are {string.Join(", ", validCategories)}");
            }

            // lighting settings
            var lightUpPattern = jsonManager.ReadSetting(lightingCategory, "LightUpPattern");
            var brightness = int.Parse(jsonManager.ReadSetting(lightingCategory, "Brightness"));
            var patternSpeed = int.Parse(jsonManager.ReadSetting(lightingCategory, "PatternSpeed"));
            var frameColor1 = ConvertStringToColor(jsonManager.ReadSetting(lightingCategory, "FrameColor1"));
            var frameColor2 = ConvertStringToColor(jsonManager.ReadSetting(lightingCategory, "FrameColor2"));
            var buttonColor1 = ConvertStringToColor(jsonManager.ReadSetting(lightingCategory, "ButtonColor1"));
            var buttonColor2 = ConvertStringToColor(jsonManager.ReadSetting(lightingCategory, "ButtonColor2"));

            // Assign these values to the corresponding controls
            foreach (ComboBoxItem item in lightUpPatternComboBox.Items)
            {
                if (item.Content.ToString() == lightUpPattern)
                {
                    // Temporarily remove the event handler.
                    lightUpPatternComboBox.SelectionChanged -= ComboBox_SelectionChanged;

                    lightUpPatternComboBox.SelectedItem = item;

                    // Re-add the event handler.
                    lightUpPatternComboBox.SelectionChanged += ComboBox_SelectionChanged;

                    break;
                }
            }

            // Temporarily remove the event handlers for the sliders.
            brightnessSlider.ValueChanged -= Slider_ValueChanged;
            patternSlider.ValueChanged -= Slider_ValueChanged;

            brightnessSlider.Value = brightness;
            patternSlider.Value = patternSpeed;

            // Re-add the event handlers for the sliders.
            brightnessSlider.ValueChanged += Slider_ValueChanged;
            patternSlider.ValueChanged += Slider_ValueChanged;

            FrameColorButton1.Background = new SolidColorBrush(frameColor1);
            FrameColorButton2.Background = new SolidColorBrush(frameColor2);
            ButtonColorButton1.Background = new SolidColorBrush(buttonColor1);
            ButtonColorButton2.Background = new SolidColorBrush(buttonColor2);
        }

    }
}
