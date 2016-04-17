using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public class DataContainer<TDataType>
    {
        public TDataType[] Trans = new TDataType[3];

        public TDataType[] Rot = new TDataType[3];

    }

}
