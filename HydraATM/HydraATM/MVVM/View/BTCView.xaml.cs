using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Windows.Input;
using HydraATM.MVVM.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HydraATM.MVVM.View
{
    public sealed partial class BTCView : UserControl
    {
        public BTCView()
        {
            this.InitializeComponent();
        }

        private void LettersOnlyTextBox(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(args.NewText, @"^[a-zA-Z]*$"))
            {
                args.Cancel = true;
            }
        }

        public void NumbersOnlyTextBox(object sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(args.NewText, @"^\d*$"))
            {
                args.Cancel = true;
            }
        }

        public void NumbersAndDecimalTextBox(object sender, TextBoxBeforeTextChangingEventArgs args)
        {
            // Allow numbers and a single decimal point
            if (!System.Text.RegularExpressions.Regex.IsMatch(args.NewText, @"^(\d*\.?\d*)?$"))
            {
                args.Cancel = true;
            }
            else if (args.NewText.Count(c => c == '.') > 1)
            {
                args.Cancel = true;
            }
        }
    }
}