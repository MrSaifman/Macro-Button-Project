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
            }
            else if (clickedButton == btnRageMode)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 1;
                rectMacroMode.Opacity = 0;
                rectLightning.Opacity = 0;
            }
            else if (clickedButton == btnMacroMode)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 0;
                rectMacroMode.Opacity = 1;
                rectLightning.Opacity = 0;
            }
            else if (clickedButton == btnLightning)
            {
                rectDashboard.Opacity = 0;
                rectRageMode.Opacity = 0;
                rectMacroMode.Opacity = 0;
                rectLightning.Opacity = 1;
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
    }
}
