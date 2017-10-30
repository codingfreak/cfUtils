namespace codingfreaks.cfUtils.Ui.WindowsApp.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using Logic.Wpf.Components;

    /// <summary>
    /// 
    /// </summary>
    public class MainViewsModel : ViewModelBase
    {
        #region constructors and destructors

        public MainViewsModel()
        {
            if (!IsInDesignMode)
            {
                var list = new List<Person>();
                for (var i = 0; i < 1000; i++)
                {
                    list.Add(
                        new Person()
                        {
                            Id = i,
                            Firstname = Guid.NewGuid().ToString("N"),
                            Lastname = Guid.NewGuid().ToString("N")
                        });
                }
                People = new ContainedCollectionView<Person>(list, 10);                
                People.ItemsView.CurrentChanged += (s, e) =>
                {
                    Task.Run(
                        () =>
                        {
                            var item = People.CurrentItem;
                            Trace.WriteLine(item.Firstname);                            
                        });
                };
                Task.Delay(2000).ContinueWith(t => People.SwitchToFullView());
                TestCommand = new RelayCommand(
                    () =>
                    {
                        var newItem = new Person()
                        {
                            Id = 0,
                            Firstname = Guid.NewGuid().ToString("N"),
                            Lastname = Guid.NewGuid().ToString("N")
                        };
                        People.Replace(newItem, m => m.Id == newItem.Id);
                    });
            }
        }

        #endregion
        


        public ContainedCollectionView<Person> People { get; set; }

        public RelayCommand TestCommand { get; private set; }
    }
}