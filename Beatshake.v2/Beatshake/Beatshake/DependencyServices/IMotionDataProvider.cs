using System;
using Beatshake.Core;

namespace Beatshake.DependencyServices
{
    public interface IMotionDataProvider
    {
        DataContainer<double> Pose { get; }

        DataContainer<double> Velocity { get; }

        DataContainer<double> Acceleration { get; }

        /// <summary>
        /// The refresh rate in milliseconds.
        /// </summary>
        int RefreshRate { get; set; }

        event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
    }
}