using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Beatshake.Core;
using Prism.Mvvm;

namespace Beatshake.ViewModels
{
    public class DrumKitViewModel : BindableBase, IInstrumentalIdentification
    {
        private ObservableCollection<InstrumentalComponent> _components;
        private string _title;


        public DrumKitViewModel()
        {
            Components = new ObservableCollection<InstrumentalComponent>();
            Title = "DrumKit 1";

            foreach (var allName in DrumComponentNames.GetAllNames())
            {
                Components.Add(new InstrumentalComponent(this) {Name = allName});
            }
        }

        public string Kit { get; set; }

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
