namespace s2.s2Utils.Logic.Base.Delegates
{
    using EventArguments;

    using Interfaces;

    /// <summary>
    /// Is used by events which inform on entity-doublettes.
    /// </summary>
    /// <param name="sender">The sender of the event (IEntity).</param>
    /// <param name="e">Contains the list of doublettes and the possibility to cancel further operations.</param>
    public delegate void DoubletteFoundEventHandler<T>(object sender, ref DoubletteFoundEventArgs<T> e) where T : class, IEntity;
}