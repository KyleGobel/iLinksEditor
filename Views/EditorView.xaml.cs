using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using iLinksEditor.ViewModels;
using MahApps.Metro.Controls;
using ReactiveUI;

namespace iLinksEditor.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : IViewFor<IEditorViewModel>
    {
        public Editor()
        {
            InitializeComponent();

            this.WhenNavigatedTo(ViewModel, () =>
            {
                DataContext = ViewModel;
                return null;
            });
        }

        public IEditorViewModel ViewModel
        {
            get { return (IEditorViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value);}
        }

        public static DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (IEditorViewModel), typeof (Editor),
                new PropertyMetadata(null));
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IEditorViewModel)value; }
        }
    }
    // XXX: Ignore the man behind this curtain. This will soon be in ReactiveUI itself
    public static class ViewForMixins
    {
        public static IDisposable WhenNavigatedTo<TView, TViewModel>(this TView This, TViewModel viewModel, Func<IDisposable> onNavigatedTo)
            where TView : IViewFor<TViewModel>
            where TViewModel : class, IRoutableViewModel
        {
            var disp = Disposable.Empty;
            var inner = This.WhenAny(x => x.ViewModel, x => x.Value)
                .Where(x => x != null && x.HostScreen.Router.GetCurrentViewModel() == x)
                .Subscribe(x =>
                {
                    if (disp != null) disp.Dispose();
                    disp = onNavigatedTo();
                });

            return Disposable.Create(() =>
            {
                inner.Dispose();
                disp.Dispose();
            });
        }
    }
}
