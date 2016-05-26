using Beatshake.Core;

namespace Beatshake.ExtensionMethods
{
    public static class InterfaceExtensions
    {
        public static string ToString(this IInstrumentalComponentIdentification component)
        {
            return component.Name + component.Number;
        }
    }
}