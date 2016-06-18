using System;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SharpDX.Mathematics.Interop;

class MotionDataProviderImplementation : IMotionDataProvider
{
    readonly Accelerometer _accelerometer = Accelerometer.GetDefault();
    readonly Gyrometer _gyrometer = Gyrometer.GetDefault();
    readonly OrientationSensor _orientationSensor = OrientationSensor.GetDefault();

    private OrientationSensorReading _initialOrientation;
    private Matrix<double> _initialRotationMatrix;
    private AccelerometerReading _initialAcceleration;
    private Vector<double> _initialAccelerationVector;
    private Vector<double> _initialTransformedAccelerationVector;

    private OrientationSensorReading _currentOrientation;
    private AccelerometerReading _currentAcceleration;
    private GyrometerReading _currentGyro;

    public uint MinInterval
    {
        get { return _minInterval; }
    }

    private readonly uint _minInterval;
    private uint _currentInterval;

    public MotionDataProviderImplementation()
    {
        _minInterval = 0;

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
        Calibrate();
        RefreshRate = BeatshakeSettings.SensorRefreshInterval;
    }

    /// <summary>
    /// <exception cref="NotSupportedException">Throws if a accelerometer or orientation sensor is missing</exception>
    /// </summary>
    private void Calibrate()
    {
        if (HasAccelerometer && HasOrientationSensor)
        {
            _initialAcceleration = _accelerometer.GetCurrentReading();
            _initialAccelerationVector = _initialAcceleration.BuildVector();

            _initialOrientation = _orientationSensor.GetCurrentReading();
            _initialRotationMatrix = _initialOrientation.RotationMatrix.BuildMatrix();

            _initialTransformedAccelerationVector = _initialRotationMatrix.Transpose().Multiply(_initialAccelerationVector);
        }
        else
        {
            throw new NotSupportedException("Either no accelerometer or orientation sensor available.");
        }
    }

    private double[] gravity = new double[] {0,0,0};
    
    /// alpha is calculated as t / (t + dT)
    /// with t, the low-pass filter's time-constant
    /// and dT, the event delivery rate
    private const double Alpha = 0.8;



    private void ProcessCurrentMeasurements()
    {
        // If any measurements are not ready
        if (HasOrientationSensor && _currentOrientation == null ||
            HasGyrometer && _currentGyro == null ||
            HasAccelerometer && _currentAcceleration == null)
        {
            return;
        }

        var newTimestamp = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;

        AccelerationDerivation.Trans[0] = _currentAcceleration.AccelerationX - AbsoluteAcceleration.Trans[0];
        AccelerationDerivation.Trans[1] = _currentAcceleration.AccelerationY - AbsoluteAcceleration.Trans[1];
        AccelerationDerivation.Trans[2] = _currentAcceleration.AccelerationZ - AbsoluteAcceleration.Trans[2];
        AccelerationDerivation.Timestamp = newTimestamp;

        // Assign Acceleration (no dump needed, cause values are measured, not calculated)
        AbsoluteAcceleration.Trans[0] = _currentAcceleration.AccelerationX;
        AbsoluteAcceleration.Trans[1] = _currentAcceleration.AccelerationY;
        AbsoluteAcceleration.Trans[2] = _currentAcceleration.AccelerationZ;
        AbsoluteAcceleration.Timestamp = newTimestamp;

        // Assign Linear Acceleration without gravity (high pass filter)
        if (HasOrientationSensor)
        {
            var orientation = _currentOrientation.RotationMatrix.BuildMatrix();

            var gravityVector = orientation.Multiply(_initialTransformedAccelerationVector);

            // use the orientation to substract gravity
            Acceleration.Trans[0] = _currentAcceleration.AccelerationX - gravityVector[0];
            Acceleration.Trans[1] = _currentAcceleration.AccelerationY - gravityVector[1];
            Acceleration.Trans[2] = _currentAcceleration.AccelerationZ - gravityVector[2];
        }
        else
        {
            gravity[0] = Alpha * gravity[0] + (1 - Alpha) * _currentAcceleration.AccelerationX;
            gravity[1] = Alpha * gravity[1] + (1 - Alpha) * _currentAcceleration.AccelerationY;
            gravity[2] = Alpha * gravity[2] + (1 - Alpha) * _currentAcceleration.AccelerationZ;

            Acceleration.Trans[0] = _currentAcceleration.AccelerationX - gravity[0];
            Acceleration.Trans[1] = _currentAcceleration.AccelerationY - gravity[1];
            Acceleration.Trans[2] = _currentAcceleration.AccelerationZ - gravity[2];
        }


        // Integrate Velocity
        var velDeltaT = newTimestamp - Velocity.Timestamp;
        Velocity.Trans[0] *= BeatshakeSettings.AccelerationDamp;
        Velocity.Trans[0] += _currentAcceleration.AccelerationX * (velDeltaT);

        Velocity.Trans[1] *= BeatshakeSettings.AccelerationDamp;
        Velocity.Trans[1] += _currentAcceleration.AccelerationY * (velDeltaT);

        Velocity.Trans[2] *= BeatshakeSettings.AccelerationDamp;
        Velocity.Trans[2] += _currentAcceleration.AccelerationZ * (velDeltaT);

        Velocity.Timestamp = newTimestamp;

        // Integrate Position
        var posDeltaT = newTimestamp - Pose.Timestamp;
        var posDeltaTAcc = posDeltaT * posDeltaT * 0.5;

        Pose.Trans[0] *= BeatshakeSettings.AccelerationDamp;
        Pose.Trans[0] += _currentAcceleration.AccelerationX * (posDeltaTAcc) + Velocity.Trans[0] * posDeltaT;

        Pose.Trans[1] *= BeatshakeSettings.AccelerationDamp;
        Pose.Trans[1] += _currentAcceleration.AccelerationY * (posDeltaTAcc) + Velocity.Trans[1] * posDeltaT;

        Pose.Trans[2] *= BeatshakeSettings.AccelerationDamp;
        Pose.Trans[2] += _currentAcceleration.AccelerationZ * (posDeltaTAcc) + Velocity.Trans[2] * posDeltaT;

        Pose.Timestamp = newTimestamp;
        MotionDataRefreshed?.Invoke(this);

        Velocity.Timestamp = newTimestamp;
        Velocity.Rot[0] = _currentGyro.AngularVelocityX;
        Velocity.Rot[1] = _currentGyro.AngularVelocityY;
        Velocity.Rot[2] = _currentGyro.AngularVelocityZ;

        Pose.Timestamp = newTimestamp;
        Pose.Rot[0] = _currentOrientation.Quaternion.X;
        Pose.Rot[1] = _currentOrientation.Quaternion.Y;
        Pose.Rot[2] = _currentOrientation.Quaternion.Z;

        // Reset readings
        ResetReadings();
    }


