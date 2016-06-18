using System;
using Microsoft.Xna.Framework;

namespace Beatshake.Core
{
    public class DataContainer<TDataType>
    {
        public TDataType[] Trans = new TDataType[3];

        public TDataType[] Rot = new TDataType[3];

        // this is the timestamp in milliseconds
        public double Timestamp = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;

        public Tuple<TDataType, TDataType, TDataType> TransAsTuple()
        {
            return Tuple.Create(Trans[0], Trans[1], Trans[2]);
        }

        public Tuple<TDataType, TDataType, TDataType> RotAsTuple()
        {
            return Tuple.Create(Rot[0], Rot[1], Rot[2]);
            
        }
    }

}
