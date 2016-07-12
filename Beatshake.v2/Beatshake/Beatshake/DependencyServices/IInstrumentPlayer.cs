using System;
using System.Threading.Tasks;
using Beatshake.Core;

namespace Beatshake.DependencyServices
{
    public interface IInstrumentPlayer : IDisposable
    {
        IInstrumentalComponentIdentification Component { get; set; }

        void Play();

        Task PlayAsync();

        void PreLoadAudio();

        Task PreLoadAudioAsync();


    }
}
