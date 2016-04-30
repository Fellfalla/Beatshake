using System.Threading.Tasks;
using Beatshake.Core;

namespace Beatshake.DependencyServices
{
    public interface IInstrumentPlayer
    {
        Task Play(object audioData);

        Task<object> PreLoadAudio(IInstrumentalComponentIdentification component);
    }
}
