using HydraATM.MVVM.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraATM.Core
{
    public class ViewModelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ATMViewTemplate { get; set; }
        public DataTemplate DepositViewTemplate { get; set; }
        public DataTemplate BTCViewTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                ATMViewModel => ATMViewTemplate,
                DepositViewModel => DepositViewTemplate,
                BTCViewModel => BTCViewTemplate,
                _ => base.SelectTemplateCore(item, container)
            };
        }
    }
}
