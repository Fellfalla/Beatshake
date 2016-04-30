using Prism.Commands;
using Prism.Navigation;

namespace Beatshake.Core
{
    public abstract class InstrumentViewModelBase : BaseViewModel, IInstrumentalIdentification
    {
        public InstrumentViewModelBase(INavigationService navigationService) : base(navigationService)
        {
            TeachCommand = new DelegateCommand<InstrumentalComponent>(Teach);
        }

        public abstract string Kit { get; set; }

        public DelegateCommand<InstrumentalComponent> TeachCommand { get; set; }

        protected abstract void Teach(InstrumentalComponent component);
    }
}
