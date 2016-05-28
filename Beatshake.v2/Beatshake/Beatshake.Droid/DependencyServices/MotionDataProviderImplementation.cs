using System;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Widget;
using Beatshake.Core;
using Beatshake.DependencyServices;

namespace Beatshake.Droid.DependencyServices
{
    class MotionDataProviderImplementation : IMotionDataProvider, ISensorEventListener
    {

        public DataContainer<double> Pose { get; } = new DataContainer<double>();

        public DataContainer<double> Velocity { get; } = new DataContainer<double>();

        public DataContainer<double> Acceleration { get; } = new DataContainer<double>();


        static readonly object _syncLock = new object();
        SensorManager _sensorManager;
        TextView _sensorTextView;
        private Sensor _accelerometer;

        public bool HasAccellerometer { get { return _accelerometer != null; } }
        public bool HasGyrometer { get; }
        public bool HasOrientationSensor { get; }

        public MotionDataProviderImplementation()
        {
            _sensorManager = (SensorManager) Application.Context.GetSystemService(Context.SensorService);
            _accelerometer = _sensorManager.GetDefaultSensor(SensorType.LinearAcceleration);
            _sensorManager.RegisterListener(this, _accelerometer, SensorDelay.Normal);

            Sensor sens = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            _sensorManager.RegisterListener(this, sens, SensorDelay.Normal);

            sens = _sensorManager.GetDefaultSensor(SensorType.Gyroscope);
            _sensorManager.RegisterListener(this, sens, SensorDelay.Normal);

        }


        public uint RefreshRate
        {
            get { return _sensorManager != null ? (uint) _accelerometer.MinDelay : 0; }
            set
            {
                if (_accelerometer != null)
                {
                    //_accelerometer.Resolution = (int) Math.Max(value, _accelerometer.MinimumReportInterval);
                }

            }
        }

        public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IntPtr Handle { get; }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            
            throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            Acceleration.Trans[0] = e.Values[0];
            Acceleration.Trans[1] = e.Values[1];
            Acceleration.Trans[2] = e.Values[2];
            MotionDataRefreshed?.Invoke(this);
        }
    }
}