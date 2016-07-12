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
        private bool _isProcessingMotionData;
        public IMotionDataProvider MotionDataProvider { get; set; }

        public MeasureViewModelBase(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService)
        {
            MotionDataProvider = motionDataProvider;
            //motionDataProvider.MotionDataRefreshed += ProcessMotionData;
            IsProcessingMotionData = true;
        }

        public abstract void ProcessMotionData(IMotionDataProvider sender);

        public bool IsProcessingMotionData
        {
            get
            {
                return _isProcessingMotionData; 
            }
            set
            {
                SetProperty( ref _isProcessingMotionData, value);
                if (value)
                {
                    MotionDataProvider.MotionDataRefreshed += ProcessMotionData;
                }
                else
                {
                    MotionDataProvider.MotionDataRefreshed -= ProcessMotionData;
                }
            }
        }
    }
}
