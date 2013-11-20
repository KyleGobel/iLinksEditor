using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Api.JetNett.Models.Operations;
using Api.JetNett.Models.Types;
using ReactiveUI;
using RestSharp;
using ServiceStack.Common;
using ServiceStack.ServiceClient.Web;

namespace iLinksEditor.ViewModels
{
    public class EditorViewModel : ReactiveObject
    {

        private readonly RestClient _restClient = null;
        public EditorViewModel()
        {
            _restClient = new RestClient("http://jetnett.com/api/");



            //var q = iLinksObservable.GroupJoin(clientsObservabe,
            //    _ => Observable.Never<Unit>(),
            //    _ => Observable.Never<Unit>(),
            //    (i, obsC) => Tuple.Create(i, obsC));



            var clientsObs = GetClients();
            var iLinksObs = GetILinks();

         
            var clientsILinks = clientsObs.Join(
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
                ).Subscribe(r => MetroiLinks = r.ToDictionary(k => k.Client, v => v.MetroiLinks));



            this.WhenAny(i => i.SelectedMetroILink,vm => MetroILinkName = vm.Value.Value != null ? vm.Value.Value.HomeSearchText : "No value").Subscribe(s => MetroILinkName = s);



        }

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

        private string _metroILinkName;

        public string MetroILinkName
        {
            get { return _metroILinkName; }
            set { this.RaiseAndSetIfChanged(ref _metroILinkName, value); }
        }

        private KeyValuePair<Client, MetroiLinks> _selectedMetroILink;

        public KeyValuePair<Client, MetroiLinks> SelectedMetroILink
        {
            get { return _selectedMetroILink; }
            set { this.RaiseAndSetIfChanged(ref _selectedMetroILink, value); }
        }

        private Dictionary<Client, MetroiLinks> _metroiLinks;
        public Dictionary<Client, MetroiLinks> MetroiLinks
        {
            get { return _metroiLinks; }
            set { this.RaiseAndSetIfChanged(ref _metroiLinks, value); }
        }
    }
}