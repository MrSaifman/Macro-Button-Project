using System;
using System.Threading.Tasks;
using Windows_App_WinUI3;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                Debug.WriteLine("Active Window: " + Buff.ToString());
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
    }
}