    private void ResetReadings()
    {
        _currentAcceleration = null;
        _currentGyro = null;
        _currentOrientation = null;
    }

    private void OnNewSensorData(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
    {
        _currentAcceleration = args.Reading;
    }

    private void OnNewSensorData(Gyrometer sender, GyrometerReadingChangedEventArgs args)
    {
        _currentGyro = args.Reading;
    }

    private void OnNewSensorData(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
    {
        _currentOrientation = args.Reading;
        ProcessCurrentMeasurements();
    }

    /// <summary>
    /// Unit: G*milliseconds^2 -> Factor To m: 9,81*10^-6
    /// </summary>
    public DataContainer<double> Pose { get; } = new DataContainer<double>();

    /// <summary>
    /// Unit: G*milliseconds -> Factor to m/s: 9,81*10-3
    /// </summary>
    public DataContainer<double> Velocity { get; } = new DataContainer<double>();

    public DataContainer<double> Acceleration { get; } = new DataContainer<double>();

    public DataContainer<double> AbsoluteAcceleration { get; } = new DataContainer<double>();

    public DataContainer<double> AccelerationDerivation { get; } = new DataContainer<double>();

    public bool HasAccelerometer { get { return _accelerometer != null; } }

    public bool HasGyrometer { get { return _gyrometer != null; } }

    public bool HasOrientationSensor { get { return _orientationSensor != null; } }

    public uint RefreshRate
    {
        get { return _accelerometer != null ? _accelerometer.ReportInterval : _currentInterval; }
        set
        {
            _currentInterval = Math.Max(value, MinInterval);

            if (_accelerometer != null) _accelerometer.ReportInterval = _currentInterval;
            if (_gyrometer != null) _gyrometer.ReportInterval = _currentInterval;
            if (_orientationSensor != null) _orientationSensor.ReportInterval = _currentInterval;

        }
    }

    public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
}

public static class FactoryExtensions
{
    public static Matrix<double> BuildMatrix(this SensorRotationMatrix rotationMatrix)
    {
        return Matrix<double>.Build.Dense(3, 3, new double[]
        {
            rotationMatrix.M11,
            rotationMatrix.M12,
            rotationMatrix.M13,
            rotationMatrix.M21,
            rotationMatrix.M22,
            rotationMatrix.M23,
            rotationMatrix.M31,
            rotationMatrix.M32,
            rotationMatrix.M33,
        });
    }
        public static Vector<double> BuildVector(this AccelerometerReading accelerometerReading)
    {
        return Vector<double>.Build.Dense(new double[]
        {
            accelerometerReading.AccelerationX,
            accelerometerReading.AccelerationY,
            accelerometerReading.AccelerationZ,
        });
    }
    
}