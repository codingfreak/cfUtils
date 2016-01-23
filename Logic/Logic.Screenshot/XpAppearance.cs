namespace s2.s2Utils.Logic.Screenshot
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static partial class XpAppearance
    {
        #region properties

        /// <summary>
        /// Gets or Sets MinAnimate Effect
        /// </summary>
        public static bool MinAnimate
        {
            get
            {
                var animationInfo = new Animationinfo(false);
                NativeMethods.SystemParametersInfo(SPI.SPI_GETANIMATION, Animationinfo.GetSize(), ref animationInfo, Spif.None);
                return animationInfo.MinAnimate;
            }
            set
            {
                var animationInfo = new Animationinfo(value);
                NativeMethods.SystemParametersInfo(SPI.SPI_SETANIMATION, Animationinfo.GetSize(), ref animationInfo, Spif.SPIF_SENDCHANGE);
            }
        }

        #endregion
    }
}