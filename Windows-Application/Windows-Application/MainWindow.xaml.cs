using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Timers;

namespace Windows_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HidDevice _device;
        private Timer deviceCheckTimer;

        public MainWindow()
        {
            InitializeComponent();
            deviceCheckTimer = new Timer(1000); // Checks for device every second
            deviceCheckTimer.Elapsed += CheckDevice;
            deviceCheckTimer.Start();
        }

        private async void CheckDevice(object sender, ElapsedEventArgs e)
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                ushort vendorId = 1155; // Replace with your HID device's vendor id
                ushort productId = 22555; // Replace with your HID device's product id
                ushort usagePage = 0xFF00; // Replace with your HID device's usage page
                ushort usageId = 0x01; // Replace with your HID device's usage id

                string selector = HidDevice.GetDeviceSelector(usagePage, usageId, vendorId, productId);
                var devices = await DeviceInformation.FindAllAsync(selector);

                if (devices.Count > 0)
                {
                    _device = await HidDevice.FromIdAsync(devices[0].Id, FileAccessMode.ReadWrite);
                    DeviceStatusTextBlock.Text = "BUTTON CONNECTED";
                    DeviceStatusTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    _device = null;
                    DeviceStatusTextBlock.Text = "BUTTON DISCONNECTED";
                    DeviceStatusTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 128, 0));
                }
            });
        }

        private async void InitializeDeviceAsync()
        {
            ushort vendorId = 1155; // Replace with your HID device's vendor id
            ushort productId = 22555; // Replace with your HID device's product id
            ushort usagePage = 0xFF00; // Replace with your HID device's usage page
            ushort usageId = 0x01; // Replace with your HID device's usage id

            string selector = HidDevice.GetDeviceSelector(usagePage, usageId, vendorId, productId);
            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Count > 0)
            {
                _device = await HidDevice.FromIdAsync(devices[0].Id, FileAccessMode.ReadWrite);
            }
            else
            {
                // Handle the case where the device was not found
                MessageBox.Show("Device not found");
            }
        }

        private async void SendReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (_device != null)
            {
                byte reportId = 1; // the report ID your device expects
                byte[] reportData = new byte[64]; // the actual data
                reportData[1] = (byte)0x02;
                reportData[2] = (byte)0x07;
                reportData[3] = (byte)0x02;
                reportData[4] = (byte)0x00;
                reportData[5] = (byte)0xFF;
                reportData[6] = (byte)0x00;
                //for (int i = 1; i < reportData.Length; i++)
                //{
                //    reportData[i] = (byte)(i);
                //}
                // Create the output report
                HidOutputReport report = _device.CreateOutputReport(reportId);

                // Initialize the data buffer and fill it in  
                byte[] buffer = new byte[report.Data.Capacity];

                // Fill the buffer with reportData
                reportData.CopyTo(buffer, 0);

                // Set the data in the report
                report.Data = buffer.AsBuffer();

                // Send the output report
                await _device.SendOutputReportAsync(report);

            }
        }
        private void RageSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Rage Settings
        }

        private void LightningButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Lightning
        }

        private void MacroButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Macro
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Settings
        }
    }
}
