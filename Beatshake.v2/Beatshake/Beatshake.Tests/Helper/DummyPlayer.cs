using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;

namespace Beatshake.Tests.ViewModels
{
    class DummyPlayer : IInstrumentPlayer
    {
        public IInstrumentalComponentIdentification Component { get; set; }

        public void Play()
        {
            Trace.WriteLine("Audio played for " + Component.ToString());
        }

        public Task PlayAsync()
        {
            return Task.Factory.StartNew(Play);
        }

        public void PreLoadAudio()
        {
            Trace.WriteLine("Audio Preloaded for " + Component.ToString());
        }

        public Task PreLoadAudioAsync()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

    }
}