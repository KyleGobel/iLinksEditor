using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Data;
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
            SelectedPages = new SortableObservableCollection<Page>();
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
                var pageIdsList = pageIds.Split(new[] { ',' }).Select(int.Parse).ToArray();
                var repo = new PagesRepository(new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress), 15);

                foreach(var x in pageIdsList)
                {
                    repo.GetById(x).ObserveOnDispatcher().Subscribe(page =>
                    {
                        SelectedPages.Add(page);
                        SelectedPages.Sort(s => s.Title);
                    });
                }


            });

            //setup our remove page command
            RemovePageCommand = new ReactiveCommand();
            RemovePageCommand.Subscribe(x => SelectedPages.Remove(x as Page));

            this.ObservableForProperty(x => x.SelectedPageAdd)
                .Where(x => x.Value != null)
                .Select(x => x.Value)
                .Subscribe(x =>
            {

            });
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

        private SortableObservableCollection<Page> _selectedPages;

        public SortableObservableCollection<Page> SelectedPages
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
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        public SortableObservableCollection(IEnumerable<T> collection) :
            base(collection) { }

        public SortableObservableCollection() : base() { }

        public void Sort<TKey>(Func<T, TKey> keySelector)
        {
            Sort(Items.OrderBy(keySelector));
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            Sort(Items.OrderBy(keySelector, comparer));
        }

        public void SortDescending<TKey>(Func<T, TKey> keySelector)
        {
            Sort(Items.OrderByDescending(keySelector));
        }

        public void SortDescending<TKey>(Func<T, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            Sort(Items.OrderByDescending(keySelector, comparer));
        }

        public void Sort(IEnumerable<T> sortedItems)
        {
            List<T> sortedItemsList = sortedItems.ToList();
            for (int i = sortedItemsList.Count - 1; i > -1; i--)
            {
                if (Count > 0) Move(IndexOf(sortedItemsList[i]), 0);
            }
        }
    }
}