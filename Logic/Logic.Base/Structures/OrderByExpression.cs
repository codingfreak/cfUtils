namespace s2.s2Utils.Logic.Base.Structures
{
    using System;
    using System.Linq;

    using Enumerations;

    /// <summary>
    /// This structure is used by PagedRequest{TEntity} to define order-expressions.
    /// </summary>
    public class OrderByExpression
    {
        #region properties

        /// <summary>
        /// The order direction.
        /// </summary>
        public OrderByDirections OrderDirection { get; set; }

        /// <summary>
        /// The name of the property which should be used for ordering.
        /// </summary>
        public string PropertyName { get; set; }

        #endregion
    }
}
