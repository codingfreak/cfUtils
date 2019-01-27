namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Basic implementation of a <see cref="ITypeDescriptorContext" />.
    /// </summary>
    public class BaseTypeDescriptorContext : ITypeDescriptorContext
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="instance">The object instance which should be handled by this context.</param>
        /// <param name="propertyName">The name of the property of <paramref name="instance" />.</param>
        public BaseTypeDescriptorContext(object instance, string propertyName)
        {
            Instance = instance;
            PropertyDescriptor = TypeDescriptor.GetProperties(instance)[propertyName];
        }

        #endregion

        #region explicit interfaces

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return null;
        }

        /// <inheritdoc />
        public IContainer Container { get; private set; }

        /// <inheritdoc />
        public object Instance { get; }

        /// <inheritdoc />
        public void OnComponentChanged()
        {
        }

        /// <inheritdoc />
        public bool OnComponentChanging()
        {
            return true;
        }

        /// <summary>
        /// The descriptor for the property.
        /// </summary>
        public PropertyDescriptor PropertyDescriptor { get; }

        #endregion
    }
}