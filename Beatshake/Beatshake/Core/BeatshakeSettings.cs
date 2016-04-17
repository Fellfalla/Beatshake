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
        /// </summary>
        public static int InstrumentalCooldown = 1000/64;

        /// <summary>
        /// This value gives the max amount of time, that a instrumental play request can be sent,
        /// before cooldown has finished. This request will be executed, after the cooldown has finished.
        /// </summary>
        public static int MaxInstrumentalRequestDelay = InstrumentalCooldown / 3;

        /// <summary>
        /// Sets the refreshing interval of sensors in milliseconds .
        /// Depending on the target system this value might only can be a approximated.
        /// Setting very low values as 0 means the minimum reading interval.
        /// </summary>
        public static int SensorRefreshInterval = 0;

    }
}
