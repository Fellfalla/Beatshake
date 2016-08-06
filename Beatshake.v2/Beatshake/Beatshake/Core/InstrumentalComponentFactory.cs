using Beatshake.DependencyServices;
using Beatshake.ViewModels;
using Microsoft.Practices.Unity;

namespace Beatshake.Core
{
    public class InstrumentalComponentFactory
    {
        private IInstrumentalIdentification _containingInstrument;
        private IMotionDataProcessor _dataProcessor;
        private IMotionDataProvider _dataProvider;
        private IUnityContainer _container;
        private bool _isInitialized;

        public InstrumentalComponentFactory(IUnityContainer container)
        {
            _container = container;
        }

        public void Init(IInstrumentalIdentification containingInstrument,
            IMotionDataProcessor dataProcessor, IMotionDataProvider dataProvider)
        {
            _containingInstrument = containingInstrument;
            _dataProcessor = dataProcessor;
            _dataProvider = dataProvider;
            _isInitialized = true;
        }

        public InstrumentalComponent CreateInstrumentalComponent(string name)
        {
            if (!_isInitialized)
            {
                throw new IncompleteInitializationException("Call " + nameof(Init));
            }

            var newPlayerInstance = _container.Resolve<IInstrumentPlayer>();
            //IInstrumentPlayer newPlayerInstance = null;
            return new InstrumentalComponent(_containingInstrument,_dataProcessor, _dataProvider, newPlayerInstance, name);
        }
    }
}