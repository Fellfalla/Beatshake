using Beatshake.Core;
using Beatshake.DependencyServices;
using Prism.Commands;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public abstract class InstrumentViewModelBase : MeasureViewModelBase, IInstrumentalIdentification
    {


        public InstrumentViewModelBase(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService, motionDataProvider)
        {
            //TeachCommand = new DelegateCommand(Teach);
        }

        public abstract string Kit { get; set; }

        //private DelegateCommand _teachCommand;
        //public DelegateCommand TeachCommand
        //{
        //    get { return _teachCommand; }
        //    set { SetProperty(ref _teachCommand, value); }
        //}

        //public DelegateCommand<InstrumentalComponent> TeachCommand { get; set; }

        //protected abstract void Teach(InstrumentalComponent component);
        //protected abstract void Teach();
    }
}
