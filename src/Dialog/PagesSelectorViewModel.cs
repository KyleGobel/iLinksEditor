using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using iLinks.Data;
using ReactiveUI;
using ServiceStack;

namespace iLinksEditor.Dialog
{
  
    public class PagesSelectorViewModel : ReactiveObject
    {
        protected static readonly JsonServiceClient JsonClient = new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress);
        public PagesSelectorViewModel()
        {
            SelectedPages = new SortableObservableCollection<iLinks.Data.Page>();
            //_selectedFolder = new Folder {Id =0, Name="none"};
            this.ObservableForProperty(x => x.SelectedFolder).Subscribe(x => PagesByFolder(x.Value.ID).Subscribe(
                pages =>
                {
                    Pages = pages.OrderBy(o => o.Title).ToList();
                }));


            //Listen for messages on the channel "pagesToSelect" for a list of
            //page Ids to put into the selected pages box
            MessageBus.Current.Listen<ReactiveList<iLinks.Data.Page>>("pagesToSelect").Subscribe(pages =>
            {
                if (pages == null) return;

                foreach(var x in pages)
                {
                    SelectedPages.Add(x);
                    SelectedPages.Sort(s => s.Title);
                }
            });

            StatusMessage = "Folders loaded";
            //setup our remove page command
            RemovePageCommand = new ReactiveCommand();
            RemovePageCommand.Subscribe(x => SelectedPages.Remove(x as iLinks.Data.Page));

            this.ObservableForProperty(x => x.SelectedPageAdd)
                .Where(x => x.Value != null)
                .Select(x => x.Value)
                .Subscribe(x =>
            {
                SelectedPages.Add(x);
                SelectedPages.Sort(s => s.Title);
            });
        }

        private iLinks.Data.Page _selectedPageAdd;

        public iLinks.Data.Page SelectedPageAdd
        {
            get { return _selectedPageAdd; }
            set { this.RaiseAndSetIfChanged(ref _selectedPageAdd, value); }
        }
        public IReactiveCommand RemovePageCommand { get; set; }

        private string _statusMessage;

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { this.RaiseAndSetIfChanged(ref _statusMessage, value); }
        }
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

        private List<iLinks.Data.Page> _pages; 
        public List<iLinks.Data.Page> Pages
        {
            get { return _pages; }
            set { this.RaiseAndSetIfChanged(ref _pages, value); }
        }

        private SortableObservableCollection<iLinks.Data.Page> _selectedPages;

        public SortableObservableCollection<iLinks.Data.Page> SelectedPages
        {
            get { return _selectedPages; }
            set { this.RaiseAndSetIfChanged(ref _selectedPages, value); }
        }

        private IObservable<List<iLinks.Data.Page>> PagesByFolder(int folderId)
        {
            var pagesRepo = new iLinks.Data.PagesRepo();

            return Observable.Return(pagesRepo.GetByFolderId(folderId));
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