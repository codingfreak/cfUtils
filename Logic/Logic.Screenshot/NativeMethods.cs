using System;
using System.Linq;

namespace s2.s2Utils.Logic.Screenshot
{
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Defines states for Spif.
    /// </summary>
    [Flags]
    public enum Spif
    {
        /// <summary>
        /// Writes the new system-wide parameter setting to the user profile.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// ??
        /// </summary>
        SPIF_UPDATEINIFILE = 0x01,

        /// <summary>
        /// Broadcasts the WM_SETTINGCHANGE message after updating the user profile.
        /// </summary>
        SPIF_SENDCHANGE = 0x02,

        /// <summary>
        /// Same as SPIF_SENDCHANGE.
        /// </summary>
        SPIF_SENDWININICHANGE = 0x02 
    }

    internal delegate bool EnumWindowsCallbackHandler(IntPtr hWnd, IntPtr lParam);

    /// <summary>
    /// Defines states for extended WindowStyles.
    /// </summary>
    [Flags]
    internal enum ExtendedWindowStyles : uint
    {
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_TRANSPARENT = 0x00000020,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_WINDOWEDGE = 0x00000100,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST)
    }

    /// <summary>
    /// Defines states for ShowWindow.
    /// </summary>
    internal enum ShowWindowEnum
    {
        Hide = 0,
        ShowNormal = 1,
        ShowMinimized = 2,
        ShowMaximized = 3,
        Maximize = 3,
        ShowNormalNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActivate = 7,
        ShowNoActivate = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimized = 11
    };

    /// <summary>
    /// Defines states for WindowStyles.
    /// </summary>
    [Flags]
    internal enum WindowStyles : uint
    {
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = 0x80000000,
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,

        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,

        WS_CAPTION = WS_BORDER | WS_DLGFRAME,
        WS_TILED = WS_OVERLAPPED,
        WS_ICONIC = WS_MINIMIZE,
        WS_SIZEBOX = WS_THICKFRAME,
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
        WS_CHILDWINDOW = WS_CHILD
    }

    /// <summary>
    /// Defines a rectangle.
    /// </summary>
    internal struct Rect
    {
        #region member vars

        private int _bottom;
        private int right;

        #endregion

        #region methods

        public static implicit operator Rectangle(Rect rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        #endregion

        #region properties

        public int Height
        {
            get
            {
                return _bottom > Top ? _bottom - Top : Top;
            }
        }

        public int Left { get; private set; }

        public int Top { get; private set; }

        public int Width
        {
            get
            {
                return right > Left ? right - Left : Left;
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines a window info.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Windowinfo
    {
        public uint cbSize;
        public Rect rcWindow;
        public readonly Rect rcClient;
        public readonly WindowStyles dwStyle;
        public readonly ExtendedWindowStyles dwExStyle;
        public readonly uint dwWindowStatus;
        public readonly uint cxWindowBorders;
        public readonly uint cyWindowBorders;
        public readonly ushort atomWindowType;
        public readonly ushort wCreatorVersion;

        public static uint GetSize()
        {
            return (uint)Marshal.SizeOf(typeof(Windowinfo));
        }
    }

    /// <summary>
    /// Contains unmanaged wrapper calls.
    /// </summary>
    internal static class NativeMethods
    {
        #region methods

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsCallbackHandler lpEnumFunc, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder name, int maxCount);

        [DllImport("user32")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref Windowinfo pwi);

        [DllImport("user32")]
        public static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32")]
        public static extern int GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);

        [DllImport("user32")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool reDraw);

        [DllImport("user32")]
        public static extern int PrintWindow(IntPtr hWnd, IntPtr dc, uint flags);

        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return")]
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "3")]
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "2")]
        [DllImport("user32.dll")]
        public static extern uint SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "2")]
        [SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "1")]
        [DllImport("user32")]
        public static extern int SetLayeredWindowAttributes(IntPtr hWnd, byte crey, byte alpha, int flags);

        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        [DllImport("user32")]
        public static extern int SetWindowLong(IntPtr hWnd, int index, int dwNewLong);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(XpAppearance.SPI uiAction, uint uiParam, ref Animationinfo pvParam, Spif fWinIni);

        #endregion
    }
}