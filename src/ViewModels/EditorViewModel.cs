using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Windows;
using Api.JetNett.Models.Operations;
using Api.JetNett.Models.Types;
using iLinks.Data;
using iLinksEditor.Dialog;
using JetNettApiReactive;
using ReactiveUI;
using RestSharp;
using ServiceStack;
using ServiceStack.Common;
using ServiceStack.Configuration;
using Client = Api.JetNett.Models.Types.Client;
using DataFormat = RestSharp.DataFormat;
using Page = Api.JetNett.Models.Types.Page;

namespace iLinksEditor.ViewModels
{
    public interface IEditorViewModel : IRoutableViewModel
    {
        Dictionary<iLinks.Data.Client, MetroiLinks> MetroiLinks { get; }
        IReactiveCommand SaveILinksCommand { get; }
        MetroiLinksViewModel MetroiLinksViewModel { get; }
        string FilterClientsText { get; set; }
        ReactiveList<iLinks.Data.Client> Clients { get; set; } 
        iLinks.Data.Client SelectedClient { get; set; }

        IReactiveCommand ClearFilterTextCommand { get; set; }
    }

    public class EditorViewModel : ReactiveObject, IEditorViewModel
    {
        protected JsonServiceClient JsonClient { get; set; }
        public EditorViewModel(IScreen screen)
        {
            HostScreen = screen;

            _restClient = new RestClient(ConfigSettings.Current.JetNettApiAddress);
            JsonClient = new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress);

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
                    i => i.ClientId,
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
                var client = new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress);
                client.Credentials = new NetworkCredential("ApiUser", "ssapi");
                var repo = new MetroiLinkRepository(client);
                repo.Update(MetroiLinksViewModel.MetroiLink);
                MessageBox.Show("Saved");
            });

            MetroiLinksViewModel = new MetroiLinksViewModel();


            MessageBus.Current.Listen<SortableObservableCollection<iLinks.Data.Page>>().Subscribe(x =>
            {
                var cpRepo = new CommunityProfilesRepo();
                var currentSelectedILink = MetroiLinksViewModel.MetroiLink;
                cpRepo.UpdateCommunityProfiles(currentSelectedILink.ClientId, x);
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
            return repo.GetAll();
        }

        private IObservable<List<MetroiLinks>> GetILinks()
        {
            var repo = new MetroiLinkRepository(new JsonServiceClient(ConfigSettings.Current.JetNettApiAddress));
            return repo.GetAll();
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



        private Dictionary<iLinks.Data.Client, MetroiLinks> _metroiLinks;
        public Dictionary<iLinks.Data.Client, MetroiLinks> MetroiLinks
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

        private readonly RestClient _restClient = null;
    }
}