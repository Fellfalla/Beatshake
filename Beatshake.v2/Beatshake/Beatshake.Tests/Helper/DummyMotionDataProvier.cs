using Beatshake.Core;
using Beatshake.DependencyServices;

namespace Beatshake.Tests.Helper
{
    internal class DummyMotionDataProvier : IMotionDataProvider
    {
        public DataContainer<double> Pose { get; }
        public DataContainer<double> Velocity { get; }
        public DataContainer<double> RelAcceleration { get; }
        public DataContainer<double> AbsAcceleration { get; }
        public DataContainer<double> Jolt { get; }
        public MotionData MotionDataNeeds { get; set; }

        public bool HasAccelerometer { get; }
        public bool HasGyrometer { get; }
        public bool HasOrientationSensor { get; }
        public DropOutStack<double> Timestamps { get; }

        public uint RefreshRate { get; set; }
        public uint MinInterval { get; }
        public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
    }
}