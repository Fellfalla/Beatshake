using System;

using Windows.Devices.Sensors;
using Windows.UI.Core;
using Beatshake.Core;
using Beatshake.DependencyServices;

class MotionDataProviderImplementation : IMotionDataProvider
{
    readonly Accelerometer _accelerometer = Accelerometer.GetDefault();
    readonly Gyrometer _gyrometer = Gyrometer.GetDefault();
    private readonly uint _minInterval;

    public MotionDataProviderImplementation()
    {
        if (_accelerometer != null)
        {
#if WINDOWS_UWP
                _accelerometer.ReportLatency = 0;
#endif


            _accelerometer.ReadingChanged += OnNewSensorData;
            _minInterval = Math.Max(_minInterval, _accelerometer.MinimumReportInterval);
        }


        if (_gyrometer == null)
        {
            new UserTextNotifierImplementation().Notify("Your phone doesn't have a gyrometer.\n This might reduce your user experience drastically.");
        }
        else
        {
            _gyrometer.ReadingChanged += OnNewSensorData;
            _minInterval = Math.Max(_minInterval, _gyrometer.MinimumReportInterval);
        }

        RefreshRate = BeatshakeSettings.SensorRefreshInterval;
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
    private async void OnNewSensorData(Gyrometer sender, GyrometerReadingChangedEventArgs args)
    {
        await
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.High, () =>
                {
                    Acceleration.Trans[0] = args.Reading.AngularVelocityX;
                    Acceleration.Trans[1] = args.Reading.AngularVelocityY;
                    Acceleration.Trans[2] = args.Reading.AngularVelocityZ;
                    //MotionDataRefreshed?.Invoke(this); // todo: sync Gyrometer and Accelerometer events
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
            var interval = Math.Max(value, _minInterval);

            if (_accelerometer != null)
                _accelerometer.ReportInterval = interval;

            if (_gyrometer != null) _gyrometer.ReportInterval = interval;
        }
    }

    public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
}