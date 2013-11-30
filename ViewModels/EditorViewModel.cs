using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Api.JetNett.Models.Operations;
using Api.JetNett.Models.Types;
using ReactiveUI;
using RestSharp;
using ServiceStack.Common;
using DataFormat = RestSharp.DataFormat;

namespace iLinksEditor.ViewModels
{
    public interface IEditorViewModel : IRoutableViewModel
    {
        Dictionary<Client, MetroiLinks> MetroiLinks { get; }
        IReactiveCommand SaveILinksCommand { get; }
        MetroiLinksViewModel MetroiLinksViewModel { get; }
        string FilterClientsText { get; set; }
        List<Client> Clients { get; set; } 
        Client SelectedClient { get; set; }

        IReactiveCommand ClearFilterTextCommand { get; set; }
    }

    public class EditorViewModel : ReactiveObject, IEditorViewModel
    {
        public EditorViewModel(IScreen screen)
        {
            HostScreen = screen;

            _restClient = new RestClient("http://jetnett.com/api/");

            var clientsObs = GetClients();
            var iLinksObs = GetILinks();

         

            //join both data streams, and make a dictionary out of them when they return
            clientsObs.Join(
                iLinksObs,
                _ => Observable.Never<Unit>(),
                _ => Observable.Never<Unit>(),
                (clientList, iLinksList) => clientList.Join(
                    iLinksList,
                    c => c.Id,
                    i => i.ClientId,
                    (c, i) => new
                    {
                        Client = c,
                        MetroiLinks = i
                    })
                ).Subscribe(r =>
                {
                    MetroiLinks = r.ToDictionary(k => k.Client, v => v.MetroiLinks);
                    Clients = r.Select(x => x.Client).ToList();
                });

      

            //Filter the clients list based on what the filter text is
            this.ObservableForProperty(x => x.FilterClientsText)
               .Where(x => x != null)
               .Select(x => x.Value)
               .Subscribe(filterText =>
               {
                   if (filterText.Length == 0)
                       Clients = MetroiLinks.Select(m => m.Key).ToList();
                   else
                   {
                        Clients = MetroiLinks.Select(m => m.Key)
                           .ToList()
                           .Where(c => c.Id.ToString().Contains(filterText) || c.Name.ToUpper().Contains(filterText.ToUpper()))
                           .ToList();   
                   }
               });

            //On SelectedClient Changed
            this.ObservableForProperty(x => x.SelectedClient)
                .Select(x => x.Value)
                .Subscribe(client =>
                {
                    if (client != null)
                        MetroiLinksViewModel.MetroiLink = MetroiLinks[client];
                });
            
            //Clear the filter on the ClearFilterTextcommand
            ClearFilterTextCommand = new ReactiveCommand();
            ClearFilterTextCommand.Subscribe(x => FilterClientsText = "");

            SaveILinksCommand = new ReactiveCommand(this.WhenAny(x => x.SelectedClient, x => x.Value != null));
            SaveILinksCommand.Subscribe(x =>
            {
                 var requestDto = new MetroiLinkRequestDTO
                 {
                     Entity = MetroiLinks[SelectedClient],
                     Id = MetroiLinks[SelectedClient].Id
                 };
                
                 var request = new RestRequest("metroilinks/", Method.PUT) {RequestFormat = DataFormat.Json};

                 request.AddBody(requestDto);
                 request.AddParameter("request", requestDto);
                 _restClient.ExecuteAsync(request, response =>
                 {
                     if (response.StatusCode != HttpStatusCode.NoContent)
                     {
                         MessageBox.Show("Error: " + response.StatusCode);
                     }
                     else
                     {
                         MessageBox.Show("Successfully updated");
                     }
                 });
            });

            MetroiLinksViewModel = new MetroiLinksViewModel();


            MessageBus.Current.Listen<ObservableCollection<Page>>().Subscribe(x =>
            {
                var pageIds = x.Select(page => page.Id);
                var pageIdsString = pageIds.Aggregate("", (current, id) => current + (id + ","));
                pageIdsString = pageIdsString.Remove(pageIdsString.Length - 1);

                MetroiLinksViewModel.MetroiLink.DropDownPageIds = pageIdsString;
                var currentSelectedILink = MetroiLinksViewModel.MetroiLink;

                MetroiLinksViewModel.MetroiLink = null;
                MetroiLinksViewModel.MetroiLink = currentSelectedILink;
            });

        }


        private List<Client> _clients;

        public List<Client> Clients
        {
            get { return _clients; }
            set { this.RaiseAndSetIfChanged(ref _clients, value); }
        }

        public IReactiveCommand ClearFilterTextCommand { get; set; }
        private IObservable<List<Client>> GetClients()
        {
            var request = new RestRequest("client/", Method.GET);
            var subject = new AsyncSubject<List<Client>>();

            _restClient.ExecuteAsync<ClientResponseDTO>(request, response =>
            {
                subject.OnNext(response.Data.Entities);
                subject.OnCompleted();
            });
            return subject;
        }

        private IObservable<List<MetroiLinks>> GetILinks()
        {
            var request = new RestRequest("metroilinks/", Method.GET);
            var subject = new AsyncSubject<List<MetroiLinks>>();

            _restClient.ExecuteAsync<MetroiLinksResponseDTO>(request, response =>
            {
                subject.OnNext(response.Data.Entities);
                subject.OnCompleted();
            });
            return subject;
        }

        private string _filterClientsText;

        public string FilterClientsText
        {
            get { return _filterClientsText; }
            set { this.RaiseAndSetIfChanged(ref _filterClientsText, value); }
        }

        private Client _selectedClient;

        public Client SelectedClient
        {
            get { return _selectedClient; }
            set { this.RaiseAndSetIfChanged(ref _selectedClient, value); }
        }



        private Dictionary<Client, MetroiLinks> _metroiLinks;
        public Dictionary<Client, MetroiLinks> MetroiLinks
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