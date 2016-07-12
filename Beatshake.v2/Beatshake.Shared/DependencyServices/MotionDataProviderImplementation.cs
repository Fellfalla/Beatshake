using System;
using System.Collections.Generic;
using Windows.Devices.Sensors;
using Windows.UI.Popups;
using Beatshake.Core;
using Beatshake.DependencyServices;
using MathNet.Numerics.LinearAlgebra;

internal class MotionDataProviderImplementation : IMotionDataProvider
{
    /// alpha is calculated as t / (t + dT)
    /// with t, the low-pass filter's time-constant
    /// and dT, the event delivery rate
    private const double Alpha = 0.8;

    private readonly Accelerometer _accelerometer = Accelerometer.GetDefault();
    private readonly Gyrometer _gyrometer = Gyrometer.GetDefault();
    private readonly OrientationSensor _orientationSensor = OrientationSensor.GetDefault();

    private readonly IDictionary<MotionData, Action<double>> _calculators;
    private AccelerometerReading _currentAcceleration;
    private GyrometerReading _currentGyro;
    private uint _currentInterval;

    private OrientationSensorReading _currentOrientation;
    private AccelerometerReading _initialAcceleration;
    private Vector<double> _initialAccelerationVector;

    private OrientationSensorReading _initialOrientation;
    private Matrix<double> _initialRotationMatrix;
    private Vector<double> _initialTransformedAccelerationVector;

    private readonly double[] _gravity = {0, 0, 0};

    public MotionData MotionDataNeeds { get; set; } = MotionData.JoltTrans | MotionData.VelocityRot;

    public MotionDataProviderImplementation()
    {
        MinInterval = 0;

        // Set CalucationActions. Pay attention about the order
        _calculators = new SortedDictionary<MotionData, Action<double>>
        {
            {MotionData.JoltTrans,              CalculateJoltTrans},
            {MotionData.JoltRot,                CalculateJoltRot},
            {MotionData.AbsAccelerationTrans,   CalculateAbsAccelerationTrans},
            {MotionData.AbsAccelerationRot,     CalculateAbsAccelerationRot},
            {MotionData.RelAccelerationTrans,   CalculateRelAccelerationTrans},
            {MotionData.RelAccelerationRot,     CalculateRelAccelerationRot},
            {MotionData.VelocityTrans,          CalculateVelocityTrans},
            {MotionData.VelocityRot,            CalculateVelocityRot},
            {MotionData.PoseTrans,              CalculatePoseTrans},
            {MotionData.PoseRot,                CalculatePoseRot}
        };

        if (_accelerometer != null)
        {
#if WINDOWS_UWP
            _accelerometer.ReportLatency = 0;
#endif

            MinInterval = Math.Max(MinInterval, _accelerometer.MinimumReportInterval);
            _accelerometer.ReadingChanged += OnNewSensorData;
        }

        if (_orientationSensor != null)
        {
            MinInterval = Math.Max(MinInterval, _orientationSensor.MinimumReportInterval);
            _orientationSensor.ReadingChanged += OnNewSensorData;
        }

        if (_gyrometer != null)
        {
            MinInterval = Math.Max(MinInterval, _gyrometer.MinimumReportInterval);
            _gyrometer.ReadingChanged += OnNewSensorData;
        }

        try
        {
            Calibrate();
        }
        catch (NotSupportedException e)
        {
            var dialog = new MessageDialog(e.ToString(), "Motion data error");
            var _ = dialog.ShowAsync();
        }
        RefreshRate = BeatshakeSettings.SensorRefreshInterval;
    }

    public uint MinInterval { get; }

    /// <summary>
    ///     Unit: G*milliseconds^2 -> Factor To m: 9,81*10^-6
    /// </summary>
    public DataContainer<double> Pose { get; } = new DataContainer<double>();

    /// <summary>
    ///     Unit: G*milliseconds -> Factor to m/s: 9,81*10-3
    /// </summary>
    public DataContainer<double> Velocity { get; } = new DataContainer<double>();

    public DataContainer<double> RelAcceleration { get; } = new DataContainer<double>();

    public DataContainer<double> AbsAcceleration { get; } = new DataContainer<double>();

    /// <summary>
    ///     Amount of change of <see cref="RelAcceleration" /> in terms of time.
    /// </summary>
    public DataContainer<double> Jolt { get; } = new DataContainer<double>();

    public bool HasAccelerometer
    {
        get { return _accelerometer != null; }
    }

    public bool HasGyrometer
    {
        get { return _gyrometer != null; }
    }

    public bool HasOrientationSensor
    {
        get { return _orientationSensor != null; }
    }

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

    /// <summary>
    ///     <exception cref="NotSupportedException">Throws if a accelerometer or orientation sensor is missing</exception>
    /// </summary>
    private void Calibrate()
    {
        if (HasAccelerometer && HasOrientationSensor)
        {
            _initialAcceleration = _accelerometer.GetCurrentReading();
            _initialAccelerationVector = _initialAcceleration.BuildVector();

            _initialOrientation = _orientationSensor.GetCurrentReading();
            _initialRotationMatrix = _initialOrientation.RotationMatrix.BuildMatrix();

            _initialTransformedAccelerationVector =
                _initialRotationMatrix.Transpose().Multiply(_initialAccelerationVector);
        }
        else
        {
            throw new NotSupportedException("Either no accelerometer or orientation sensor available.");
        }
    }

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

        foreach (var calcucationPair in _calculators)
        {
            if (MotionDataNeeds.HasFlag(calcucationPair.Key))
            {
                calcucationPair.Value.Invoke(newTimestamp);
            }
        }

        MotionDataRefreshed?.Invoke(this);

