using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;
using Prism.Navigation;
using Xamarin.Forms;

namespace Beatshake.ViewModels
{
    public class DrumViewModel : InstrumentViewModelBase
    {

        public DrumViewModel(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService, motionDataProvider)
        {
            Components = new ObservableCollection<InstrumentalComponent>();
            Title = "DrumKit 1";
            Kit = "Kit1";
            foreach (var allName in DrumComponentNames.GetAllNames())
            {
                Components.Add(new InstrumentalComponent(this, allName));
            }
            MotionDataProvider.RefreshRate = BeatshakeSettings.SensorRefreshInterval;
        }

        private string _heading = "Shake your Drums!";

        public string Heading
        {
            get { return _heading; }
            set { SetProperty(ref _heading, value); }
        }

        private ObservableCollection<InstrumentalComponent> _components;
        private string _title;
        private string _kit;

        protected override async void ProcessMotionData(IMotionDataProvider motionDataProvider)
        {
            if (motionDataProvider.Acceleration.Trans.Any(d => d > 1))
            {
                await Components.Random().PlaySoundCommand.Execute();

                //var tasks = new Task[Components.Count];

                //for (int i = 0; i < Components.Count; i++)
                //{
                //    tasks[i] = Components[i].PlaySoundCommand.Execute();
                //}
                      
                ////foreach (var component in Components)
                ////{
                ////    await component.PlaySoundCommand.Execute();
                ////}
                //await Task.WhenAll(tasks);
            }
            
        }

        public override string Kit
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

        protected override void Teach(InstrumentalComponent component)
        {

            // unregister current processing
            MotionDataProvider.MotionDataRefreshed -= ProcessMotionData;

            Xamarin.Forms.DependencyService.Get<IUserSoudNotifier>().Notify();

            // reenable motion processing 
            MotionDataProvider.MotionDataRefreshed += ProcessMotionData;
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
