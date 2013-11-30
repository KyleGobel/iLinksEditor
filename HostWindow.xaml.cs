using iLinksEditor.ViewModels;
using MahApps.Metro.Controls;

namespace iLinksEditor
{
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    public partial class HostWindow : MetroWindow
    {

        public AppBootstrapper AppBootstrapper { get; protected set; }
        public HostWindow()
        {
            InitializeComponent();

            AppBootstrapper = new AppBootstrapper();
            DataContext = AppBootstrapper;
        }
    }
}
