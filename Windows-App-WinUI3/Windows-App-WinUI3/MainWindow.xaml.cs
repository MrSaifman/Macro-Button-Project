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
            }

            // Add selection effect to clicked button
            clickedButton.BorderBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x6b, 0x27));
            clickedButton.BorderThickness = new Thickness(5, 0, 0, 0);
            clickedButton.Foreground = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x6b, 0x27));

            // Set the clicked button as the selected button
            _selectedButton = clickedButton;
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
