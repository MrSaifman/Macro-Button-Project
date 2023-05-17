using System;
using System.Threading.Tasks;
using Windows_App_WinUI3;

namespace Windows_App_WinUI3
{
    public class LightingManager
    {
        private USBDeviceManager _deviceManager;

        public LightingManager(USBDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public async Task SetColor(byte red, byte green, byte blue)
        {
            byte reportId = 1; // Assuming color setting uses report id 1
            byte[] reportData = new byte[64];

            reportData[0] = red;
            reportData[1] = green;
            reportData[2] = blue;

            await _deviceManager.SendReport(reportId, reportData);
        }

        public async Task SetPattern(byte patternId)
        {
            byte reportId = 2; // Assuming pattern setting uses report id 2
            byte[] reportData = new byte[64];

            reportData[0] = patternId;

            await _deviceManager.SendReport(reportId, reportData);
        }
    }
}