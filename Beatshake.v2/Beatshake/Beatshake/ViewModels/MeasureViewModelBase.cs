using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public abstract class MeasureViewModelBase : BaseViewModel, IMotionDataProcessor
    {
        public IMotionDataProvider MotionDataProvider { get; set; }

        public MeasureViewModelBase(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService)
        {
            MotionDataProvider = motionDataProvider;
            motionDataProvider.MotionDataRefreshed += ProcessMotionData;
        }

        public abstract void ProcessMotionData(IMotionDataProvider sender);

    }

    public interface IMotionDataProcessor
    {
        IMotionDataProvider MotionDataProvider { get; set; }

        void ProcessMotionData(IMotionDataProvider sender);
    }
}
