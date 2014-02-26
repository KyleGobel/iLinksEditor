using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using iLinks.Data;
using ReactiveUI;

namespace iLinksEditor.ViewModels
{
    public class BaseFoldersViewModel : ReactiveObject
    {
        private readonly ReadOnlyCollection<FolderViewModel> _folders;

        public BaseFoldersViewModel(Folder[] baseFolders)
        {
            _folders = new ReadOnlyCollection<FolderViewModel>(
                baseFolders.Select(x => new FolderViewModel(x)).OrderBy(x => x.Folder.Name).ToArray());
        }

        public ReadOnlyCollection<FolderViewModel> Folders
        {
            get { return _folders; }
        }
    }
}