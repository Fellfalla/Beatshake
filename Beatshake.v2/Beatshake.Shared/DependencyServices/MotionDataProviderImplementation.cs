using System;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Beatshake.Core;
using Beatshake.DependencyServices;

class MotionDataProviderImplementation : IMotionDataProvider
{
    readonly Accelerometer _accelerometer = Accelerometer.GetDefault();
    readonly Gyrometer _gyrometer = Gyrometer.GetDefault();
    readonly OrientationSensor _orientationSensor = OrientationSensor.GetDefault();

    public uint MinInterval
    {
        get { return _minInterval; }
    }

    private readonly uint _minInterval;

    public MotionDataProviderImplementation()
    {
        _minInterval = BeatshakeSettings.SensorRefreshInterval;

        if (_accelerometer != null)
        {
            #if WINDOWS_UWP
                _accelerometer.ReportLatency = 0;
            #endif

            _minInterval = Math.Max(_minInterval, _accelerometer.MinimumReportInterval);
            _accelerometer.ReadingChanged += OnNewSensorData;
        }

        if (_orientationSensor != null)
        {
            _minInterval = Math.Max(_minInterval, _orientationSensor.MinimumReportInterval);
            _orientationSensor.ReadingChanged += OnNewSensorData;
        }

        if (_gyrometer != null)
        {
            _minInterval = Math.Max(_minInterval, _gyrometer.MinimumReportInterval);
            _gyrometer.ReadingChanged += OnNewSensorData;
        }

        RefreshRate = BeatshakeSettings.SensorRefreshInterval;
    }

    private void OnNewSensorData(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
    {
        
           var task = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    Acceleration.Timestamp = (args.Reading.Timestamp.DateTime - DateTime.MinValue).TotalMilliseconds;
                    Acceleration.Trans[0] = args.Reading.AccelerationX;
                    Acceleration.Trans[1] = args.Reading.AccelerationY;
                    Acceleration.Trans[2] = args.Reading.AccelerationZ;
                    MotionDataRefreshed?.Invoke(this);
                });
        Task.WaitAll(task.AsTask());
    }

    private void OnNewSensorData(Gyrometer sender, GyrometerReadingChangedEventArgs args)
    {
        var task =
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    Velocity.Timestamp = (args.Reading.Timestamp.DateTime - DateTime.MinValue).TotalMilliseconds;
                    Velocity.Rot[0] = args.Reading.AngularVelocityX;
                    Velocity.Rot[1] = args.Reading.AngularVelocityY;
                    Velocity.Rot[2] = args.Reading.AngularVelocityZ;
                    //MotionDataRefreshed?.Invoke(this); // todo: sync Gyrometer and Accelerometer events
                });
        Task.WaitAll(task.AsTask());

    }
    private void OnNewSensorData(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
    {
        var task=
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    Pose.Timestamp = (args.Reading.Timestamp.DateTime - DateTime.MinValue).TotalMilliseconds;
                    Pose.Rot[0] = args.Reading.Quaternion.X;
                    Pose.Rot[1] = args.Reading.Quaternion.Y;
                    Pose.Rot[2] = args.Reading.Quaternion.Z;
                    //MotionDataRefreshed?.Invoke(this); // todo: sync Gyrometer and Accelerometer events
                });
        Task.WaitAll(task.AsTask());

    }

    public DataContainer<double> Pose { get; } = new DataContainer<double>();

    public DataContainer<double> Velocity { get; } = new DataContainer<double>();

    public DataContainer<double> Acceleration { get; } = new DataContainer<double>();

    public bool HasAccellerometer { get { return _accelerometer != null; } }

    public bool HasGyrometer { get { return _gyrometer != null; } }

    public bool HasOrientationSensor { get { return _orientationSensor != null; } }

    public uint RefreshRate
    {
        get { return _accelerometer != null ? _accelerometer.ReportInterval : 0; }
        set
        {
            var interval = Math.Max(value, _minInterval);

            if (_accelerometer != null) _accelerometer.ReportInterval = interval;
            if (_gyrometer != null) _gyrometer.ReportInterval = interval;
            if (_orientationSensor != null) _orientationSensor.ReportInterval = interval;

        }
    }

    public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
}