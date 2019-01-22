namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public class BaseTypeDescriptorContext : ITypeDescriptorContext
    {
        #region constructors and destructors

        public BaseTypeDescriptorContext(object instance, string propertyName)
        {
            Instance = instance;
            PropertyDescriptor = TypeDescriptor.GetProperties(instance)[propertyName];
        }

        #endregion

        #region explicit interfaces

        public object GetService(Type serviceType)
        {
            return null;
        }

        public IContainer Container { get; private set; }

        public object Instance { get; }

        public void OnComponentChanged()
        {
        }

        public bool OnComponentChanging()
        {
            return true;
        }

        public PropertyDescriptor PropertyDescriptor { get; }

        #endregion
    }
}