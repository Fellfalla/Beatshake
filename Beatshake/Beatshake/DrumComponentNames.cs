using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake
{
    public static class DrumComponentNames
    {
        public static readonly string Cowbell = "cowbell";

        public static readonly string Cymbal = "cymbal";

        public static readonly string Hihat = "hihat";

        public static readonly string Kick = "kick";

        public static readonly string Ride = "ride";

        public static readonly string Snare = "snare";

        public static readonly string Tom = "tom";

        public static IEnumerable<string> GetAllNames()
        {
            foreach (var field in typeof(DrumComponentNames).GetRuntimeFields())
            {
                yield return field.GetValue(null).ToString();
            }
            
        }
    }
}
