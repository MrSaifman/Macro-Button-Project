// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

// System namespaces
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Windows;

// Windows namespaces
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using WinRT.Interop;

// Microsoft namespaces
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Interop;
using Microsoft.UI.Windowing;
using Microsoft.Win32;

// Project specific namespaces
using Windows_App_WinUI3.FileHandlers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_App_WinUI3
{
    public sealed partial class MainWindow : Window
    {
        private const string DataDirectoryName = "Data";
        private const string BlacklistJson = "blacklist.json";
        private const string WhitelistJson = "whitelist.json";
        private const string TempIconFolder  = "TempIcons";
        private const string AppSettings = "AppSettings";
        private const string FocusedAppOnly = "FocusedAppOnly";

        // Variable to keep track of current lighting mode
        private const string IdleLighting = "IdleLighting";
        private const string LidLiftLighting = "LidLiftLighting";
        private const string ButtonPressLighting = "ButtonPressLighting";  
        private string currentLightingMode;

        private const string FrameColor1 = "FrameColor1";
        private const string ButtonColor1 = "ButtonColor1";
        private const string FrameColor2 = "FrameColor2";
        private const string ButtonColor2 = "ButtonColor2";
        private Button _lastClickedButton;

        private JsonManager jsonManager;
        private USBDeviceManager deviceManager;
        private Button _selectedNavigationButton;

        private AppWindow _appWindow;

        private SolidColorBrush _originalBackground;
        private SolidColorBrush _originalForeground;

        private Dictionary<Button, SolidColorBrush> _originalBackgrounds = new Dictionary<Button, SolidColorBrush>();
        private Dictionary<Button, SolidColorBrush> _originalForegrounds = new Dictionary<Button, SolidColorBrush>();

        public MainWindow()
        {
            this.InitializeComponent();
            InitializeManagers();
            InitializeApplication();
            InitializeOnClose();
        }

        private void OnClosed(object sender, WindowEventArgs e)
        {
            // on closed function
        }

        private void OnClosing(object sender, AppWindowClosingEventArgs e)
        {
            e.Cancel = true;  // Cancel close
            _appWindow.Hide();  // Hide the window
        }


        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(myWndId);
        }

        // Initialize managers for JSON and USB device
        private void InitializeManagers()
        {
            deviceManager = new USBDeviceManager();
            deviceManager.DataReceived += deviceManager.DeviceManager_DataReceived; // Subscribe to the event here
            deviceManager.OnRequestReceived += BulkSettings_Load; // Subscribe to the event here

            jsonManager = new JsonManager();
            jsonManager.EnsureDefaultSettingsExist();
            currentLightingMode = IdleLighting;
        }

        // Initialize application with json settings
        private void InitializeApplication()
        {
            PopulateProgramsListBox();
            PopulateBlackAndWhiteLists();

            Underline1.Visibility = Visibility.Visible;
            PopulateLightSettings(IdleLighting);

            string focusedAppOnlySetting = jsonManager.ReadSetting(AppSettings, FocusedAppOnly);
            GreetingToggleSwitch.IsOn = focusedAppOnlySetting == "True";
            FocusedAppToggleSwitch_Toggled(null, null);

            InitializeDevice();
        }

        // Initialize USB device
        private async void InitializeDevice()
        {
            await deviceManager.InitializeDeviceAsync();
        }

        private void InitializeOnClose()
        {
            this.Closed += OnClosed;
            _appWindow = GetAppWindowForCurrentWindow();
            _appWindow.Closing += OnClosing;
        }

        // Update UI based on selected button
        private void UpdateUIForSelectedButton(Button clickedButton, Grid screen)
        {
            // Remove selection effect from previously selected button
            if (_selectedNavigationButton != null)
            {
                _selectedNavigationButton.Background = _originalBackgrounds[_selectedNavigationButton];
                _selectedNavigationButton.Foreground = _originalForegrounds[_selectedNavigationButton];
                // Also reset the background color of the Grid that contains the previously selected button
                // Also change the background color of the Grid that contains the selected button
                ((Grid)((FrameworkElement)_selectedNavigationButton.Parent).Parent).Background = new SolidColorBrush(Colors.Transparent);

            }

            // Set the clicked button as the selected button
            _selectedNavigationButton = clickedButton;

            // Change the background and text color of the selected button
            _selectedNavigationButton.Background = new SolidColorBrush(Colors.Transparent); // Transparent
            _selectedNavigationButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x14, 0x14, 0x15)); // #141415

            // Also change the background color of the Grid that contains the selected button
            ((Grid)((FrameworkElement)_selectedNavigationButton.Parent).Parent).Background = new SolidColorBrush(Colors.White);

            // Reset all screens' visibility
            DashboardScreen.Visibility = Visibility.Collapsed;
            RageModeScreen.Visibility = Visibility.Collapsed;
            MacroModeScreen.Visibility = Visibility.Collapsed;
            LightingScreen.Visibility = Visibility.Collapsed;
            HelpScreen.Visibility = Visibility.Collapsed;
            SettingsScreen.Visibility = Visibility.Collapsed;
            screen.Visibility = Visibility.Visible;
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton == _selectedNavigationButton)
                return;

            // Store the original Background and Foreground
            _originalBackground = clickedButton.Background as SolidColorBrush;
            _originalForeground = clickedButton.Foreground as SolidColorBrush;

            if (_selectedNavigationButton == btnDashboard)
            {
                dashboardImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/house.png"));
            }
            else if (_selectedNavigationButton == btnRageMode)
            {
                rageModeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/vein_pop.png"));
            }
            else if (_selectedNavigationButton == btnMacroMode)
            {
                macroModeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/keyboard_key_f.png"));
            }
            else if (_selectedNavigationButton == btnLighting)
            {
                lightingImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/lightning.png"));
            }
            else if (_selectedNavigationButton == btnHelp)
            {
                helpImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/help_square.png"));
            }
            else if (_selectedNavigationButton == btnSettings)
            {
                settingsImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/gear.png"));
            }



            if (clickedButton == btnDashboard)
            {
                UpdateUIForSelectedButton(clickedButton, DashboardScreen);
                dashboardImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/house_press.png"));
            }
            else if (clickedButton == btnRageMode)
            {
                UpdateUIForSelectedButton(clickedButton, RageModeScreen);
                rageModeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/vein_pop_press.png"));
            }
            else if (clickedButton == btnMacroMode)
            {
                UpdateUIForSelectedButton(clickedButton, MacroModeScreen);
                macroModeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/keyboard_key_f_press.png"));
            }
            else if (clickedButton == btnLighting)
            {
                UpdateUIForSelectedButton(clickedButton, LightingScreen);
                lightingImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/lightning_press.png"));
            }
            else if (clickedButton == btnHelp)
            {
                UpdateUIForSelectedButton(clickedButton, HelpScreen);
                helpImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/help_square_press.png"));
            }
            else if (clickedButton == btnSettings)
            {
                UpdateUIForSelectedButton(clickedButton, SettingsScreen);
                settingsImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/gear_press.png"));
            }
        }

        private void NavButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Button pressedButton = sender as Button;

            // If the pressed button is not the currently selected button, change the text color
            if (pressedButton != _selectedNavigationButton)
            {
                VisualStateManager.GoToState(pressedButton, "Pressed", true);
            }
        }

        private void NavButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Button releasedButton = sender as Button;

            // If the released button is not the currently selected button, change the text color
            if (releasedButton != _selectedNavigationButton)
            {
                VisualStateManager.GoToState(releasedButton, "Released", true);
            }
        }



        private void FocusedAppToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (GreetingToggleSwitch.IsOn)
            {
                GreetingTextBlock.Text = "Only closes the focused app";
            }
            else
            {
                GreetingTextBlock.Text = "Closes applications based on whitelist";
            }

            // Update the JSON when the switch is toggled.
            jsonManager.UpdateSetting(AppSettings, FocusedAppOnly, GreetingToggleSwitch.IsOn.ToString());
        }

        private void NavButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button hoveredButton = sender as Button;

            // Store the original Background and Foreground for each button
            if (!_originalBackgrounds.ContainsKey(hoveredButton))
            {
                _originalBackgrounds[hoveredButton] = hoveredButton.Background as SolidColorBrush;
            }
            if (!_originalForegrounds.ContainsKey(hoveredButton))
            {
                _originalForegrounds[hoveredButton] = hoveredButton.Foreground as SolidColorBrush;
            }

            // If the hovered button is not the currently selected button, change the text color #a1a1a1
            if (hoveredButton != _selectedNavigationButton)
            {
                hoveredButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xa1, 0xa1, 0xa1));
            }
            else
            {
                // If the hovered button is the currently selected button, keep the text color as #141415
                hoveredButton.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x14, 0x14, 0x15));
            }

            if (hoveredButton != _selectedNavigationButton)
            {
                if (hoveredButton == btnDashboard)
                {
                    dashboardImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/house_hover.png"));
                }
                else if (hoveredButton == btnRageMode)
                {
                    rageModeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/vein_pop_hover.png"));
                }
                else if (hoveredButton == btnMacroMode)
                {
                    macroModeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/keyboard_key_f_hover.png"));
                }
                else if (hoveredButton == btnLighting)
                {
                    lightingImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/lightning_hover.png"));
                }
                else if (hoveredButton == btnHelp)
                {
                    helpImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/help_square_hover.png"));
                }
                else if (hoveredButton == btnSettings)
                {
                    settingsImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/gear_hover.png"));
                }
            }
        }

        private void NavButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button exitedButton = sender as Button;

            // If the exited button is not the currently selected button, change the text color back to white
            if (exitedButton != _selectedNavigationButton)
            {
                exitedButton.Foreground = new SolidColorBrush(Colors.White);
            }

            if(exitedButton != _selectedNavigationButton)
            {
                if (exitedButton == btnDashboard)
                {
                    dashboardImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/house.png"));
                }
                else if (exitedButton == btnRageMode)
                {
                    rageModeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/vein_pop.png"));
                }
                else if (exitedButton == btnMacroMode)
                {
                    macroModeImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/keyboard_key_f.png"));
                }
                else if (exitedButton == btnLighting)
                {
                    lightingImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/lightning.png"));
                }
                else if (exitedButton == btnHelp)
                {
                    helpImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/help_square.png"));
                }
                else if (exitedButton == btnSettings)
                {
                    settingsImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/gear.png"));
                }
            }   
        }


        private void InstalledProgramsButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<AppInformation> programs = new ObservableCollection<AppInformation>();
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                ProcessKeyForPrograms(key, programs);
            }
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                ProcessKeyForPrograms(key, programs);
            }
            ProgramsListBox.ItemsSource = programs;
        }
        
        private void ActiveProgramsButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<AppInformation> programs = new ObservableCollection<AppInformation>();
            ProcessKeyForRunningPrograms(programs);
            ProgramsListBox.ItemsSource = programs;
        }
        
        private void AddToBlacklist_Click(object sender, RoutedEventArgs e)
        {
            AppInformation selectedProgram = ProgramsListBox.SelectedItem as AppInformation;
            if (selectedProgram != null)
            {
                string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
                string dataDirectory = Path.Combine(projectDirectory, DataDirectoryName);
                string blacklistFilePath = Path.Combine(dataDirectory, BlacklistJson);

                // Ensure the Data directory exists
                Directory.CreateDirectory(dataDirectory);

                ObservableCollection<AppInformation> blacklist;

                // Read the existing blacklist file if it exists
                if (File.Exists(blacklistFilePath))
                {
                    string jsonString = File.ReadAllText(blacklistFilePath);
                    blacklist = JsonSerializer.Deserialize<ObservableCollection<AppInformation>>(jsonString);
                }
                else
                {
                    // If it doesn't exist, create a new list
                    blacklist = new ObservableCollection<AppInformation>();
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
            AppInformation selectedProgram = ProgramsListBox.SelectedItem as AppInformation;
            if (selectedProgram != null)
            {
                string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
                string dataDirectory = Path.Combine(projectDirectory, DataDirectoryName);
                string whitelistFilePath = Path.Combine(dataDirectory, WhitelistJson);

                // Ensure the Data directory exists
                Directory.CreateDirectory(dataDirectory);

                ObservableCollection<AppInformation> whitelist;

                // Read the existing whitelist file if it exists
                if (File.Exists(whitelistFilePath))
                {
                    string jsonString = File.ReadAllText(whitelistFilePath);
                    whitelist = JsonSerializer.Deserialize<ObservableCollection<AppInformation>>(jsonString);
                }
                else
                {
                    // If it doesn't exist, create a new list
                    whitelist = new ObservableCollection<AppInformation>();
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
        
        private void RemoveFromWhitelist_Click(object sender, RoutedEventArgs e)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, DataDirectoryName);
            string whitelistFilePath = Path.Combine(dataDirectory, WhitelistJson);

            // Get the selected program
            AppInformation selectedProgram = (AppInformation)WhiteListBox.SelectedItem;

            if (selectedProgram != null)
            {
                // Load the current whitelist
                string jsonString = File.ReadAllText(whitelistFilePath);
                List<AppInformation> whitelist = JsonSerializer.Deserialize<List<AppInformation>>(jsonString);

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
            string dataDirectory = Path.Combine(projectDirectory, DataDirectoryName);
            string blacklistFilePath = Path.Combine(dataDirectory, BlacklistJson);

            // Get the selected program
            AppInformation selectedProgram = (AppInformation)BlackListBox.SelectedItem;

            if (selectedProgram != null)
            {
                // Load the current blacklist
                string jsonString = File.ReadAllText(blacklistFilePath);
                List<AppInformation> blacklist = JsonSerializer.Deserialize<List<AppInformation>>(jsonString);

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
            if (_lastClickedButton != null)
            {
                _lastClickedButton.Background = new SolidColorBrush(Colors.Transparent);
                _lastClickedButton.Foreground = new SolidColorBrush(Colors.White);
            }

            var button = (Button)sender;
            button.Background = new SolidColorBrush(Colors.White);
            button.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x14, 0x14, 0x15)); // #141415
            _lastClickedButton = button;

            // Unsubscribe from the ColorChanged event before changing the color
            ColorPickerControl.ColorChanged -= ColorPickerControl_ColorChanged;

            ColorPickerControl.Color = ConvertStringToColor(jsonManager.ReadSetting(currentLightingMode, (string)button.Tag));

            // Resubscribe to the ColorChanged event after changing the color
            ColorPickerControl.ColorChanged += ColorPickerControl_ColorChanged;
        }

        private void Button_PointerEntered1(object sender, PointerRoutedEventArgs e)
        {
            var button = (Button)sender;
            button.Background = new SolidColorBrush(Colors.White);
            button.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x14, 0x14, 0x15)); // #141415
        }

        private void Button_PointerExited1(object sender, PointerRoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button != _lastClickedButton)
            {
                button.Background = new SolidColorBrush(Colors.Transparent);
                button.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private async void ColorPickerControl_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (currentLightingMode != null)
            {
                byte reportValue = GetReportForTag((string)_lastClickedButton.Tag);

                // Construct the report data
                byte[] reportData = new byte[]
                {
                    0x01,
                    ConvertLightModeToByte(currentLightingMode),
                    reportValue,
                    ColorPickerControl.Color.R,
                    ColorPickerControl.Color.G,
                    ColorPickerControl.Color.B
                };

                await deviceManager.ReadWriteToHidDevice(reportData);

                jsonManager.UpdateSetting(currentLightingMode, (string)_lastClickedButton.Tag, ColorPickerControl.Color.ToString());

            }
        }
        
        // private void ColorPickerControl_LostFocus(object sender, RoutedEventArgs e)
        // {
        //     ColorPickerControl.Visibility = Visibility.Collapsed;
        //     ColorPickerControl.ColorChanged -= ColorPickerControl_ColorChanged;
        //     // Update setting in JSON file
        //     if (_currentColorSelectionButton != null)
        //     {
        //         string settingName = _currentColorSelectionButton.Tag.ToString();
        //         jsonManager.UpdateSetting(jsonManager.currentLightingMode, settingName, ColorPickerControl.Color.ToString());
        //     }
        // }

        public async void Slider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (jsonManager == null)
            {
                // Initialize jsonManager here, or return if it's not supposed to be null at this point.
                return;
            }

            Slider slider = (Slider)sender;
            double value = slider.Value;

            string settingName = (string)slider.Tag;

            jsonManager.UpdateSetting(currentLightingMode, settingName, value.ToString());

            byte[] reportData;
            if (settingName == "Brightness")
            {
                reportData = new byte[]
                {
                    0x01, ConvertLightModeToByte(currentLightingMode), 0x02, 0x00, 0x00, Convert.ToByte(value)
                };
            }
            else if (settingName == "PatternSpeed")
            {
                reportData = new byte[]
                {
                    0x01, ConvertLightModeToByte(currentLightingMode), 0x01, 0x00, 0x00, Convert.ToByte(value)
                };
            }
            else
            {
                // Unknown slider setting. Do nothing or handle error appropriately.
                return;
            }

            await deviceManager.ReadWriteToHidDevice(reportData);
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Get the search term from the AutoSuggestBox
            string searchTerm = sender.Text.ToLowerInvariant();

            ObservableCollection<AppInformation> programs = new ObservableCollection<AppInformation>();

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

        private void ProcessKeyForPrograms(RegistryKey key, ObservableCollection<AppInformation> programs, string searchTerm = null)
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, DataDirectoryName);
            string tempDirectory = Path.Combine(dataDirectory, TempIconFolder);

            // Create the TempIcons directory if it doesn't exist
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }

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
                            // Check if the icon path has a ",0" at the end and remove it
                            if (iconPath != null && iconPath.EndsWith(",0"))
                            {
                                iconPath = iconPath.Remove(iconPath.Length - 2);
                            }

                            // Try to resolve the icon file path
                            if (!string.IsNullOrEmpty(iconPath))
                            {
                                iconPath = Path.GetFullPath(iconPath);
                            }

                            // Create a new Program object with the name, icon, and path of the installed program
                            BitmapImage icon = null;
                            if (!string.IsNullOrEmpty(iconPath))
                            {
                                if (Path.GetExtension(iconPath).ToLower() == ".exe")
                                {
                                    string iconFileName = name.Replace(" ", "_") + ".ico"; // Generate a file name based on the program name
                                    string tempIconPath = Path.Combine(tempDirectory, iconFileName);

                                    // Check if an icon for this program already exists
                                    if (!File.Exists(tempIconPath))
                                    {
                                        // If not, create a new icon
                                        // Extract the icon from the exe and save it into the temporary .ico file
                                        using (var fileStream = File.Create(tempIconPath))
                                        {
                                            Toolbelt.Drawing.IconExtractor.Extract1stIconTo(iconPath, fileStream);
                                        }
                                    }

                                    // Create a BitmapImage from the .ico file
                                    icon = new BitmapImage(new Uri(tempIconPath, UriKind.Absolute));
                                }
                                else
                                {
                                    // If the icon is not an .exe file, use the original icon path
                                    icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
                                }
                            }
                            programs.Add(new AppInformation(name, icon, appPath));
                        }
                    }
                    catch { }
                }
            }
        }

        //Used to remove the temp folder when its done. Need to implement this
        //DeleteTempIcons(tempDirectory);
        public void DeleteTempIcons(string tempDirectory)
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        private void PopulateProgramsListBox()
        {
            ObservableCollection<AppInformation> programs = new ObservableCollection<AppInformation>();

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

        private void PopulateBlackAndWhiteLists()
        {
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            string dataDirectory = Path.Combine(projectDirectory, DataDirectoryName);

            string blacklistFilePath = Path.Combine(dataDirectory, BlacklistJson);
            string whitelistFilePath = Path.Combine(dataDirectory, WhitelistJson);

            // Ensure the Data directory exists
            Directory.CreateDirectory(dataDirectory);

            if (File.Exists(blacklistFilePath))
            {
                string jsonString = File.ReadAllText(blacklistFilePath);
                BlackListBox.ItemsSource = JsonSerializer.Deserialize<ObservableCollection<AppInformation>>(jsonString);
            }

            if (File.Exists(whitelistFilePath))
            {
                string jsonString = File.ReadAllText(whitelistFilePath);
                WhiteListBox.ItemsSource = JsonSerializer.Deserialize<ObservableCollection<AppInformation>>(jsonString);
            }
        }
        
        // Populates the UI with the lighting settings for a given category.
        public void PopulateLightSettings(string lightingCategory)
        {
            // Validate the input
            List<string> validCategories = new List<string> { IdleLighting, "ButtonPressLighting", "LidLiftLighting" };
            if (!validCategories.Contains(lightingCategory))
            {
                throw new ArgumentException($"{lightingCategory} is not a valid lighting category. Valid categories are {string.Join(", ", validCategories)}");
            }

            // lighting settings
            var lightUpPattern = jsonManager.ReadSetting(lightingCategory, "LightUpPattern");
            var brightness = int.Parse(jsonManager.ReadSetting(lightingCategory, "Brightness"));
            var patternSpeed = int.Parse(jsonManager.ReadSetting(lightingCategory, "PatternSpeed"));

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

            if (_lastClickedButton != null)
            {
                _lastClickedButton.Background = new SolidColorBrush(Colors.Transparent);
                _lastClickedButton.Foreground = new SolidColorBrush(Colors.White);
            }

            FrameColor1_Btn.Background = new SolidColorBrush(Colors.White);
            FrameColor1_Btn.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x14, 0x14, 0x15)); // #141415
            _lastClickedButton = (Button)FrameColor1_Btn;

            // Unsubscribe from the ColorChanged event before changing the color
            ColorPickerControl.ColorChanged -= ColorPickerControl_ColorChanged;

            ColorPickerControl.Color = ConvertStringToColor(jsonManager.ReadSetting(lightingCategory, (string)FrameColor1_Btn.Tag));

            // Resubscribe to the ColorChanged event after changing the color
            ColorPickerControl.ColorChanged += ColorPickerControl_ColorChanged;

            //FrameColor1_Btn.Background = new SolidColorBrush(Colors.White);
            //FrameColor1_Btn.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x14, 0x14, 0x15)); // #141415
        }

        private void ProcessKeyForRunningPrograms(ObservableCollection<AppInformation> programs)
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        string name = process.ProcessName;
                        string appPath = process.MainModule.FileName;

                        // Create a new Program object with the name and path of the running program
                        programs.Add(new AppInformation(name, null, appPath));
                    }
                }
                catch { }
            }
        }

        private byte GetReportForTag(string tag)
        {
            switch (tag)
            {
                case FrameColor1: return 0x03;
                case FrameColor2: return 0x04;
                case ButtonColor1: return 0x05;
                case ButtonColor2: return 0x06;
                default: return 0x00; // Default case if no match
            }
        }

        private void LightingModeChange(object sender, RoutedEventArgs e)
        {
            // Reset all buttons to black and hide all underlines
            Button1.Foreground = new SolidColorBrush(Colors.White);
            Underline1.Visibility = Visibility.Collapsed;

            Button2.Foreground = new SolidColorBrush(Colors.White);
            Underline2.Visibility = Visibility.Collapsed;

            Button3.Foreground = new SolidColorBrush(Colors.White);
            Underline3.Visibility = Visibility.Collapsed;

            

            // Change the clicked button to orange and show the underline
            Button button = sender as Button;
            if (button != null)
            {
                if (button == Button1)
                {
                    Underline1.Visibility = Visibility.Visible;
                }
                else if (button == Button2)
                {
                    Underline2.Visibility = Visibility.Visible;
                }
                else if (button == Button3)
                {
                    Underline3.Visibility = Visibility.Visible;
                }

                if (jsonManager.lightingModeMapping.TryGetValue(button.Content.ToString(), out string mode))
                {
                    PopulateLightSettings(mode);
                    currentLightingMode = mode;
                }
            }
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                button.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && currentLightingMode != button.Content.ToString())
            {
                button.Foreground = new SolidColorBrush(Colors.White);
            }
        }


        public async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (jsonManager == null)
            {
                // Initialize jsonManager here, or return if it's not supposed to be null at this point.
                return;
            }

            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            string selectedPattern = (string)selectedItem.Content;

            jsonManager.UpdateSetting(currentLightingMode, "LightUpPattern", selectedPattern);

            byte patternValue = ConvertPatternToByte(selectedPattern);

            byte[] reportData = new byte[]
            {
                0x01, ConvertLightModeToByte(currentLightingMode), 0x00, 0x00, 0x00, patternValue
            };

            await deviceManager.ReadWriteToHidDevice(reportData);
        }

        /// <summary>
        /// Converts a pattern string to a numeric value representing the pattern for the USB.
        /// </summary>
        /// <param name="pattern">The pattern string.</param>
        /// <returns>The corresponding byte value representing the pattern.</returns>
        private byte ConvertPatternToByte(string pattern)
        {
            switch (pattern)
            {
                case "None": return 0;
                case "Static": return 1;
                case "Blink": return 2;
                case "Blink Between": return 3;
                case "Ease In": return 4;
                case "Ease Between": return 5;
                case "Rainbow Cycle": return 6;
                default: return 0;  // If pattern does not match any case, return 0 (default case)
            }
        }

        private byte ConvertLightModeToByte(string mode)
        {
            switch (mode)
            {
                case "IdleLighting": return 1;
                case "ButtonPressLighting": return 2;
                case "LidLiftLighting": return 3;
                default: return 0;  // If pattern does not match any case, return 0 (default case)
            }
        }

        /// <summary>
        /// Converts a hex color string to a Windows.UI.Color object.
        /// </summary>
        /// <param name="hex">The hex color string in the format "#AARRGGBB".</param>
        /// <returns>The corresponding Windows.UI.Color object.</returns>
        public static Windows.UI.Color ConvertStringToColor(string hex)
        {
            // Check if the hex string is null, empty, or has an incorrect length or format
            if (string.IsNullOrEmpty(hex) || hex.Length != 9 || !hex.StartsWith("#"))
            {
                throw new FormatException("The hex color string is not in the correct format.");
            }

            hex = hex.Substring(1);  // remove the hashtag

            // Extract the alpha, red, green, and blue components from the hex string
            var a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            var r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            var g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            var b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));

            // Create a Windows.UI.Color object with the extracted components and return it
            return Windows.UI.Color.FromArgb(a, r, g, b);
        }

        private byte[] ConvertStringToColorBytes(string color)
        {
            // Parse the color from the string
            var colorObject = System.Drawing.ColorTranslator.FromHtml(color);

            // Construct a byte array from the color's components
            byte[] colorBytes = new byte[]
            {
                colorObject.R, 
                colorObject.G, 
                colorObject.B
            };

            return colorBytes;
        }

        private async void BulkSettings_Load()
        {
            Debug.WriteLine("BulkSettings_Load invoked.");

            if (jsonManager == null)
            {
                // Initialize jsonManager here, or return if it's not supposed to be null at this point.
                return;
            }

            List<string> validCategories = new List<string> { IdleLighting, LidLiftLighting, ButtonPressLighting };

            List<byte> reportData = new List<byte>()
            {
                0x01, 0x04,
            };

            foreach (var category in validCategories)
            {
                var temp = jsonManager.ReadSetting(category, "LightUpPattern");
                var lightUpPattern = ConvertPatternToByte(temp);
                var brightness = byte.Parse(jsonManager.ReadSetting(category, "Brightness"));
                var patternSpeed = byte.Parse(jsonManager.ReadSetting(category, "PatternSpeed"));
                var frameColor1 = ConvertStringToColorBytes(jsonManager.ReadSetting(category, "FrameColor1"));
                var frameColor2 = ConvertStringToColorBytes(jsonManager.ReadSetting(category, "FrameColor2"));
                var buttonColor1 = ConvertStringToColorBytes(jsonManager.ReadSetting(category, "ButtonColor1"));
                var buttonColor2 = ConvertStringToColorBytes(jsonManager.ReadSetting(category, "ButtonColor2"));

                reportData.Add(lightUpPattern);
                reportData.Add(brightness);
                reportData.Add(patternSpeed);
                reportData.AddRange(frameColor1);
                reportData.AddRange(frameColor2);
                reportData.AddRange(buttonColor1);
                reportData.AddRange(buttonColor2);
            }

            await deviceManager.ReadWriteToHidDevice(reportData.ToArray());
        }
    }
}
