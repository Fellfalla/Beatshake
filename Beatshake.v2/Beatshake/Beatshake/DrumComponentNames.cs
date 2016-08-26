using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake
{
    public enum DrumComponents
    {
        Cowbell = 1,
                  
        Cymbal  = 2,
                  
        Hihat   = 3,
                  
        Kick    = 4,
                  
        Ride    = 5,
                  
        Snare   = 6,
                  
        Tom     = 7

        //public static readonly string Cowbell = "cowbell";

        //public static readonly string Cymbal = "cymbal";

        //public static readonly string Hihat = "hihat";

        //public static readonly string Kick = "kick";

        //public static readonly string Ride = "ride";

        //public static readonly string Snare = "snare";

        //public static readonly string Tom = "tom";

   
    }

    public static class DrumComponentNames {

        public static IEnumerable<string> GetAllNames()
        {
            foreach (var name in Enum.GetNames(typeof(DrumComponents)))
            {
                yield return name.ToLower();
            }
        }

        public static int GetNumberOfComponents()
        {
            return Enum.GetNames(typeof(DrumComponents)).Length;
        }
    }

}
