namespace codingfreaks.cfUtils.Logic.Screenshot
{
    using System;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Logic and data of a screenshot including the <see cref="GetAllWindows()"/> mehod.
    /// </summary>
    public class Screenshot
    {
        #region constants

        private const int GWL_EXSTYLE = -20;
        private const int LWA_ALPHA = 0x2;
        private const string PROGRAMMANAGER = "Program Manager";
        private const string RUNDLL = "RunDLL";
        private const uint WM_PAINT = 0x000F;
        private const int WS_EX_LAYERED = 0x80000;

        #endregion

        #region constructors and destructors

        private Screenshot(IntPtr hWnd, bool specialCapturing)
        {
            IsMinimized = NativeMethods.IsIconic(hWnd);
            Handle = hWnd;
            if (specialCapturing)
            {
                EnterSpecialCapturing(hWnd);
            }
            var wInfo = new Windowinfo
            {
                cbSize = Windowinfo.GetSize()
            };
            NativeMethods.GetWindowInfo(hWnd, ref wInfo);
            var isChild = false;
            var parent = NativeMethods.GetParent(hWnd);
            var pos = new Rectangle();
            var parentPos = new Rectangle();
            if (ForceMdiCapturing && parent != IntPtr.Zero && (wInfo.dwExStyle & ExtendedWindowStyles.WS_EX_MDICHILD) == ExtendedWindowStyles.WS_EX_MDICHILD)
            {
                var name = new StringBuilder();
                NativeMethods.GetClassName(parent, name, RUNDLL.Length + 1);
                if (name.ToString() != RUNDLL)
                {
                    isChild = true;
                    pos = GetWindowPlacement(hWnd);
                    NativeMethods.MoveWindow(hWnd, int.MaxValue, int.MaxValue, pos.Width, pos.Height, true);

                    NativeMethods.SetParent(hWnd, IntPtr.Zero);

                    parentPos = GetWindowPlacement(parent);
                }
            }
            var rect = GetWindowPlacement(hWnd);
            Size = rect.Size;
            Location = rect.Location;
            Text = GetWindowText(hWnd);
            Image = GetWindowImage(hWnd, Size);
            if (isChild)
            {
                NativeMethods.SetParent(hWnd, parent);
                var x = wInfo.rcWindow.Left - parentPos.X;
                var y = wInfo.rcWindow.Top - parentPos.Y;

                if ((wInfo.dwStyle & WindowStyles.WS_THICKFRAME) == WindowStyles.WS_THICKFRAME)
                {
                    x -= SystemInformation.Border3DSize.Width;
                    y -= SystemInformation.Border3DSize.Height;
                }

                NativeMethods.MoveWindow(hWnd, x, y, pos.Width, pos.Height, true);
            }

            if (specialCapturing)
            {
                ExitSpecialCapturing(hWnd);
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Get the collection of Snapshot instances fro all available windows
        /// </summary>
        /// <param name="minimized">Capture a window even it's Minimized</param>
        /// <param name="specialCapturring">use special capturing method to capture minmized windows</param>
        /// <returns>return collections of Snapshot instances</returns>
        public static ScreenshotCollection GetAllWindows(bool minimized, bool specialCapturring)
        {
            _snapshots = new ScreenshotCollection();
            _countMinimizedWindows = minimized; //set minimized flag capture
            _useSpecialCapturing = specialCapturring; //set specialcapturing flag
            EnumWindowsCallbackHandler callback = EnumWindowsCallback;
            NativeMethods.EnumWindows(callback, IntPtr.Zero);
            return new ScreenshotCollection(_snapshots.ToArray(), true);
        }

        /// <summary>
        /// Get the collection of Snapshot instances fro all available windows
        /// </summary>
        /// <returns>return collections of Snapshot instances</returns>
        public static ScreenshotCollection GetAllWindows()
        {
            return GetAllWindows(false, false);
        }

        /// <summary>
        /// Take a Snap from the specific Window
        /// </summary>
        /// <param name="hWnd">Handle of the Window</param>
        /// <param name="useSpecialCapturing">if you need to capture from the minimized windows set it true,otherwise false</param>
        /// <returns></returns>
        public static Screenshot GetSnapshot(IntPtr hWnd, bool useSpecialCapturing)
        {
            if (!useSpecialCapturing)
            {
                return new Screenshot(hWnd, false);
            }
            return new Screenshot(hWnd, NeedSpecialCapturing(hWnd));
        }

        /// <summary>
        /// Gets the Name and Handle of the Snapped Window
        /// </summary>
        /// <returns>The string representation of this type.</returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("Window Text: {0}, Handle: {1}", Text, Handle);

            return str.ToString();
        }

        private static void EnterSpecialCapturing(IntPtr hWnd)
        {
            if (XpAppearance.MinAnimate)
            {
                XpAppearance.MinAnimate = false;
                _minAnimateChanged = true;
            }
            _winLong = NativeMethods.GetWindowLong(hWnd, GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hWnd, GWL_EXSTYLE, _winLong | WS_EX_LAYERED);
            NativeMethods.SetLayeredWindowAttributes(hWnd, 0, 1, LWA_ALPHA);
            NativeMethods.ShowWindow(hWnd, ShowWindowEnum.Restore);
            NativeMethods.SendMessage(hWnd, WM_PAINT, 0, 0);
        }

        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            var specialCapturing = false;
            if (hWnd == IntPtr.Zero)
            {
                return false;
            }
            if (!NativeMethods.IsWindowVisible(hWnd))
            {
                return true;
            }
            if (!_countMinimizedWindows)
            {
                if (NativeMethods.IsIconic(hWnd))
                {
                    return true;
                }
            }
            else if (NativeMethods.IsIconic(hWnd) && _useSpecialCapturing)
            {
                specialCapturing = true;
            }
            if (GetWindowText(hWnd) == PROGRAMMANAGER)
            {
                return true;
            }
            _snapshots.Add(new Screenshot(hWnd, specialCapturing));
            return true;
        }

