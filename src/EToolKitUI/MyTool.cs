using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EToolKit
{
    public class MyTool
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(
           string lpClassName,
           string lpWindowName
       );

        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern int SetWindowText(
            IntPtr hwnd,
            string lpString
        );

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
    }
}
