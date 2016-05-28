using System.Diagnostics;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;

namespace Beatshake.Tests.ViewModels
{
    class DummyPlayer : IInstrumentPlayer
    {
        public Task Play(object audioData)
        {
            Trace.WriteLine("Audio played for " + audioData.ToString());
            return new Task(() => {});
        }

        public Task<object> PreLoadAudio(IInstrumentalComponentIdentification component)
        {
            Trace.WriteLine("Audio Preloaded for " + component.ToString());
            return new Task<object>(component.ToString);
        }
    }
}