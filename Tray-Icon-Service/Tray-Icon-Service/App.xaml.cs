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
using System.Runtime.InteropServices;

namespace Tray_Icon_Service
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private NotifyIcon? notifyIcon;

        public class MyRenderer : ToolStripProfessionalRenderer
        {
            public MyRenderer() : base(new MyColors()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected)
                    base.OnRenderMenuItemBackground(e);
                else
                {
                    Rectangle rc = new Rectangle(System.Drawing.Point.Empty, e.Item.Size);
                    e.Graphics.FillRectangle(Brushes.Gray, rc);
                }
            }
        }

        public class MyColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(45, 45, 45); } // Change this to your desired color
            }
            public override Color MenuItemBorder
            {
                get { return Color.FromArgb(80, 80, 80); }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(35, 35, 35); } // Darker color for hover effect
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(35, 35, 35); } // Darker color for hover effect
            }
            public override Color MenuItemPressedGradientBegin
            {
                get { return Color.FromArgb(60, 60, 60); }
            }
            public override Color MenuItemPressedGradientEnd
            {
                get { return Color.FromArgb(60, 60, 60); }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return Color.FromArgb(45, 45, 48); }
            }
            public override Color ImageMarginGradientBegin
            {
                get { return Color.FromArgb(45, 45, 48); }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return Color.FromArgb(45, 45, 48); }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return Color.FromArgb(45, 45, 48); }
            }
        }


        public class CustomContextMenu : ContextMenuStrip
        {
            [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern long DwmSetWindowAttribute(IntPtr hwnd,
                                                            DWMWINDOWATTRIBUTE attribute,
                                                            ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                            uint cbAttribute);

            public CustomContextMenu()
            {
                var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;     //change as you want
                DwmSetWindowAttribute(Handle,
                                      DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                                      ref preference,
                                      sizeof(uint));
            }

            public enum DWMWINDOWATTRIBUTE
            {
                DWMWA_WINDOW_CORNER_PREFERENCE = 33
            }
            public enum DWM_WINDOW_CORNER_PREFERENCE
            {
                DWMWA_DEFAULT = 0,
                DWMWCP_DONOTROUND = 1,
                DWMWCP_ROUND = 2,
                DWMWCP_ROUNDSMALL = 3,
            }
        }

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new Icon("placeHolder.ico");
            notifyIcon.Visible = true;
            notifyIcon.MouseUp += NotifyIcon_MouseUp;

            // Create a context menu and add it to the notify icon
            var contextMenu = new CustomContextMenu(); // Use the custom context menu
            var showItem = new ToolStripMenuItem("Show");
            var exitItem = new ToolStripMenuItem("Exit");

            showItem.Click += (sender, args) => ShowMainWindow();
            exitItem.Click += (sender, args) => ExitApplication();

            contextMenu.Items.Add(showItem);
            contextMenu.Items.Add(exitItem);

            showItem.ForeColor = Color.White;
            exitItem.ForeColor = Color.White;
            contextMenu.Renderer = new MyRenderer();

            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void NotifyIcon_MouseUp(object? sender, MouseEventArgs e) // Changed from Click to MouseUp
        {
            if (e.Button == MouseButtons.Left) // Check if the left mouse button was clicked
            {
                using var client = new NamedPipeClientStream(".", "RageQuitPipe", PipeDirection.Out);
                client.Connect();
                using var writer = new StreamWriter(client);
                writer.Write("show");
            }
        }

        private void ShowMainWindow()
        {
            using var client = new NamedPipeClientStream(".", "RageQuitPipe", PipeDirection.Out);
            client.Connect();
            using var writer = new StreamWriter(client);
            writer.Write("show");
        }

        private void ExitApplication()
        {
            using var client = new NamedPipeClientStream(".", "RageQuitPipe", PipeDirection.Out);
            client.Connect();
            using var writer = new StreamWriter(client);
            writer.Write("quit");
            Current.Shutdown();
        }

        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
