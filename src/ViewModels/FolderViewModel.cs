using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using iLinks.Data;
using ReactiveUI;
using RestSharp;
using ServiceStack;

namespace iLinksEditor.ViewModels
{
    public class FolderViewModel : TreeViewItemViewModel
    {
        private readonly ReadOnlyCollection<FolderViewModel> _folders;
        private readonly Folder _folder;
        private static readonly JsonServiceClient JsonClient = new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress);
        public FolderViewModel(Folder folder) : base(null, true)
        {
            _folder = folder;
        }

        
        public Folder Folder
        {
            get { return _folder; }
        }
        public ReadOnlyCollection<FolderViewModel> Folders
        {
            get { return _folders; }
        }

        protected override void LoadChildren()
        {
            GetChildFolders(Folder.ID)
                .ObserveOnDispatcher()
                .Subscribe(x => x.OrderBy(o => o.Name)
                    .ToList()
                    .ForEach(f => base.Children.Add(new FolderViewModel(f))));
        }
        private IObservable<List<Folder>> GetChildFolders(int id)
        {
            var repo = new FoldersRepo();

            return Observable.Return(repo.GetChildFolders(id).ToList());
        }

    }
}