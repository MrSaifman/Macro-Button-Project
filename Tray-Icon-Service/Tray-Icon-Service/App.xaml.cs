using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO.Pipes;

namespace Tray_Icon_Service
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private NotifyIcon? notifyIcon;

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("placeHolder.ico");
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            // Create a context menu and add it to the notify icon
            var contextMenu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show");
            var exitItem = new ToolStripMenuItem("Exit");

            showItem.Click += (sender, args) => ShowMainWindow();
            exitItem.Click += (sender, args) => ExitApplication();

            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(exitItem);

            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            using var client = new NamedPipeClientStream(".", "MyPipe", PipeDirection.Out);
            client.Connect();
            using var writer = new StreamWriter(client);
            writer.Write("show");
        }

        private void ShowMainWindow()
        {
            // TODO: Show your WinUI 3 application
        }

        private void ExitApplication()
        {
            // TODO: Exit your application
            Current.Shutdown();
        }

        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
