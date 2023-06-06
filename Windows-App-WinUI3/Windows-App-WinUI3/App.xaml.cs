// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.UI.ViewManagement;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_App_WinUI3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>

        private Window m_window;
        private NamedPipeServerStream server;

        [DllImport("user32.dll")]
        static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);


        public App()
        {
            this.InitializeComponent();
            // Enable window transitions
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();

            // Dispose the named pipe server when the main window is closed
            m_window.Closed += (sender, e) => server?.Dispose();

            // Start the named pipe server in a separate task
            Task.Run(() => StartServer());
        }

        private void StartServer()
        {
            server = new NamedPipeServerStream("MyPipe");

            while (true)
            {
                server.WaitForConnection();

                using var reader = new StreamReader(server, leaveOpen: true);
                var message = reader.ReadLine();
                if (message == "show")
                {
                    // Show the main window
                    _ = m_window.DispatcherQueue.TryEnqueue(() =>
                    {
                        LaunchAndBringToForegroundIfNeeded();
                    });
                }

                server.Disconnect();
            }
        }

        private void LaunchAndBringToForegroundIfNeeded()
        {
            if (m_window == null)
            {
                m_window = new MainWindow();
                m_window.Activate();
            }
            else
            {
                m_window.Activate();

                // Get the window handle
                var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(m_window);

                // Use SetForegroundWindow to bring the window to the foreground
                SetForegroundWindow(windowHandle);
            }
        }



    }
}
