using System;
using Beatshake.Core;
using Microsoft.Xna.Framework;

namespace Beatshake.DependencyServices
{
    public interface IMotionDataProvider
    {
        DataContainer<double> Pose { get; }

        DataContainer<double> Velocity { get; }

        DataContainer<double> Acceleration { get; }

        bool HasAccellerometer { get; }

        bool HasGyrometer { get; }

        bool HasOrientationSensor { get; }


        /// <summary>
        /// The refresh rate in milliseconds.
        /// </summary>
        uint RefreshRate { get; set; }
        
        uint MinInterval { get; }

        event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
    }
}