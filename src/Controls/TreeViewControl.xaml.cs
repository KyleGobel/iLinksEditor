using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Api.JetNett.Models.Operations;
using Api.JetNett.Models.Types;
using iLinksEditor.ViewModels;
using JetNettApiReactive;
using ReactiveUI;
using RestSharp;
using ServiceStack;

namespace iLinksEditor.Controls
{
    /// <summary>
    /// Interaction logic for TreeViewControl.xaml
    /// </summary>
    public partial class TreeViewControl : UserControl
    {
        public TreeViewControl()
        {
            InitializeComponent();
            GetBaseFolders().ObserveOnDispatcher().Subscribe(x =>
            {
                var baseFolders = new BaseFoldersViewModel(x.ToArray());
                this.DataContext = baseFolders;
            });
        }

        public static readonly DependencyProperty SelectedFolderProperty =
            DependencyProperty.Register("SelectedFolder", typeof (Folder), typeof (TreeViewControl),new PropertyMetadata(OnFolderChanged));

        private static void OnFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //MessageBox.Show("WeChanged");
        }


        public Folder SelectedFolder
        {
            get { return (Folder)GetValue(SelectedFolderProperty); }
            set { SetValue(SelectedFolderProperty, value); }
        }
        private IObservable<List<Folder>> GetBaseFolders()
        {
            var repo = new FolderRepository(new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress));

            var allFolders = repo.GetAll();
            var folders = allFolders.Wait();
            return Observable.Return(folders.Where(x => x.ParentFolderId == null).ToList());
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var folderViewModel = e.NewValue as FolderViewModel;

            if (folderViewModel != null)
            {
                SelectedFolder = folderViewModel.Folder;
            }
        }
    }
}
