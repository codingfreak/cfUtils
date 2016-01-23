namespace codingfreaks.cfUtils.Logic.Base.Structures
{
    using System.Collections.Generic;

    /// <summary>
    /// This type is used as a definition for querying multiple elements in a paged manner.
    /// </summary>    
    public class PagedRequest
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PagedRequest()
        {
        }

        /// <summary>
        /// Constructor allowing passing in all properties for the request.
        /// </summary>
        /// <param name="pageToDeliver">The page which should be delivered (defaults to 1).</param>
        /// <param name="itemsPerPage">The items per page (defaults to 10).</param>
        /// <param name="orderByExpressions">A list of order expressions to use.</param>
        public PagedRequest(int pageToDeliver = 1, int itemsPerPage = 10, IEnumerable<OrderByExpression> orderByExpressions = null)
        {
            PageToDeliver = pageToDeliver;
            ItemsPerPage = itemsPerPage;
            OrderByExpressions = orderByExpressions;
        }

        #endregion

        #region properties

        /// <summary>
        /// If this property has a value the logic will not calculate the entries to skip
        /// from the multiplication of <see cref="ItemsPerPage"/> and <see cref="PageToDeliver"/> but
        /// use this value. 
        /// </summary>
        /// <remarks>
        /// Use this in scenarios where you have to load asymmetrically because you want chunks of 100, 200, 1000 ...
        /// e.g.
        /// </remarks>
        public int? EntriesToSkip { get; set; }

        /// <summary>
        /// Defines how many items should be contained in one page.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// A list of all sorting expressions used for this request.
        /// </summary>
        public IEnumerable<OrderByExpression> OrderByExpressions { get; set; }

        /// <summary>
        /// Defines which page should be delivered.
        /// </summary>
        public int PageToDeliver { get; set; }

        #endregion
    }
}