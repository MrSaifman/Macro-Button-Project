using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Windows_App_WinUI3
{
    public class USBDeviceManager
    {
        private CloseHandler closeHandler = new CloseHandler();

        // Define your device VID and PID.
        private ushort vendorId = 1155;  // USBD_VID
        private ushort productId = 22555; // USBD_PID

        // UsagePage and UsageId can vary based on your device, using generic ones here.
        // In case of a specific device, these values should be provided by the device manufacturer.
        private ushort usagePage = 0xFF00;
        private ushort usageId = 0x0001;

        private HidDevice device;
        
        public event EventHandler<byte[]> DataReceived;
        private DeviceWatcher deviceWatcher;

        public async Task InitializeDeviceAsync()
        {
            string selector = HidDevice.GetDeviceSelector(usagePage, usageId, vendorId, productId);
            var devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Any())
            {
                // Device is available, you can communicate with it.
                device = await HidDevice.FromIdAsync(devices.ElementAt(0).Id, FileAccessMode.ReadWrite);

                if (device != null)
                {
                    device.InputReportReceived += Device_InputReportReceived;
                }
            }
            
            StartDeviceWatcher();
        }

        private void Device_InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            HidInputReport inputReport = args.Report;
            var buffer = inputReport.Data;

            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] inputBuffer = new byte[buffer.Length];
            reader.ReadBytes(inputBuffer);

            // Trigger the DataReceived event, passing the received data.
            DataReceived?.Invoke(this, inputBuffer);
        }

        public async Task ReadWriteToHidDevice(byte[] reportData)
        {
            if (device != null)
            {
                byte reportId = 1; // The report ID your device expects

                // Construct a HID output report to send to the device.
                HidOutputReport outReport = device.CreateOutputReport(reportId);

                // Initialize the data buffer and fill it in.
                byte[] buffer = new byte[outReport.Data.Capacity];

                // Fill the buffer with reportData.
                reportData.CopyTo(buffer, 0);

                // Set the data in the report.
                outReport.Data = buffer.AsBuffer();

                // Send the output report asynchronously.
                await device.SendOutputReportAsync(outReport);
            }
            else
            {
                // Device is NULL.
                // Handle this case as appropriate for your application.
            }
        }

        public void DeviceManager_DataReceived(object sender, byte[] data)
        {
            // This method will be called whenever data is received.
            // You can process the data here. For example:

            string dataString = BitConverter.ToString(data);

            // Log the data string or show it in your UI, etc...
            Debug.WriteLine("Data received: " + dataString);
            closeHandler = new CloseHandler();
            closeHandler.ForceCloseActiveWindow();
        }

        private void StartDeviceWatcher()
        {
            string selector = HidDevice.GetDeviceSelector(usagePage, usageId, vendorId, productId);
            deviceWatcher = DeviceInformation.CreateWatcher(selector);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;

            deviceWatcher.Start();
        }

        private void StopDeviceWatcher()
        {
            if (deviceWatcher.Status == DeviceWatcherStatus.Started)
            {
                deviceWatcher.Stop();
            }

            deviceWatcher.Added -= DeviceWatcher_Added;
            deviceWatcher.Removed -= DeviceWatcher_Removed;
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            if (device == null) // Check if device is already initialized
            {
                await InitializeDeviceAsync();
            }
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }

    }
}
