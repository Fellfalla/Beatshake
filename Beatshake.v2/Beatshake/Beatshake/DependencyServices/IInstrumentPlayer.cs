using System.Threading.Tasks;
using Beatshake.Core;

namespace Beatshake.DependencyServices
{
    public interface IInstrumentPlayer
    {
        void Play(object audioData);

        Task PlayAsync(object audioData);

        object PreLoadAudio(IInstrumentalComponentIdentification component);

        Task<object> PreLoadAudioAsync(IInstrumentalComponentIdentification component);


    }
}
