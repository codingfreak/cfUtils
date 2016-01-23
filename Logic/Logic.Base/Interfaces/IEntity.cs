namespace codingfreaks.cfUtils.Logic.Base.Interfaces
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Must be implemented by all entities.
    /// </summary>
    public interface IEntity : INotifyPropertyChanged, ICloneable
    {

        #region properties

        /// <summary>
        /// The database id of the entity.
        /// </summary>
        long Id { get; set; }

        #endregion
    }
}