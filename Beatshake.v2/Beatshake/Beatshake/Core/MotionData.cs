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
        None                        = 0,                                                    // 0b00000000000
        AbsAccelerationTrans        = 4,                                                    // 0b00000000100
        AbsAccelerationRot          = 8,                                                    // 0b00000001000
        RelAccelerationTrans        = 16    |   AbsAccelerationTrans,                       // 0b00000010000
        RelAccelerationRot          = 32    |   AbsAccelerationRot,                         // 0b00000100000
        JoltTrans                   = 1     |   AbsAccelerationTrans,                       // 0b00000000001
        JoltRot                     = 2     |   AbsAccelerationRot,                         // 0b00000000010
        VelocityTrans               = 64    |   RelAccelerationTrans,                       // 0b00001000000
        VelocityRot                 = 128   |   RelAccelerationRot,                         // 0b00010000000
        PoseTrans                   = 256   |   VelocityTrans   |   RelAccelerationTrans,   // 0b00100000000
        PoseRot                     = 512   |   VelocityRot     |   RelAccelerationRot,     // 0b01000000000
        All                         = 1023                                                  // 1023 in binary : 0b01111111111
    }
}