using System.Reactive.Disposables;
using Api.JetNett.Models.Types;
using iLinks.Data;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System;
using Page = iLinks.Data.Page;

namespace iLinksEditor.ViewModels
{
    public interface IMetroiLinksViewModel
    {
        MetroiLinks MetroiLink { get; set; }
        IReactiveCommand OpenPageSelectorCommand { get; set; }
        ReactiveList<iLinks.Data.Page> CommunityProfiles { get; set; } 
        bool ShowEditor { get; set; }

        bool NotShowEditor { get; set; }
    }
    public class MetroiLinksViewModel : ReactiveObject, IMetroiLinksViewModel
    {
        public MetroiLinksViewModel()
        {
            OpenPageSelectorCommand = new ReactiveCommand();
            OpenPageSelectorCommand
                .Where(x => MetroiLink != null)
                .Select(x => new Dialog.PageSelector())
                .Subscribe(x =>
                {
                    x.Show();
                    MessageBus.Current.SendMessage(CommunityProfiles, "pagesToSelect");
                });

            this.ObservableForProperty(x => x.MetroiLink)
                .Subscribe(x =>
                {
                    ShowEditor = x.Value != null;
                    NotShowEditor = !ShowEditor;
                    var cpRepo = new CommunityProfilesRepo();
                    if (x.Value != null)
                        CommunityProfiles = new ReactiveList<Page>(cpRepo.GetCommunityProfiles(x.Value.ClientId));
                });

            CommunityProfiles = new ReactiveList<Page>();
            NotShowEditor = !ShowEditor;
        }

        private MetroiLinks _metroiLink;

        public MetroiLinks MetroiLink
        {
            get { return _metroiLink; }
            set { this.RaiseAndSetIfChanged(ref _metroiLink, value); }
        }

        private bool _notShowEditor;

        public bool NotShowEditor
        {
            get { return _notShowEditor; }
            set { this.RaiseAndSetIfChanged(ref _notShowEditor, value); }
        }

        private bool _showEditor;
        public bool ShowEditor
        {
            get { return _showEditor; }
            set { this.RaiseAndSetIfChanged(ref _showEditor, value); }
        }

        public IReactiveCommand OpenPageSelectorCommand { get; set; }

        private ReactiveList<iLinks.Data.Page> _communityProfiles;

        public ReactiveList<iLinks.Data.Page> CommunityProfiles
        {
            get { return _communityProfiles; }
            set { this.RaiseAndSetIfChanged(ref _communityProfiles, value); }
        }
    }
}