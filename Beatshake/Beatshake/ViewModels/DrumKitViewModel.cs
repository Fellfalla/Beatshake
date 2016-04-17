using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Prism.Mvvm;
using Prism.Services;
using Cadenza.Collections;
using Prism.Events;
using Xamarin.Forms;

namespace Beatshake.ViewModels
{
    public class DrumKitViewModel : BindableBase, IInstrumentalIdentification
    {
        private ObservableCollection<InstrumentalComponent> _components;
        private string _title;
        private string _kit;
        private IMotionDataProvider _dataProvider = Xamarin.Forms.DependencyService.Get<IMotionDataProvider>();

        public DrumKitViewModel()
        {
            Components = new ObservableCollection<InstrumentalComponent>();
            Title = "DrumKit 1";

            foreach (var allName in DrumComponentNames.GetAllNames())
            {
                Components.Add(new InstrumentalComponent(this, allName));
            }

            _dataProvider.RefreshRate = 0;
            _dataProvider.MotionDataRefreshed += ProcessMotionData;
        }



        private async void ProcessMotionData(object sender, EventArgs eventArgs)
        {
            var dataProvider = sender as IMotionDataProvider;

            if (dataProvider != null)
            {
                if (dataProvider.Acceleration.Trans.Any(d => d > 0.2))
                {
                    foreach (var component in Components)
                    {
                        await component.PlaySoundCommand.Execute();
                    }
                }
            }
        }

        public string Kit
        {
            get { return _kit; }
            set
            {
                SetProperty(ref _kit, value);
                foreach (var instrumentalComponent in Components)
                {
                    instrumentalComponent.PreLoadAudio(); // the whole Kit has changed
                }
            }
        }

        public ObservableCollection<InstrumentalComponent> Components
        {
            get { return _components; }
            set { SetProperty(ref _components, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

    }
}
