using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Api.JetNett.Models.Types;
using iLinksEditor.ViewModels;
using MahApps.Metro.Controls;
using Ninject.Planning.Bindings;
using ReactiveUI;
using RestSharp.Extensions;
using ServiceStack.Text;

namespace iLinksEditor.Views
{
    public class ListToReactiveListConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type lhs, Type rhs)
        {
            return rhs == typeof (ReactiveList<Client>) ? 2 : 0;
        }

        public bool TryConvert(object @from, Type toType, object conversionHint, out object result)
        {
            if (conversionHint.ToString() == "Client")
            {
                result = new ReactiveList<Client>(@from as List<Client>);
                return true;
            }

            result = null;
            return false;
        }
    }
    public class ReactiveListToListConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type lhs, Type rhs)
        {
            return rhs == typeof (List<Client>) ? 2 : 0;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            if (conversionHint.ToString() == "Client")
            {
                if (from == null)
                {
                    result = default(List<Client>);
                    return true;
                }
                var reactiveList = from as ReactiveList<Client>;
                if (reactiveList == null)
                    throw new ArgumentException("Wrong converter is being used");
                result = reactiveList.AsEnumerable();
                return true;
            }
            result = toType.GetDefaultValue();
            return false;
        }
    }
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : IViewFor<IEditorViewModel>
    {
        public Editor()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
            SetupBindings();
        }

        public void SetupBindings()
        {
            this.Bind(ViewModel, x => x.FilterClientsText, x => x.FilterClientsTextBox.Text);
            this.BindCommand(ViewModel, x => x.ClearFilterTextCommand, x => x.ClearFiltersButton);
            this.Bind(ViewModel, x => x.SelectedClient, x => x.ClientsListBox.SelectedItem);
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
}
