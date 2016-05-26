using System;

namespace Beatshake.Core
{
    public class IncompleteInitializationException : Exception
    {
        public IncompleteInitializationException()
        {
            
        }

        public IncompleteInitializationException(params string[] missingInitializationSteps)
        {
            MissingInizializationSteps = missingInitializationSteps;
        }

        public string[] MissingInizializationSteps;
    }
}