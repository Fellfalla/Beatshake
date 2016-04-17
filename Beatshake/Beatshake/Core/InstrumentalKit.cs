using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Prism.Commands;
using Prism.Mvvm;

namespace Beatshake.ViewModels
{
    public abstract class InstrumentalKit : BindableBase, IInstrumentalIdentification
    {
        public InstrumentalKit()
        {
            TeachCommand = new DelegateCommand<InstrumentalComponent>(Teach);
        }

        public abstract string Kit { get; set; }

        public DelegateCommand<InstrumentalComponent> TeachCommand { get; set; }

        protected abstract void Teach(InstrumentalComponent component);
    }
}
