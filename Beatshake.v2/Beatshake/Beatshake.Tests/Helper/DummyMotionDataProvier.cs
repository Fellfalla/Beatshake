using Beatshake.Core;
using Beatshake.DependencyServices;

namespace Beatshake.Tests.ViewModels
{
    class DummyMotionDataProvier : IMotionDataProvider
    {
        public DataContainer<double> Pose { get; }
        public DataContainer<double> Velocity { get; }
        public DataContainer<double> Acceleration { get; }
        public bool HasAccellerometer { get; }
        public bool HasGyrometer { get; }
        public bool HasOrientationSensor { get; }

        public uint RefreshRate { get; set; }
        public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
    }
}