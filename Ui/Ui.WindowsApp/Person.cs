namespace codingfreaks.cfUtils.Ui.WindowsApp
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Logic.Wpf.Annotations;

    public class Person : INotifyPropertyChanged
    {
        #region events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region properties

        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        #endregion
    }
}