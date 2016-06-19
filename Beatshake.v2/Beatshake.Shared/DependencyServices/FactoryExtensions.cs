using Windows.Devices.Sensors;
using MathNet.Numerics.LinearAlgebra;

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
            rotationMatrix.M33
        });
    }

    public static Vector<double> BuildVector(this AccelerometerReading accelerometerReading)
    {
        return Vector<double>.Build.Dense(new[]
        {
            accelerometerReading.AccelerationX,
            accelerometerReading.AccelerationY,
            accelerometerReading.AccelerationZ
        });
    }
}