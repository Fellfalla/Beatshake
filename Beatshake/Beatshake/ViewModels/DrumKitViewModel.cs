using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Beatshake.Core;
using Prism.Mvvm;

namespace Beatshake.ViewModels
{
    public class DrumKitViewModel : BindableBase
    {
        private ObservableCollection<InstrumentalComponent> _components;
        private string _title;

        public DrumKitViewModel()
        {
            Components = new ObservableCollection<InstrumentalComponent>();
            Title = "DrumKit 1";

            for (int i = 0; i < 5; i++)
            {
                Components.Add(new InstrumentalComponent());
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
