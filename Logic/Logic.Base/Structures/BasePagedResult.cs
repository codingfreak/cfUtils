namespace codingfreaks.cfUtils.Logic.Base.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base.Interfaces;

    /// <summary>
    /// A core structure for delivering paged results.
    /// </summary>
    /// <remarks>
    /// This class contains no logic. See PagedResult{TEntity,TContext} to get logic too.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity each element of <see cref="Items"/> will be of.</typeparam>
    public class BasePagedResult<TEntity>
        where TEntity : class, IEntity
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="request">The request for generating the result.</param>
        public BasePagedResult(PagedRequest request)
        {
            Request = request;
        }

        #endregion

        #region methods

        /// <summary>
        /// Converts this instance into one that holds items of type <typeparamref name="TResult"/>
        /// </summary>
        /// <remarks>This method will call ToList() on <paramref name="transform"/> internally!</remarks>
        /// <typeparam name="TResult">The type of items that should be returned.</typeparam>
        /// <param name="transform">A tranform method to convert <typeparamref name="TEntity"/> to <typeparamref name="TResult"/>.</param>
        /// <returns>The converted result.</returns>
        public BasePagedResult<TResult> Convert<TResult>(Func<TEntity, TResult> transform) where TResult : class, IEntity
        {
            var items = Items.Select(transform);
            return new BasePagedResult<TResult>(Request)
            {
                Items = items.ToList(),
                CompleteItemsCount = CompleteItemsCount
            };
        }

        #endregion

        #region properties

        /// <summary>
        /// The amount of items from which the <see cref="Items"/> where taken.
        /// </summary>
        public long CompleteItemsCount { get; set; }

        /// <summary>
        /// The page that is currently delivered.
        /// </summary>
        public int CurrentPage => Request.PageToDeliver;

        /// <summary>
        /// The items for the current page in the <see cref="Request"/>.
        /// </summary>
        public IEnumerable<TEntity> Items { get; set; }

        /// <summary>
        /// Retrieves the amount of pages available depending on the current <see cref="Request"/>.
        /// </summary>
        /// <remarks>
        /// This retrieves -1 until Generate is called.
        /// </remarks>
        public int PagesAvailable
        {
            get
            {
                if (Items == null || !Items.Any() || CompleteItemsCount == 0)
                {
                    return -1;
                }
                return (int)Math.Ceiling((double)CompleteItemsCount / Request.ItemsPerPage);
            }
        }

        /// <summary>
        /// The request injected by the constructor.
        /// </summary>
        public PagedRequest Request { get; }

        #endregion
    }
}