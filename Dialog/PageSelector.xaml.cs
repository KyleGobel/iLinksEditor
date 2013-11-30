using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReactiveUI;
using ServiceStack.Messaging;
using Page = Api.JetNett.Models.Types.Page;

namespace iLinksEditor.Dialog
{
    /// <summary>
    /// Interaction logic for PageSelector.xaml
    /// </summary>
 
    public partial class PageSelector
    {
        public PagesSelectorViewModel ViewModel { get; set; }

        public PageSelector()
        {
            InitializeComponent();
           
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = this.FindResource("PageSelectorViewModel") as PagesSelectorViewModel;

            if (viewModel != null)
            {
                MessageBus.Current.SendMessage(viewModel.SelectedPages);
            }
            this.Close();
        }
    }

}
