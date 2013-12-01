using System.Reactive.Disposables;
using Api.JetNett.Models.Types;
using ReactiveUI;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System;

namespace iLinksEditor.ViewModels
{
    public interface IMetroiLinksViewModel
    {
        MetroiLinks MetroiLink { get; set; }
        IReactiveCommand OpenPageSelectorCommand { get; set; }

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
                    MessageBus.Current.SendMessage(MetroiLink.DropDownPageIds, "pagesToSelect");
                });

            this.ObservableForProperty(x => x.MetroiLink)
                .Subscribe(x =>
                {
                    ShowEditor = x.Value != null;
                    NotShowEditor = !ShowEditor;
                });

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
    }
}