        // Reset readings
        ResetReadings();
    }

    private void CalculateJoltRot(double newTimestamp)
    {
        Jolt.Timestamp = newTimestamp;
        // todo: implement
    } 

    private void CalculatePoseTrans(double newTimestamp)
    {
        // Integrate velocity and acceleration to get current pose
        var posDeltaT = newTimestamp - Pose.Timestamp;
        var posDeltaTAcc = posDeltaT*posDeltaT*0.5;

        Pose.Trans[0] *= BeatshakeSettings.PositionDamp;
        Pose.Trans[0] += RelAcceleration.Trans[0]*posDeltaTAcc + Velocity.Trans[0]*posDeltaT;

        Pose.Trans[1] *= BeatshakeSettings.PositionDamp;
        Pose.Trans[1] += RelAcceleration.Trans[1]*posDeltaTAcc + Velocity.Trans[1]*posDeltaT;

        Pose.Trans[2] *= BeatshakeSettings.PositionDamp;
        Pose.Trans[2] += RelAcceleration.Trans[2]*posDeltaTAcc + Velocity.Trans[2]*posDeltaT;

        Pose.Timestamp = newTimestamp;
    }

    private void CalculatePoseRot(double newTimestamp)
    {
        Pose.Rot[0] = _currentOrientation.Quaternion.X;
        Pose.Rot[1] = _currentOrientation.Quaternion.Y;
        Pose.Rot[2] = _currentOrientation.Quaternion.Z;

        Pose.Timestamp = newTimestamp;
    }

    private void CalculateVelocityTrans(double newTimestamp)
    {
        // Integrate Acceleration to get velocity
        var velDeltaT = newTimestamp - Velocity.Timestamp;
        Velocity.Trans[0] *= BeatshakeSettings.VelocityDamp;
        Velocity.Trans[0] += RelAcceleration.Trans[0] * velDeltaT;

        Velocity.Trans[1] *= BeatshakeSettings.VelocityDamp;
        Velocity.Trans[1] += RelAcceleration.Trans[1] * velDeltaT;

        Velocity.Trans[2] *= BeatshakeSettings.VelocityDamp;
        Velocity.Trans[2] += RelAcceleration.Trans[2] * velDeltaT;

        Velocity.Timestamp = newTimestamp;
    }

    private void CalculateVelocityRot(double newTimestamp)
    {
        if (_currentGyro != null)
        {
            Velocity.Rot[0] = _currentGyro.AngularVelocityX;
            Velocity.Rot[1] = _currentGyro.AngularVelocityY;
            Velocity.Rot[2] = _currentGyro.AngularVelocityZ;
        }

        Velocity.Timestamp = newTimestamp;
    }

    private void CalculateJoltTrans(double newTimestamp)
    {
        Jolt.Trans[0] = _currentAcceleration.AccelerationX - AbsAcceleration.Trans[0];
        Jolt.Trans[1] = _currentAcceleration.AccelerationY - AbsAcceleration.Trans[1];
        Jolt.Trans[2] = _currentAcceleration.AccelerationZ - AbsAcceleration.Trans[2];
        Jolt.Timestamp = newTimestamp;
    }

    private void CalculateAbsAccelerationTrans(double newTimestamp)
    {
        // Assign Acceleration (no dump needed, cause values are measured, not calculated)
        AbsAcceleration.Trans[0] = _currentAcceleration.AccelerationX;
        AbsAcceleration.Trans[1] = _currentAcceleration.AccelerationY;
        AbsAcceleration.Trans[2] = _currentAcceleration.AccelerationZ;
        AbsAcceleration.Timestamp = newTimestamp;
    }

    private void CalculateAbsAccelerationRot(double newTimestamp)
    {
        // Assign Acceleration (no dump needed, cause values are measured, not calculated)
        //todo: implement
        AbsAcceleration.Timestamp = newTimestamp;
    }

    private void CalculateRelAccelerationTrans(double newTimestamp)
    {
        // Assign Linear Acceleration without gravity (high pass filter)
        if (HasOrientationSensor == false)
        {
            // Revert settings
            BeatshakeSettings.GravityEliminationMode = GravityEliminationMode.Filter;
        }

        if (BeatshakeSettings.GravityEliminationMode == GravityEliminationMode.Orientation)
        {
            var orientation = _currentOrientation.RotationMatrix.BuildMatrix();

            var gravityVector = orientation.Multiply(_initialTransformedAccelerationVector);

            // use the orientation to substract gravity
            RelAcceleration.Trans[0] = _currentAcceleration.AccelerationX - gravityVector[0];
            RelAcceleration.Trans[1] = _currentAcceleration.AccelerationY - gravityVector[1];
            RelAcceleration.Trans[2] = _currentAcceleration.AccelerationZ - gravityVector[2];
        }
        else
        {
            _gravity[0] = Alpha*_gravity[0] + (1 - Alpha)*_currentAcceleration.AccelerationX;
            _gravity[1] = Alpha*_gravity[1] + (1 - Alpha)*_currentAcceleration.AccelerationY;
            _gravity[2] = Alpha*_gravity[2] + (1 - Alpha)*_currentAcceleration.AccelerationZ;

            RelAcceleration.Trans[0] = _currentAcceleration.AccelerationX - _gravity[0];
            RelAcceleration.Trans[1] = _currentAcceleration.AccelerationY - _gravity[1];
            RelAcceleration.Trans[2] = _currentAcceleration.AccelerationZ - _gravity[2];
        }

        RelAcceleration.Timestamp = newTimestamp;
    }

    private void CalculateRelAccelerationRot(double newTimestamp)
    {
        RelAcceleration.Rot = AbsAcceleration.Rot;
        RelAcceleration.Timestamp = newTimestamp;
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
}