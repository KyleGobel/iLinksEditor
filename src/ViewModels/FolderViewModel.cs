using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Api.JetNett.Models.Operations;
using Api.JetNett.Models.Types;
using JetNettApiReactive;
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
            GetChildFolders(Folder.Id).ObserveOnDispatcher().Subscribe(x => x.ForEach(f => base.Children.Add(new FolderViewModel(f))));
        }
        private IObservable<List<Folder>> GetChildFolders(int id)
        {
            var repo = new FolderRepository(new JsonServiceClient("http://jetnett.com/jetnettapi/01-04-2014/"));

            return repo.GetChildFolders(id);
        }

    }
}