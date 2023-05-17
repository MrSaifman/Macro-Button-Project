using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Timers;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Windows_App_WinUI3
{
    public class USBDeviceManager
    {
        private HidDevice _device;
        private Timer deviceCheckTimer;

        // Define an event to signal the receipt of data
        public event Action<byte[]> DataReceived;

        public USBDeviceManager()
        {
            deviceCheckTimer = new Timer(1000); // Checks for device every second
            deviceCheckTimer.Elapsed += CheckDevice;
            deviceCheckTimer.Start();
        }

        public async void CheckDevice(object sender, ElapsedEventArgs e)
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
                _device = null;
            }
        }

        public async Task SendReport(byte reportId, byte[] reportData)
        {
            if (_device != null)
            {
                // Create the output report
                HidOutputReport report = _device.CreateOutputReport(reportId);

                // Check if buffer size is not bigger than report data capacity
                if (reportData.Length > report.Data.Capacity)
                {
                    throw new Exception("Report data is bigger than report capacity");
                }

                // Copy reportData to report buffer
                reportData.CopyTo(report.Data.ToArray(), 0);

                // Send the output report
                await _device.SendOutputReportAsync(report);
            }
        }

        private async void StartListeningForData()
        {
            if (_device != null)
            {
                // Start a task that will continuously read data
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        HidInputReport inputReport = await _device.GetInputReportAsync();

                        if (inputReport != null && inputReport.Data.Length > 0)
                        {
                            // Trigger the DataReceived event
                            DataReceived?.Invoke(inputReport.Data.ToArray());
                        }
                    }
                });
            }
        }
    }
}
