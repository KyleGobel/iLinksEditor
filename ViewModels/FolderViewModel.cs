using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Api.JetNett.Models.Operations;
using Api.JetNett.Models.Types;
using ReactiveUI;
using RestSharp;

namespace iLinksEditor.ViewModels
{
    public class FolderViewModel : TreeViewItemViewModel
    {
        private readonly ReadOnlyCollection<FolderViewModel> _folders;
        private readonly Folder _folder;
        private static readonly RestClient RestClient = new RestClient("http://jetnett.com/api/");
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
            var request = new RestRequest("folder/children/" + id, Method.GET);
            var subject = new AsyncSubject<List<Folder>>();

            RestClient.ExecuteAsync<FolderResponseDTO>(request, response =>
            {
                subject.OnNext(response.Data.Entities);
                subject.OnCompleted();
            });
            return subject;
        }

    }
}