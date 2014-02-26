using System.Collections.ObjectModel;
using iLinks.Data;
using ReactiveUI;

namespace iLinksEditor.ViewModels
{
    interface ITreeViewItemViewModel
    {
        ObservableCollection<TreeViewItemViewModel> Children { get; }
        bool HasDummyChild { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        TreeViewItemViewModel Parent { get; }

    }

    public class TreeViewItemViewModel : ReactiveObject, ITreeViewItemViewModel
    {
        static readonly TreeViewItemViewModel DummyChild = new FolderViewModel(new Folder() {ID =0, Name = "Loading Folders"});
        public TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyChildren)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

         
            if (lazyChildren)
                _children.Add(DummyChild);
        }

        public TreeViewItemViewModel()
        { }

        private readonly ObservableCollection<TreeViewItemViewModel> _children;

        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        public bool HasDummyChild
        {
            get
            {
                return this.Children.Count == 1 && this.Children[0] == DummyChild;
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                this.RaiseAndSetIfChanged(ref _isExpanded, value);

                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        protected virtual void LoadChildren()
        { }

        public bool IsSelected { get; set; }

        private TreeViewItemViewModel _parent;
        public TreeViewItemViewModel Parent { get; private set; }
    }
}