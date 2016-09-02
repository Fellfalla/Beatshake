using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Beatshake.Core;
using Xunit;

namespace Beatshake.Tests
{
    public class DataContainerTests
    {
        [Fact]
        public void DataContainerShallReturnCorrectContent()
        {
            DataContainer.HistorySize = 5;
            List<int> expected = new List<int>();

            var container = new DataContainer<int>();

            for (int i = 0; i <= 10; i++)
            {
                container.XTrans.Add(i);
                container.YTrans.Add(i);
                container.ZTrans.Add(i);
                container.XRot.Add(i);
                container.YRot.Add(i);
                container.ZRot.Add(i);
            }

            for (int k = 0; k < 5; k++)
            {
                for (int i = 10; i > 5; i--)
                {
                    expected.Add(i);
                }
            }
            //Console.WriteLine(string.Join(", ", expected));
            Trace.WriteLine(string.Join(", ", expected));
            var result = container.ToArray();

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result[i]);
            }
        }
    }
}