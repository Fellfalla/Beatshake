using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public static class BeatshakeSettings
    {
        /// <summary>
        /// The cooldown a instrumental component needs to be played again in milliseconds.
        /// 120 BPM -> (1000 * 60) / 120       / 32
        ///             sec per min  120 beats  hits length
        /// </summary>
        public static int InstrumentalCooldown = (1000 * 60) / 120 / 32;

        /// <summary>
        /// This value gives the max amount of time, that a instrumental play request can be sent,
        /// before cooldown has finished. This request will be executed, after the cooldown has finished.
        /// </summary>
        public static int MaxInstrumentalRequestDelay = InstrumentalCooldown / 3;

        /// <summary>
        /// Sets the refreshing interval of sensors in milliseconds .
        /// Depending on the target system this value might only can be a approximated.
        /// Setting very low values as 0 means the minimum/fastest reading interval.
        /// </summary>
        public static uint SensorRefreshInterval = 50;

        /// <summary>
        /// This specifies the + and - Pitch for audio replay.
        /// Using this makes the sound appearing more naturally
        /// </summary>
        public static double RandomPitchRange = 0.02;

        /// <summary>
        /// This specifies the random variation of the Pan while playing sound.
        /// </summary>
        public static double RandomPan = 1;

        /// <summary>
        /// This is the amount of subsamples for calculating the integral of a analytic function in a given interval.
        /// </summary>
        public static int IntegralPrecision = 30;
    
        /// <summary>
        /// This is the amount of points which are used to create a function aproximation upon measure values.
        /// </summary>
        public static int SamplePoints = InstrumentalCooldown;

        /// <summary>
        /// The device accelleration state is every measure step damped by this value to avoid 
        /// big error accumulations
        /// </summary>
        public static double AccelerationDamp = 1;

        /// <summary>
        /// The devices velocity is every measure step damped by this value to avoid 
        /// big error accumulations
        /// </summary>
        public static double VelocityDamp = 0.95;

        /// <summary>
        /// The devices position is every measure step damped by this value to avoid 
        /// big error accumulations
        /// </summary>
        public static double PositionDamp = 0.92;

        /// <summary>
        /// This enum gives modi to specify the strategy for removing 
        /// gravitional influence from the devices acceleration data.
        /// </summary>
        public static GravityEliminationMode GravityEliminationMode = GravityEliminationMode.Orientation;
    }
}