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
using Microsoft.UI.Xaml.Shapes;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_App_WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        // Field to keep track of the currently selected navigation button (represented as a Grid).
        // This helps in applying and removing styles when a navigation button is selected.
        private Grid _selectedNavButton;

        public MainWindow()
        {
            this.InitializeComponent();

            // Set the dashboard button as the default selected navigation button on window load.
            NavButtonTapped(gridDashboard, null);
        }

        /// <summary>
        /// Event handler for when the mouse pointer enters a navigation button.
        /// Changes the appearance of the button to indicate a hover state.
        /// </summary>
        private void NavButtonEntered(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;

            // If the hovered grid is the currently selected one, do nothing.
            if (grid == _selectedNavButton) return;

            // Change the color of the Path (icon) and TextBlock (text) inside the grid to gray.
            // This provides a visual indication that the button is being hovered over.
            if (grid != null)
            {
                foreach (var child in grid.Children)
                {
                    if (child is StackPanel stackPanel)
                    {
                        foreach (var item in stackPanel.Children)
                        {
                            if (item is Microsoft.UI.Xaml.Shapes.Path path)
                            {
                                path.Fill = new SolidColorBrush(Colors.Gray);
                            }
                            else if (item is TextBlock textBlock)
                            {
                                textBlock.Foreground = new SolidColorBrush(Colors.Gray);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the mouse pointer exits a navigation button.
        /// Resets the appearance of the button unless it's the currently selected one.
        /// </summary>
        private void NavButtonExited(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;

            // If the hovered grid is the currently selected one, do nothing.
            if (grid == _selectedNavButton) return;

            // Change the color of the Path (icon) and TextBlock (text) inside the grid back to white.
            // This resets the hover effect when the mouse pointer leaves the button.
            if (grid != null)
            {
                foreach (var child in grid.Children)
                {
                    if (child is StackPanel stackPanel)
                    {
                        foreach (var item in stackPanel.Children)
                        {
                            if (item is Microsoft.UI.Xaml.Shapes.Path path)
                            {
                                path.Fill = new SolidColorBrush(Colors.White);
                            }
                            else if (item is TextBlock textBlock)
                            {
                                textBlock.Foreground = new SolidColorBrush(Colors.White);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when a navigation button is tapped/clicked.
        /// Updates the appearance of the tapped button and resets the appearance of the previously selected button.
        /// </summary>
        private void NavButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            Grid currentNavBtn = sender as Grid;

            // Reset the appearance of the previously selected grid (if any) to its default state.
            if (_selectedNavButton != null)
            {
                _selectedNavButton.Background = new SolidColorBrush(Colors.Transparent);
                foreach (var child in _selectedNavButton.Children)
                {
                    if (child is StackPanel stackPanel)
                    {
                        foreach (var item in stackPanel.Children)
                        {
                            if (item is Microsoft.UI.Xaml.Shapes.Path path)
                            {
                                path.Fill = new SolidColorBrush(Colors.White);
                            }
                            else if (item is TextBlock textBlock)
                            {
                                textBlock.Foreground = new SolidColorBrush(Colors.White);
                            }
                        }
                    }
                }
            }

            // Update the appearance of the newly tapped grid to indicate it's the currently selected button.
            if (currentNavBtn != null)
            {
                currentNavBtn.Background = new SolidColorBrush(Colors.White);
                foreach (var child in currentNavBtn.Children)
                {
                    if (child is StackPanel stackPanel)
                    {
                        foreach (var item in stackPanel.Children)
                        {
                            if (item is Microsoft.UI.Xaml.Shapes.Path path)
                            {
                                path.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 20, 20, 21)); // Hex Color #141415
                            }
                            else if (item is TextBlock textBlock)
                            {
                                textBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 20, 20, 21)); // Hex Color #141415
                            }
                        }
                    }
                }
            }

            // Update the _selectedNavButton field to the newly tapped grid.
            // This helps in tracking which navigation button is currently selected.
            _selectedNavButton = currentNavBtn;
        }


    }
}
