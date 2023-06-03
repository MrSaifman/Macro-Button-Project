using System;
using System.Threading.Tasks;
using Windows_App_WinUI3;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Windows_App_WinUI3.FileHandlers;
using System.Linq;

namespace Windows_App_WinUI3
{
    public class CloseHandler
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public void PrintActiveWindow()
        {
            string activeWindow = GetActiveWindow();
            if (activeWindow == null)
            {
                Debug.WriteLine("Active window is null");
                return;
            }

            activeWindow = activeWindow.ToLower();
            JsonManager jsonManager = new JsonManager();
            List<Application> blacklist = jsonManager.ReadApplicationList("blacklist.json");
            List<Application> whitelist = jsonManager.ReadApplicationList("whitelist.json");

            if (blacklist.Any(app => activeWindow.Contains(app.Name.ToLower()) || app.Name.ToLower().Contains(activeWindow)))
            {
                Debug.WriteLine("Active window is in the blacklist");
            }
            else
            {
                Debug.WriteLine("Active window is not in the blacklist");
            }

            if (whitelist.Any(app => activeWindow.Contains(app.Name.ToLower()) || app.Name.ToLower().Contains(activeWindow)))
            {
                Debug.WriteLine("Active window is in the whitelist");
            }
            else
            {
                Debug.WriteLine("Active window is not in the whitelist");
            }
        }




        public void ForceCloseActiveWindow()
        {
            IntPtr handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out uint processId);

            if (processId != 0)
            {
                Process process = Process.GetProcessById((int)processId);
                if (process != null)
                {
                    process.Kill();
                }
            }
        }

        public string GetActiveWindow()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                uint processId;
                GetWindowThreadProcessId(handle, out processId);
                Process process = Process.GetProcessById((int)processId);

                if (process != null)
                {
                    return process.ProcessName;
                }
            }

            return null;
        }
    }
}