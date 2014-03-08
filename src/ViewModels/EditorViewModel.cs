using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using iLinks.Data;
using iLinksEditor.Dialog;
using ReactiveUI;
using RestSharp;
using ServiceStack;

namespace iLinksEditor.ViewModels
{
    public interface IEditorViewModel : IRoutableViewModel
    {
        Dictionary<iLinks.Data.Client, iLinks.Data.Metro_iLink> MetroiLinks { get; }
        IReactiveCommand SaveILinksCommand { get; }
        MetroiLinksViewModel MetroiLinksViewModel { get; }
        string FilterClientsText { get; set; }
        ReactiveList<iLinks.Data.Client> Clients { get; set; } 
        iLinks.Data.Client SelectedClient { get; set; }

        IReactiveCommand ClearFilterTextCommand { get; set; }
    }

    public class EditorViewModel : ReactiveObject, IEditorViewModel
    {
        public EditorViewModel(IScreen screen)
        {
            HostScreen = screen;


            var clientsObs = GetClients();
            var iLinksObs = GetILinks();

         

            //join both data streams, and make a dictionary out of them when they return
            clientsObs.Join(
                iLinksObs,
                _ => Observable.Never<Unit>(),
                _ => Observable.Never<Unit>(),
                (clientList, iLinksList) => clientList.Join(
                    iLinksList,
                    c => c.ID,
                    i => i.Client_ID,
                    (c, i) => new
                    {
                        Client = c,
                        MetroiLinks = i
                    })
                ).Subscribe(r =>
                {
                    MetroiLinks = r.ToDictionary(k => k.Client, v => v.MetroiLinks);
                    Clients = new ReactiveList<iLinks.Data.Client>(r.Select(x => x.Client).OrderBy(x => x.Name));
                });

      

            //Filter the clients list based on what the filter text is
            this.ObservableForProperty(x => x.FilterClientsText)
               .Where(x => x != null)
               .Select(x => x.Value)
               .Subscribe(filterText =>
               {
                   if (filterText.Length == 0)
                       Clients = new ReactiveList<iLinks.Data.Client>(MetroiLinks.Select(m => m.Key).ToList());
                   else
                   {
                        Clients = new ReactiveList<iLinks.Data.Client>(
                            MetroiLinks.Select(m => m.Key)
                               .ToList()
                               .Where(c => c.ID.ToString().Contains(filterText) || c.Name.ToUpper().Contains(filterText.ToUpper()))
                               .ToList());   
                   }
               });

            //On SelectedClient Changed
            this.ObservableForProperty(x => x.SelectedClient)
                .Select(x => x.Value)
                .Subscribe(client =>
                {
                    if (client != null)
                    {
                        MetroiLinksViewModel.MetroiLink = MetroiLinks[client];

                    }
                });
            
            //Clear the filter on the ClearFilterTextcommand
            ClearFilterTextCommand = new ReactiveCommand();
            ClearFilterTextCommand.Subscribe(x => FilterClientsText = "");

            SaveILinksCommand = new ReactiveCommand(this.WhenAny(x => x.SelectedClient, x => x.Value != null));
            SaveILinksCommand.Subscribe(x =>
            {
                var repo = new MetroiLinksRepo();
                repo.Update(MetroiLinksViewModel.MetroiLink);
                MessageBox.Show("Saved");
            });

            MetroiLinksViewModel = new MetroiLinksViewModel();

            MessageBus.Current.Listen<SortableObservableCollection<iLinks.Data.Page>>().Subscribe(x =>
            {
                var cpRepo = new CommunityProfilesRepo();
                var currentSelectedILink = MetroiLinksViewModel.MetroiLink;
                cpRepo.UpdateCommunityProfiles(currentSelectedILink.Client_ID, x);
                MetroiLinksViewModel.MetroiLink = null;
                MetroiLinksViewModel.MetroiLink = currentSelectedILink;
            });

        }


        private ReactiveList<iLinks.Data.Client> _clients;

        public ReactiveList<iLinks.Data.Client> Clients
        {
            get { return _clients; }
            set { this.RaiseAndSetIfChanged(ref _clients, value); }
        }

        public IReactiveCommand ClearFilterTextCommand { get; set; }
        private IObservable<List<iLinks.Data.Client>> GetClients()
        {
            var repo = new ClientsRepo();
            return Observable.Return(repo.GetAll());
        }

        private IObservable<List<iLinks.Data.Metro_iLink>> GetILinks()
        {
            var repo = new MetroiLinksRepo();
            return Observable.Return(repo.GetAll().ToList());
        }

        private string _filterClientsText;

        public string FilterClientsText
        {
            get { return _filterClientsText; }
            set { this.RaiseAndSetIfChanged(ref _filterClientsText, value); }
        }

        private iLinks.Data.Client _selectedClient;

        public iLinks.Data.Client SelectedClient
        {
            get { return _selectedClient; }
            set { this.RaiseAndSetIfChanged(ref _selectedClient, value); }
        }



        private Dictionary<iLinks.Data.Client, Metro_iLink> _metroiLinks;
        public Dictionary<iLinks.Data.Client, iLinks.Data.Metro_iLink> MetroiLinks
        {
            get { return _metroiLinks; }
            set { this.RaiseAndSetIfChanged(ref _metroiLinks, value); }
        }

        public IReactiveCommand SaveILinksCommand { get; protected set; }
        public string UrlPathSegment
        {
            get { return "Metro iLinks Editor"; }
        }
        public IScreen HostScreen { get; private set; }

        private MetroiLinksViewModel _metroiLinksViewModel;
        public MetroiLinksViewModel MetroiLinksViewModel
        {
            get { return _metroiLinksViewModel; }
            set { this.RaiseAndSetIfChanged(ref _metroiLinksViewModel, value); }
        }

    }
}