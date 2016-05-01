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
        /// 120 BPM -> (1000 * 60) / 120       / 64
        ///             sec per min  120 beats  hit length
        /// </summary>
        public static int InstrumentalCooldown = (1000 * 60) / 120 / 64;

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
    }

    public static class BeatshakeGlobals
    {
        public static string NavigateToDrumKit = "NavigatteToDrumKit";

        public static string NavigateToMainPage = "NavigatteToDrumKit";
    }
}