        private static void ExitSpecialCapturing(IntPtr hWnd)
        {
            NativeMethods.ShowWindow(hWnd, ShowWindowEnum.Minimize);
            NativeMethods.SetWindowLong(hWnd, GWL_EXSTYLE, _winLong);

            if (_minAnimateChanged)
            {
                XpAppearance.MinAnimate = true;
                _minAnimateChanged = false;
            }
        }

        private static Bitmap GetWindowImage(IntPtr hWnd, Size size)
        {
            try
            {
                if (size.IsEmpty || size.Height < 0 || size.Width < 0)
                {
                    return null;
                }
                var bmp = new Bitmap(size.Width, size.Height);
                var g = Graphics.FromImage(bmp);
                var dc = g.GetHdc();
                NativeMethods.PrintWindow(hWnd, dc, 0);
                g.ReleaseHdc();
                g.Dispose();
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        private static Rectangle GetWindowPlacement(IntPtr hWnd)
        {
            var rect = new Rect();
            NativeMethods.GetWindowRect(hWnd, ref rect);
            return rect;
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            int length = NativeMethods.GetWindowTextLength(hWnd) + 1;
            StringBuilder name = new StringBuilder(length);

            NativeMethods.GetWindowText(hWnd, name, length);

            return name.ToString();
        }

        private static bool NeedSpecialCapturing(IntPtr hWnd)
        {
            if (NativeMethods.IsIconic(hWnd))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region properties

        /// <summary>
        /// if is true ,will force the mdi child to be captured completely ,maybe incompatible with some programs
        /// </summary>
        public static bool ForceMdiCapturing { get; set; }

        /// <summary>
        /// Get Handle of Snapped Window
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// Get the Captured Image of the Window
        /// </summary>
        public Bitmap Image { get; }

        /// <summary>
        /// if the state of the window is minimized return true otherwise returns false
        /// </summary>
        public bool IsMinimized { get; private set; }

        /// <summary>
        /// Get Location of Snapped Window
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// Get Size of Snapped Window
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// Get Title of Snapped Window
        /// </summary>
        public string Text { get; }

        #endregion

        #region static fields

        [ThreadStatic]
        private static bool _countMinimizedWindows;

        [ThreadStatic]
        private static bool _minAnimateChanged;

        [ThreadStatic]
        private static ScreenshotCollection _snapshots;

        [ThreadStatic]
        private static bool _useSpecialCapturing;

        [ThreadStatic]
        private static int _winLong;

        #endregion
    }
}