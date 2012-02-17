namespace Bike.Installer.Custom
{
    using System;
    using System.Runtime.InteropServices;

    internal class Win32
    {
        private const int HWND_BROADCAST = 0xffff;
        private const int WM_SETTINGCHANGE = 0x001A;
        private const int SMTO_BLOCK = 0x0001;
        private const int SMTO_ABORTIFHUNG = 0x0002;
        private const int SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;

        public static void BroadCastSettingChange()
        {
            int result;
            SendMessageTimeout(
                (IntPtr)HWND_BROADCAST,
                WM_SETTINGCHANGE, 
                0, 
                "Environment", 
                SMTO_BLOCK | SMTO_ABORTIFHUNG | SMTO_NOTIMEOUTIFNOTHUNG, 
                5000, 
                out result);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SendMessageTimeout(
            IntPtr hWnd,
            int msg,
            int wParam,
            string lParam,
            int fuFlags,
            int uTimeout,
            out int lpdwResult
        );
    }
}
