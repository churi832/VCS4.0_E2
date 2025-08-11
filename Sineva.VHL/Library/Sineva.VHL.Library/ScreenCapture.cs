/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.11 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public class ScreenCapture
    {
        public static readonly ScreenCapture Instance = new ScreenCapture();

        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);

            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);

            return img;
        }

        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                Image img = CaptureWindow(handle);
                img.Save(filename, format);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }

        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                Image img = CaptureScreen();
                img.Save(filename, format);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }

        public void CaptureMouseClick(string filePath, string image_name)
        {
            System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
            try
            {
                if (image_name.Length > 30) image_name = image_name.Substring(0, 30);

                char[] banChars0 = Path.GetInvalidPathChars();
                for (int i = 0; i < banChars0.Length; i++) filePath.Replace(banChars0[i], '_');
                char[] banChars1 = Path.GetInvalidFileNameChars();
                for (int i = 0; i < banChars1.Length; i++) image_name.Replace(banChars1[i], '_');

                Bitmap bitmap = new Bitmap(200, 200);
                Graphics graphics = Graphics.FromImage(bitmap as Image);
                graphics.CopyFromScreen(Control.MousePosition.X - 100, Control.MousePosition.Y - 100, 0, 0, bitmap.Size);

                bitmap.Save(filePath + "\\" + image_name + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".jpg", ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(method, String.Format(ex.ToString()));
            }
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        }

    }
}
