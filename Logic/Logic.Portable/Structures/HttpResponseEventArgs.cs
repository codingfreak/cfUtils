using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Portable.Structures
{
    using System.Net.Http;

    /// <summary>
    /// Is used in events where <see cref="HttpResponseMessage" /> are related.
    /// </summary>
    public class HttpResponseEventArgs : EventArgs
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message">The original message.</param>
        public HttpResponseEventArgs(HttpResponseMessage message)
        {
            Message = message;
        }

        #endregion

        #region properties

        /// <summary>
        /// The original message.
        /// </summary>
        public HttpResponseMessage Message { get; private set; }

        #endregion
    }
}