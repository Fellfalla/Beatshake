using System.Collections.Generic;
using System.Linq;
using Beatshake.Core;
using Xunit;

namespace Beatshake.Tests
{
    public class LanguageTests
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
            for (int i = numberOfElements-1; i > numberOfElements- maxCapacity; i--, counter++)
            {
                Assert.Equal(i, stackItems[counter]);
                Assert.Equal(maxCapacity, stack.Count);
            }

            for (int i = numberOfElements-1; i > numberOfElements- maxCapacity; i--)
            {
                Assert.Equal(i,stack.Pop());
            }
        }
    }
}