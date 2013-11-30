using iLinksEditor.Views;
using Ninject;
using ReactiveUI;

namespace iLinksEditor.ViewModels
{
    public interface IAppBootstrapper : IScreen
    { }

    public class AppBootstrapper : ReactiveObject, IAppBootstrapper
    {
        public IRoutingState Router { get; private set; }

        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, IRoutingState testRouter = null)
        {
            Router = testRouter ?? new RoutingState();

            dependencyResolver = dependencyResolver ?? RxApp.MutableResolver;

            //bind our shit up
            RegisterParts(dependencyResolver);

            //navigate to first screen
            Router.Navigate.Execute(new EditorViewModel(this));
        }

        void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new Editor(), typeof(IViewFor<EditorViewModel>));
            dependencyResolver.Register(() => new MetroiLinksViewModel(), typeof(IMetroiLinksViewModel));
        }
    }
}