using System;
using Beatshake.Core;
using Microsoft.Xna.Framework;

namespace Beatshake.DependencyServices
{
    public interface IMotionDataProvider
    {
        DataContainer<double> Pose { get; }

        DataContainer<double> Velocity { get; }

        DataContainer<double> RelAcceleration { get; }

        DataContainer<double> AbsAcceleration { get; }

        DataContainer<double> Jolt { get; }

        MotionData MotionDataNeeds { get; set; }

        bool HasAccelerometer { get; }

        bool HasGyrometer { get; }

        bool HasOrientationSensor { get; }

        DropOutStack<double> Timestamps { get; }

        /// <summary>
        /// The refresh rate in milliseconds.
        /// </summary>
        uint RefreshRate { get; set; }
        
        uint MinInterval { get; }

        event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
    }
}