using System;
using System.Collections.Generic;
using System.Text;

namespace codingfreaks.cfUtils.Logic.Csv
{
    public class ItemEventArgs<TItem> : EventArgs where TItem : new()
    {
        public ItemEventArgs(TItem item)
        {
            Item = item;
        }

        public TItem Item { get; }
    }
}
