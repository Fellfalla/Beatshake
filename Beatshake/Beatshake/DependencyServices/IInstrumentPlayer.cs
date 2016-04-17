using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public interface IInstrumentPlayer
    {
        Task Play(object audioData);

        Task<object> PreLoadAudio(IInstrumentalComponentIdentification component);
    }
}
