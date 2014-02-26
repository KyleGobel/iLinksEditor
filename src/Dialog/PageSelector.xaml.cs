using System.Windows;
using ReactiveUI;

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
