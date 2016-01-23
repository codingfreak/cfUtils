namespace s2.s2Utils.Logic.Screenshot
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Specifies animation effects associated with user actions. 
    /// Used with SystemParametersInfo when SPI_GETANIMATION or SPI_SETANIMATION action is specified.
    /// </summary>
    /// <remark>
    /// The uiParam value must be set to (System.UInt32)Marshal.SizeOf(typeof(ANIMATIONINFO)) when using this structure.
    /// </remark>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Animationinfo
    {
        /// <summary>
        /// Creates an AMINMATIONINFO structure.
        /// </summary>
        /// <param name="iMinAnimate">If non-zero and SPI_SETANIMATION is specified, enables minimize/restore animation.</param>
        public Animationinfo(bool iMinAnimate)
        {
            cbSize = GetSize();
            minAnimate = iMinAnimate ? 1 : 0;
        }

        /// <summary>
        /// Always must be set to (System.UInt32)Marshal.SizeOf(typeof(ANIMATIONINFO)).
        /// </summary>
        public readonly uint cbSize;

        /// <summary>
        /// If non-zero, minimize/restore animation is enabled, otherwise disabled.
        /// </summary>
        private int minAnimate;

        public bool MinAnimate
        {
            get
            {
                return minAnimate != 0;
            }
            set
            {
                minAnimate = value ? 1 : 0;
            }
        }

        public static uint GetSize()
        {
            return (uint)Marshal.SizeOf(typeof(Animationinfo));
        }
    }
}