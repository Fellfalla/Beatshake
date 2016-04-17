using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.UI.Core;
using Beatshake.Core;

namespace Beatshake.UWP
{
    class MotionDataProviderImplementation : IMotionDataProvider
    {
        readonly Accelerometer _accelerometer = Accelerometer.GetDefault();

        public MotionDataProviderImplementation()
        {

            if (_accelerometer != null)
            {
                _accelerometer.ReportLatency = 0;
                _accelerometer.ReadingChanged += OnNewSensorData;
            }
        }
        
        private async void OnNewSensorData(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync( CoreDispatcherPriority.High, () =>
            {
                Acceleration.Trans[0] = args.Reading.AccelerationX;
                Acceleration.Trans[1] = args.Reading.AccelerationY;
                Acceleration.Trans[2] = args.Reading.AccelerationZ;
                MotionDataRefreshed?.Invoke(this, EventArgs.Empty);
            });
        }

        public DataContainer<double> Pose { get; } = new DataContainer<double>();

        public DataContainer<double> Velocity { get; } = new DataContainer<double>();

        public DataContainer<double> Acceleration { get; } = new DataContainer<double>();

        public int RefreshRate
        {
            get
            {
                return _accelerometer != null ? (int) _accelerometer.MinimumReportInterval : 0;
            }
            set
            {
                if (_accelerometer != null) _accelerometer.ReportInterval = Math.Min((uint) value, _accelerometer.MinimumReportInterval);
            }
        }

        public event EventHandler MotionDataRefreshed;


    }
}
