using Beatshake.Core;
using Beatshake.DependencyServices;

namespace Beatshake.Tests.ViewModels
{
    class DummyMotionDataProvier : IMotionDataProvider
    {
        public DataContainer<double> Pose { get; }
        public DataContainer<double> Velocity { get; }
        public DataContainer<double> Acceleration { get; }
        public DataContainer<double> AbsoluteAcceleration { get; }
        public DataContainer<double> AccelerationDerivation { get; }
        public bool HasAccelerometer { get; }
        public bool HasGyrometer { get; }
        public bool HasOrientationSensor { get; }

        public uint RefreshRate { get; set; }
        public uint MinInterval { get; }
        public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
    }
}