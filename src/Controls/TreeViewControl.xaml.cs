using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using iLinks.Data;
using iLinksEditor.ViewModels;

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
            var foldersRepo = new FoldersRepo();

            return Observable.Return(foldersRepo.GetRootFolders().ToList());
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
