using System;

using Windows.Devices.Sensors;
using Windows.UI.Core;
using Beatshake.Core;
using Beatshake.DependencyServices;

class MotionDataProviderImplementation : IMotionDataProvider
{
    readonly Accelerometer _accelerometer = Accelerometer.GetDefault();

    public MotionDataProviderImplementation()
    {
        if (_accelerometer != null)
        {
#if WINDOWS_UWP
                _accelerometer.ReportLatency = 0;
#endif
            RefreshRate = BeatshakeSettings.SensorRefreshInterval;
            _accelerometer.ReadingChanged += OnNewSensorData;
        }
    }

    private async void OnNewSensorData(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
    {
        await
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.High, () =>
                {
                    Acceleration.Trans[0] = args.Reading.AccelerationX;
                    Acceleration.Trans[1] = args.Reading.AccelerationY;
                    Acceleration.Trans[2] = args.Reading.AccelerationZ;
                    MotionDataRefreshed?.Invoke(this);
                });
    }

    public DataContainer<double> Pose { get; } = new DataContainer<double>();

    public DataContainer<double> Velocity { get; } = new DataContainer<double>();

    public DataContainer<double> Acceleration { get; } = new DataContainer<double>();

    public uint RefreshRate
    {
        get { return _accelerometer != null ? _accelerometer.ReportInterval : 0; }
        set
        {
            if (_accelerometer != null)
                _accelerometer.ReportInterval = Math.Max(value, _accelerometer.MinimumReportInterval);
        }
    }

    public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
}