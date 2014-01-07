using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using Api.JetNett.Models.Operations;
using Api.JetNett.Models.Types;
using JetNettApiReactive;
using ReactiveUI;
using RestSharp;
using ServiceStack;

namespace iLinksEditor.Dialog
{
  
    public class PagesSelectorViewModel : ReactiveObject
    {
        protected static readonly JsonServiceClient JsonClient = new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress);
        public PagesSelectorViewModel()
        {
            SelectedPages = new ObservableCollection<Page>();
            //_selectedFolder = new Folder {Id =0, Name="none"};
            this.ObservableForProperty(x => x.SelectedFolder).Subscribe(x => PagesByFolder(x.Value.Id).Subscribe(
                pages =>
                {
                    Pages = pages;
                }));


            //Listen for messages on the channel "pagesToSelect" for a list of
            //page Ids to put into the selected pages box
            MessageBus.Current.Listen<string>("pagesToSelect").Subscribe(pageIds =>
            {
                if (string.IsNullOrEmpty(pageIds)) return;
                var pageIdsList = pageIds.Split(new[] { ',' }).ToList();
                pageIdsList.ForEach(pageId =>
                {
                    var id = 0;
                    int.TryParse(pageId, out id);
                    if (id > 0)
                    {
                        PagePathObservable(id).ObserveOnDispatcher().Subscribe(page =>
                        {
                            page.Title = page.Title.Replace(">", " > ");
                            SelectedPages.Add(page);
                        });
                    }
                });
            });

            //setup our remove page command
            RemovePageCommand = new ReactiveCommand();
            RemovePageCommand.Subscribe(x => SelectedPages.Remove(x as Page));

            this.ObservableForProperty(x => x.SelectedPageAdd)
                .Where(x => x.Value != null)
                .Select(x => x.Value)
                .Subscribe(
                    x => PagePathObservable(x.Id).ObserveOnDispatcher().Subscribe(page =>
                        {
                            page.Title = page.Title.Replace(">", " > ");
                            SelectedPages.Add(page);
                        }));
        }

        private Page _selectedPageAdd;

        public Page SelectedPageAdd
        {
            get { return _selectedPageAdd; }
            set { this.RaiseAndSetIfChanged(ref _selectedPageAdd, value); }
        }
        public IReactiveCommand RemovePageCommand { get; set; }

        private IObservable<Page> PagePathObservable(int id)
        {
            /*var r = JsonClient.GetAsync(new PagesDTO {PathPageId = id});

            return r.ToObservable().Select(x => x);
            var request = new RestRequest("page/path/" + id, Method.GET);
            var subject = new AsyncSubject<Page>();

            RestClient.ExecuteAsync<PagesResponseDTO>(request, response =>
            {
                subject.OnNext(response.Data.Entity);
                subject.OnCompleted();
            });
            return subject;
            */
            return null;
        }
        private Folder _selectedFolder;

        public Folder SelectedFolder
        {
            get { return _selectedFolder; }
            set { this.RaiseAndSetIfChanged(ref _selectedFolder, value); }
        }

        private List<Page> _pages; 
        public List<Page> Pages
        {
            get { return _pages; }
            set { this.RaiseAndSetIfChanged(ref _pages, value); }
        }

        private ObservableCollection<Page> _selectedPages;

        public ObservableCollection<Page> SelectedPages
        {
            get { return _selectedPages; }
            set { this.RaiseAndSetIfChanged(ref _selectedPages, value); }
        }

        private IObservable<List<Page>> PagesByFolder(int folderId)
        {
            var repo = new PagesRepository(new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress));

            return repo.GetByFolderId(folderId);
        }

    }
}