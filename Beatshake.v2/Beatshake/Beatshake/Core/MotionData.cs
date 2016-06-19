using System;

namespace Beatshake.Core
{
    /// <summary>
    ///     The calculations of those values will be performed in raising order.
    ///     This means that the values are critical for the correctness of some calculations which depens on other calculations
    /// </summary>
    [Flags]
    public enum MotionData
    {
        AbsAccelerationTrans        = 4,
        AbsAccelerationRot          = 8,
        RelAccelerationTrans        = 16    |   AbsAccelerationTrans,
        RelAccelerationRot          = 32    |   AbsAccelerationRot,
        JoltTrans                   = 1     |   AbsAccelerationTrans,
        JoltRot                     = 2     |   AbsAccelerationRot,
        VelocityTrans               = 64    |   RelAccelerationTrans,
        VelocityRot                 = 128   |   RelAccelerationRot,
        PoseTrans                   = 256   |   VelocityTrans   |   RelAccelerationTrans,
        PoseRot                     = 512   |   VelocityRot     |   RelAccelerationRot
    }
}