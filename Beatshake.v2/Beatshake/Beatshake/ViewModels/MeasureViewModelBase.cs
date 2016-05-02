using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public abstract class MeasureViewModelBase : BaseViewModel
    {
        protected IMotionDataProvider MotionDataProvider { get; set; }

        public MeasureViewModelBase(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService)
        {
            MotionDataProvider = motionDataProvider;
            motionDataProvider.MotionDataRefreshed += ProcessMotionData;
        }

        protected abstract void ProcessMotionData(IMotionDataProvider sender);

    }
}
