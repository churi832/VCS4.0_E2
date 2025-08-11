using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Sineva.VHL.Library.InteropServices
{
    internal enum WH_HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }
    internal enum WM_HookMouse : int
    {
        WM_MOUSEMOVE = 0x200,
        WM_LBUTTONDOWN = 0x201,
        WM_LBUTTONUP = 0x202,
        WM_LBUTTONDBLCLK = 0x203,
        WM_RBUTTONDOWN = 0x204,
        WM_RBUTTONUP = 0x205,
        WM_RBUTTONDBLCLK = 0x206,
        WM_MBUTTONDOWN = 0x207,
        WM_MBUTTONUP = 0x208,
        WM_MBUTTONDBLCLK = 0x209,
        WM_MOUSEWHEEL = 0x020A,
 }
    internal enum HookKeyEvents
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        SKeyDown = 0x0104,
        SKeyUp = 0x0105
    }

    // KeyBoard Hooking from 'https://stackoverflow.com/questions/34281223/c-sharp-hook-global-keyboard-events-net-4-0'
    public partial class KeyboardHooking : IDisposable
    {
        #region Struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct KeyboardHookStruct
        {
            public Int32 vkCode;
            public Int32 scanCode;
            public Int32 flags;
            public Int32 time;
            public Int32 dwExtraInfo;
        }
        #endregion

        #region Enum
        #endregion

        #region Delegate
        public delegate int DelegateKeyboardHook(int keyCode, int wParam, int lParam);
        private DelegateKeyboardHook CallbackKeyboardHooking;

        public delegate void LocalKeyEventHandler(Keys key, bool Shift, bool Ctrl, bool Alt);
        public event LocalKeyEventHandler KeyDown;
        public event LocalKeyEventHandler KeyUp;
        #endregion

        #region Field
        private bool m_GlobalHook = false;
        private bool m_PreventUserClosing = true;
        private int m_HookId = 0;
        private bool m_IsFinalized = false;
        #endregion

        #region DLL Import
        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(WH_HookType idHook, DelegateKeyboardHook lpfn, int hInstance, int threadId);
        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetCurrentThreadId();
        [DllImport("user32.dll")]
        static public extern short GetKeyState(System.Windows.Forms.Keys nVirtKey);
        #endregion
        
        #region Construtor
        //Start hook
        public KeyboardHooking(bool globalHook = false, bool preventUserClosing = true)
        {
            this.m_GlobalHook = globalHook;
            this.m_PreventUserClosing = preventUserClosing;

            CallbackKeyboardHooking = new DelegateKeyboardHook(KeyboardHookProc);
            if(globalHook)
            {
                m_HookId = SetWindowsHookEx(WH_HookType.WH_KEYBOARD_LL, CallbackKeyboardHooking, 0, 0);
            }
            else
            {
                m_HookId = SetWindowsHookEx(WH_HookType.WH_KEYBOARD, CallbackKeyboardHooking, 0, GetCurrentThreadId());
            }
        }
        ~KeyboardHooking()
        {
            if(!m_IsFinalized)
            {
                if(m_HookId != 0)
                    UnhookWindowsHookEx(m_HookId);

                m_HookId = 0;
                m_IsFinalized = true;
            }
        }
        public void Dispose()
        {
            if(!m_IsFinalized)
            {
                if(m_HookId != 0)
                    UnhookWindowsHookEx(m_HookId);

                m_HookId = 0;
                m_IsFinalized = true;
            }
        }
        #endregion

        #region Event Handler Callback
        //The listener that will trigger events
        private int KeyboardHookProc(int keyCode, int wParam, int lParam)
        {
            if(keyCode < 0)
            {
                return CallNextHookEx(m_HookId, keyCode, wParam, lParam);
            }
            try
            {
                if(!m_GlobalHook)
                {
                    if(keyCode == 3)
                    {
                        Keys key = (Keys)wParam;
                        bool shift = GetShiftPressed();
                        bool ctrl = GetCtrlPressed();
                        bool alt = GetAltPressed();

                        if(m_PreventUserClosing)
                        {
                            bool keyPrevent = false;
                            keyPrevent |= key == Keys.F4 && (ctrl || alt);
                            keyPrevent |= key == Keys.Tab && ctrl;
                            if(keyPrevent)
                            {
                                return 1;
                            }
                        }

                        if(key == Keys.Tab && ctrl) return 1;

                        //IntPtr ptr = IntPtr.Zero;

                        int keyDownUp = lParam >> 30;
                        if(keyDownUp == 0)  // KeyDown
                        {
                            if(KeyDown != null) KeyDown(key, shift, ctrl, alt);
                        }
                        if(keyDownUp == -1) // KeyUp
                        {
                            if(KeyUp != null) KeyUp(key, shift, ctrl, alt);
                        }
                        //System.Diagnostics.Debug.WriteLine("Down: " + (Keys)W);
                    }
                }
                else
                {
                    HookKeyEvents kEvent = (HookKeyEvents)wParam;

                    Int32 vkCode = Marshal.ReadInt32((IntPtr)lParam); //Leser vkCode som er de første 32 bits hvor L peker.

                    if(kEvent != HookKeyEvents.KeyDown && kEvent != HookKeyEvents.KeyUp && kEvent != HookKeyEvents.SKeyDown && kEvent != HookKeyEvents.SKeyUp)
                    {
                    }
                    if(kEvent == HookKeyEvents.KeyDown || kEvent == HookKeyEvents.SKeyDown)
                    {
                        if(KeyDown != null) KeyDown((Keys)vkCode, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    if(kEvent == HookKeyEvents.KeyUp || kEvent == HookKeyEvents.SKeyUp)
                    {
                        if(KeyUp != null) KeyUp((Keys)vkCode, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                }
            }
            catch(Exception)
            {
                //Ignore all errors...
            }

            return CallNextHookEx(m_HookId, keyCode, wParam, lParam);
        }
        #endregion

        #region Method
        public static bool GetCapslock()
        {
            return Convert.ToBoolean(GetKeyState(System.Windows.Forms.Keys.CapsLock)) & true;
        }
        public static bool GetNumlock()
        {
            return Convert.ToBoolean(GetKeyState(System.Windows.Forms.Keys.NumLock)) & true;
        }
        public static bool GetScrollLock()
        {
            return Convert.ToBoolean(GetKeyState(System.Windows.Forms.Keys.Scroll)) & true;
        }
        public static bool GetShiftPressed()
        {
            int state = GetKeyState(System.Windows.Forms.Keys.ShiftKey);
            if(state > 1 || state < -1) return true;
            return false;
        }
        public static bool GetCtrlPressed()
        {
            int state = GetKeyState(System.Windows.Forms.Keys.ControlKey);
            if(state > 1 || state < -1) return true;
            return false;
        }
        public static bool GetAltPressed()
        {
            int state = GetKeyState(System.Windows.Forms.Keys.Menu);
            if(state > 1 || state < -1) return true;
            return false;
        }
        #endregion
    }

    // Mouse Hooking from 'https://support.microsoft.com/ko-kr/kb/318804'
    public class MouseHooking
    {
        #region Struct
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
        #endregion

        #region Delegate
        public delegate int DelegateMouseHookProc(int nCode, int wParam, IntPtr lParam);
        private DelegateMouseHookProc CallbackMouseHooking;

        public delegate void LocalMouseEventHandler(MouseEventArgs e);
        public event LocalMouseEventHandler MouseMove;
        public event LocalMouseEventHandler MouseClick;
        public event LocalMouseEventHandler MouseClickR;
        public event LocalMouseEventHandler MouseDblClick;
        public event LocalMouseEventHandler MouseDblClickR;
        #endregion

        #region Field
        private bool m_GlobalHook = false;
        private int m_HookId = 0;
        private bool m_IsFinalized = false;
        private POINT m_PointDownL = new POINT();
        private POINT m_PointDownR = new POINT();

        private bool m_LMousePushed = false;
        private bool m_RMousePushed = false;
        private bool m_LMouseDblClicked = false;
        private bool m_RMouseDblClicked = false;
        #endregion

        #region DLL Import
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(WH_HookType idHook, DelegateMouseHookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetCurrentThreadId();
        #endregion

        #region Constructor
        public MouseHooking(bool globalHook = false)
        {
            this.m_GlobalHook = globalHook;

            CallbackMouseHooking = new DelegateMouseHookProc(MouseHookProc);
            if(globalHook)
            {
                m_HookId = SetWindowsHookEx(WH_HookType.WH_MOUSE_LL, CallbackMouseHooking, (IntPtr)0, 0);
            }
            else
            {
                m_HookId = SetWindowsHookEx(WH_HookType.WH_MOUSE, CallbackMouseHooking, (IntPtr)0, GetCurrentThreadId());
            }
        }
        ~MouseHooking()
        {
            if(!m_IsFinalized)
            {
                if(m_HookId != 0)
                    UnhookWindowsHookEx(m_HookId);

                m_HookId = 0;
                m_IsFinalized = true;
            }
        }
        public void Dispose()
        {
            if(!m_IsFinalized)
            {
                if(m_HookId != 0)
                    UnhookWindowsHookEx(m_HookId);

                m_HookId = 0;
                m_IsFinalized = true;
            }
        }
        #endregion

        #region Event Handler Callback
        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            //Marshall the data from callback.
            MouseHookStruct mouse = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            if(nCode < 0)
            {
                return CallNextHookEx(m_HookId, nCode, wParam, lParam);
            }
            else
            {
                bool eventExist = false;
                eventExist |= MouseMove != null;
                eventExist |= MouseClick != null;
                eventExist |= MouseClickR != null;
                eventExist |= MouseDblClick != null;
                eventExist |= MouseDblClickR != null;
                if(eventExist)
                {
                    int delta = (mouse.hwnd >> 16) & 0xffff;
                    MouseEventArgs e = null;
                    switch(wParam)
                    {
                    case (int)WM_HookMouse.WM_MOUSEMOVE:
                        if(MouseMove != null)
                        {
                            e = new MouseEventArgs(MouseButtons.None, 0, mouse.pt.x, mouse.pt.y, delta);
                            MouseMove.Invoke(e);
                        }
                        break;
                    case (int)WM_HookMouse.WM_LBUTTONDOWN:
                        if(MouseClick != null || MouseDblClick != null)
                        {
                            m_PointDownL = mouse.pt;
                            m_LMousePushed = true;
                            m_LMouseDblClicked = false;
                        }
                        break;
                    case (int)WM_HookMouse.WM_LBUTTONUP:
                        if(MouseClick != null)
                        {
                            bool click = Math.Abs(m_PointDownL.x - mouse.pt.x) < 3;
                            click &= Math.Abs(m_PointDownL.y - mouse.pt.y) < 3;
                            if(click && m_LMousePushed)
                            {
                                e = new MouseEventArgs(MouseButtons.Left, 1, mouse.pt.x, mouse.pt.y, delta);
                                MouseClick.Invoke(e);
                            }
                            m_LMousePushed = false;
                        }
                        break;
                    case (int)WM_HookMouse.WM_LBUTTONDBLCLK:
                        if(MouseDblClick != null && !m_LMouseDblClicked)
                        {
                            m_LMousePushed = false;
                            m_LMouseDblClicked = true;
                            e = new MouseEventArgs(MouseButtons.Left, 2, mouse.pt.x, mouse.pt.y, delta);
                            MouseDblClick.Invoke(e);
                        }
                        break;
                    case (int)WM_HookMouse.WM_RBUTTONDOWN:
                        if(MouseClickR != null || MouseDblClickR != null)
                        {
                            m_PointDownR = mouse.pt;
                            m_RMousePushed = true;
                            m_RMouseDblClicked = false;
                        }
                        break;
                    case (int)WM_HookMouse.WM_RBUTTONUP:
                        if(MouseClickR != null)
                        {
                            bool click = Math.Abs(m_PointDownR.x - mouse.pt.x) < 3;
                            click &= Math.Abs(m_PointDownR.y - mouse.pt.y) < 3;
                            if(click && m_RMousePushed)
                            {
                                e = new MouseEventArgs(MouseButtons.Right, 1, mouse.pt.x, mouse.pt.y, delta);
                                MouseClickR.Invoke(e);
                            }
                            m_RMousePushed = false;
                        }
                        break;
                    case (int)WM_HookMouse.WM_RBUTTONDBLCLK:
                        if(MouseDblClickR != null && !m_RMouseDblClicked)
                        {
                            m_RMousePushed = false;
                            m_RMouseDblClicked = true;
                            e = new MouseEventArgs(MouseButtons.Right, 2, mouse.pt.x, mouse.pt.y, delta);
                            MouseDblClickR.Invoke(e);
                        }
                        break;
                    case (int)WM_HookMouse.WM_MBUTTONDOWN:
                        break;
                    case (int)WM_HookMouse.WM_MBUTTONUP:
                        break;
                    case (int)WM_HookMouse.WM_MBUTTONDBLCLK:
                        break;
                    case (int)WM_HookMouse.WM_MOUSEWHEEL:
                        break;
                    }
                }
                return CallNextHookEx(m_HookId, nCode, wParam, lParam);
            }
        }
        #endregion
    }


    //public class MouseHooking
    //{
    //    //public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
    //    public delegate int DelegateMouseHookProc(int nCode, int wParam, IntPtr lParam);

    //    //Declare hook handle as int.
    //    static int m_HookId = 0;
    //    private bool m_IsFinalized = false;

    //    //Declare mouse hook constant.
    //    //For other hook types, you can obtain these values from Winuser.h in Microsoft SDK.
    //    public const int WH_MOUSE = 7;

    //    //Declare MouseHookProcedure as HookProc type.
    //    static DelegateMouseHookProc MouseHookProcedure;

    //    //Declare wrapper managed POINT class.
    //    [StructLayout(LayoutKind.Sequential)]
    //    public class POINT
    //    {
    //        public int x;
    //        public int y;
    //    }

    //    //Declare wrapper managed MouseHookStruct class.
    //    [StructLayout(LayoutKind.Sequential)]
    //    public class MouseHookStruct
    //    {
    //        public POINT pt;
    //        public int hwnd;
    //        public int wHitTestCode;
    //        public int dwExtraInfo;
    //    }

    //    #region DLL Import
    //    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    //    private static extern int SetWindowsHookEx(HookType idHook, DelegateMouseHookProc lpfn, IntPtr hInstance, int threadId);
    //    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    //    private static extern bool UnhookWindowsHookEx(int idHook);
    //    //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    //    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    //    public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
    //    #endregion

    //    public static void Dispose()
    //    {
    //        UnsubscribeFromMouseEvents();
    //    }

    //    private static void EnsureSubscribedToMouseEvents()
    //    {
    //        if(m_HookId == 0)
    //        {
    //            // Create an instance of HookProc.
    //            MouseHookProcedure = new DelegateMouseHookProc(MouseHookProc);

    //            m_HookId = SetWindowsHookEx(HookType.WH_MOUSE, MouseHookProcedure, (IntPtr)0, AppDomain.GetCurrentThreadId());
    //            //If SetWindowsHookEx fails.
    //            if(m_HookId == 0)
    //            {
    //                MessageBox.Show("SetWindowsHookEx Failed");
    //                return;
    //            }
    //        }
    //    }
    //    public static void UnsubscribeFromMouseEvents()
    //    {
    //        if(m_HookId != 0)
    //        {
    //            bool ret = UnhookWindowsHookEx(m_HookId);
    //            //If UnhookWindowsHookEx fails.
    //            if(ret == false)
    //            {
    //                MessageBox.Show("UnhookWindowsHookEx Failed");
    //                return;
    //            }
    //            m_HookId = 0;
    //        }
    //    }

    //    //public static int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    //    public static int MouseHookProc(int nCode, int wParam, IntPtr lParam)
    //    {
    //        //Marshall the data from callback.
    //        MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

    //        if(nCode < 0)
    //        {
    //            return CallNextHookEx(m_HookId, nCode, wParam, lParam);
    //        }
    //        else
    //        {

    //            if(s_MouseClick != null)
    //            {
    //                int click = 0;
    //                MouseButtons button = (wParam == 0x201 || wParam == 0x202 || wParam == 0x203) ? MouseButtons.Left :
    //                                      (wParam == 0x204 || wParam == 0x205 || wParam == 0x206) ? MouseButtons.Right :
    //                                      (wParam == 0x207 || wParam == 0x208 || wParam == 0x209) ? MouseButtons.Middle : MouseButtons.None;
    //                switch(wParam)
    //                {
    //                case 0x201:
    //                case 0x202:
    //                case 0x204:
    //                case 0x205:
    //                case 0x207:
    //                case 0x208:
    //                    click = 1;
    //                    break;
    //                case 0x203:
    //                case 0x206:
    //                case 0x209:
    //                    click = 2;
    //                    break;
    //                }

    //                if(click > 0)
    //                {
    //                    int delta = (MyMouseHookStruct.hwnd >> 16) & 0xffff;
    //                    MouseEventArgs e = new MouseEventArgs(button, click, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, delta);
    //                    s_MouseClick.Invoke(null, e);
    //                }
    //                //MouseEventArgs e = new MouseEventArgs(
    //                //MouseEventExtArgs e = new MouseEventExtArgs(
    //                //               button,
    //                //               clickCount,
    //                //               mouseHookStruct.Point.X,
    //                //               mouseHookStruct.Point.Y,
    //                //               mouseDelta);


    //            }
    //            return CallNextHookEx(m_HookId, nCode, wParam, lParam);
    //        }
    //    }


    //    private static event MouseEventHandler s_MouseClick;
    //    public static event MouseEventHandler MouseClick
    //    {
    //        add
    //        {
    //            EnsureSubscribedToMouseEvents();
    //            s_MouseClick += value;
    //        }
    //        remove
    //        {
    //            s_MouseClick -= value;
    //            UnsubscribeFromMouseEvents();
    //        }
    //    }
    //}

}
