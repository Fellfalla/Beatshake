using System.Collections.Generic;
using System.Linq;
using Beatshake.Core;
using Xunit;

namespace Beatshake.Tests
{
    public class DropOutStackTests
    {
        [Fact]
        public void StackPopsLastElementWheCapacityReached()
        {
            int numberOfElements = 15;
            int maxCapacity = 5;

            var stack = new DropOutStack<int>(maxCapacity);
            for (int i = 0; i < numberOfElements; i++)
            {
                stack.Push(i);
            }

            //Assert.Equal(maxCapacity, stack.Count);
            var stackItems = stack.ToArray();
            int counter = 0;
            for (int i = numberOfElements - 1; i > numberOfElements - maxCapacity; i--, counter++)
            {
                Assert.Equal(i, stackItems[counter]);
                Assert.Equal(maxCapacity, stack.Count);
            }

            for (int i = numberOfElements - 1; i > numberOfElements - maxCapacity; i--)
            {
                Assert.Equal(i, stack.Pop());
            }
        }

        [Fact]
        public void StackReturnsCorrectEnumeration()
        {
            var expected = new List<int>();
            var soc = new DropOutStack<int>(5);

            for (int i = 0; i <= 10; i++)
            {
                // Push and Add shall not make any difference
                if (i%2 == 0)
                {
                    soc.Push(i);
                }
                else
                {
                    soc.Add(i);
                }
            }

            for (int i = 10; i > 5; i--)
            {
                expected.Add(i);
            }

            var resultList = soc.ToArray();

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], resultList[i]);
            }

        }
    }
}