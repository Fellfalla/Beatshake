using System;
using System.Collections;
using System.Collections.Generic;

namespace Beatshake.Core
{
    public class DataContainer<TDataType> : IEnumerable<TDataType>
    {
        public readonly DropOutStack<TDataType> XTrans = new DropOutStack<TDataType>(BeatshakeSettings.SamplePoints);
        public readonly DropOutStack<TDataType> YTrans = new DropOutStack<TDataType>(BeatshakeSettings.SamplePoints);
        public readonly DropOutStack<TDataType> ZTrans = new DropOutStack<TDataType>(BeatshakeSettings.SamplePoints);

        public readonly DropOutStack<TDataType> XRot = new DropOutStack<TDataType>(BeatshakeSettings.SamplePoints);
        public readonly DropOutStack<TDataType> YRot = new DropOutStack<TDataType>(BeatshakeSettings.SamplePoints);
        public readonly DropOutStack<TDataType> ZRot = new DropOutStack<TDataType>(BeatshakeSettings.SamplePoints);

        //public TDataType[] Trans = new TDataType[3];

        //public TDataType[] Rot = new TDataType[3];

        // this is the timestamp in milliseconds
        //public DropOutStack<double> Timestamp = new DropOutStack<double>(BeatshakeSettings.SamplePoints)
        //{
        //      (DateTime.Now - DateTime.MinValue).TotalMilliseconds
        //};

        [System.Runtime.CompilerServices.IndexerName("DataStack")]
        public TDataType this[int index]   // Indexer declaration
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.XTrans.Peek();
                    case 1:
                        return this.YTrans.Peek();
                    case 2:    
                        return this.ZTrans.Peek();
                    case 3:    
                        return this.XRot.Peek();
                    case 4:    
                        return this.YRot.Peek();
                    case 5:    
                        return this.ZRot.Peek();
                    default:
                        return default(TDataType);
                }
            }
        }



        //public Tuple<TDataType, TDataType, TDataType> TransAsTuple()
        //{
        //    return Tuple.Create(Trans[0], Trans[1], Trans[2]);
        //}

        //public Tuple<TDataType, TDataType, TDataType> RotAsTuple()
        //{
        //    return Tuple.Create(Rot[0], Rot[1], Rot[2]);

        //}
        public IEnumerator<TDataType> GetEnumerator()
        {
            return new DataContainerEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class DataContainerEnumerator : IEnumerator<TDataType>
        {

            public DataContainerEnumerator(DataContainer<TDataType> dataContainer)
            {
                _container = dataContainer;
                Counter = 0;
            }

            private int Counter { get; set; }

            private DataContainer<TDataType> _container;
            private readonly int _counter;

            public bool MoveNext()
            {
                if (Counter <= 5) // DataContainer has 6 fields, counter starts at 0
                {
                    Counter += 1;
                    return true;
                }
                else
                {
                    Counter = -1;
                    return false;
                }
            }

            public void Reset()
            {
                Counter = 0;
            }

            public TDataType Current
            {
                get { return _container[Counter]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
            }
        }
    }

}
