using MahApps.Metro.Controls;

namespace iLinksEditor.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : MetroWindow
    {
        public Editor()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.EditorViewModel();
        }
    }
}
