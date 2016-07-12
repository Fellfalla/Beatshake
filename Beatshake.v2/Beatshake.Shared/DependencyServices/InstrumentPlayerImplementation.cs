using System;
using System.IO;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;
using Microsoft.Practices.Unity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;


class InstrumentPlayerImplementation : IInstrumentPlayer, IDisposable
{
    //private readonly InstancePool<SoundEffect> _mediaElementPool = new InstancePool<SoundEffect>(200);
    private Random random = new Random();
    private SoundEffect _soundEffect;
    private IInstrumentalComponentIdentification _component;
    private bool _isAudioPreloaded;

    [InjectionConstructor]
    public InstrumentPlayerImplementation()
    {
        
    }

    public InstrumentPlayerImplementation(IInstrumentalComponentIdentification component)
    {
        Component = component;
    }

    public IInstrumentalComponentIdentification Component
    {
        get { return _component; }
        set
        {
            if (_component != value)
            {
                _component = value;

                if (_isAudioPreloaded)
                {
                    PreLoadAudio(); // load again if audio component has changed
                }
            }
        }
    }

    public void Play()
    {
        
        //var soundElement = _mediaElementPool.GetInstance(out nr);

        //var transmitter = audioData as AudioTransmitter;

        //if (transmitter != null)
        if (_component == null || string.IsNullOrWhiteSpace(_component.Name))
        {
            throw new ArgumentNullException("Sound media has not been set");
        }

        if (_soundEffect == null)
        {
            PreLoadAudio();
            if (_soundEffect == null)
            {
                throw new FileNotFoundException("The audiofile was not loaded.", GetFilePath());
            }
            //soundElement.Play();
        }

        //var soundElement = (MediaElement) audioData;
        //if (soundElement != null) await soundElement.Dispatcher.RunAsync(CoreDispatcherPriority.High, soundElement.Play);
        //transmitter.Stream.Seek(0);

        //var soundElement = SoundEffect.FromStream(transmitter.Stream.AsStreamForRead());
        _soundEffect.Play(1f,
            (float)random.NextDouble(-BeatshakeSettings.RandomPitchRange, BeatshakeSettings.RandomPitchRange),
            (float)random.NextDouble(-BeatshakeSettings.RandomPan, BeatshakeSettings.RandomPan));
            //transmitter.SoundEffect.Play(1f, 
            //    (float) random.NextDouble(-BeatshakeSettings.RandomPitchRange, BeatshakeSettings.RandomPitchRange), 
            //    (float)random.NextDouble(-BeatshakeSettings.RandomPan, BeatshakeSettings.RandomPan));
            //soundElement.SetSource(transmitter.Stream, transmitter.StorageFile.ContentType);
        //_mediaElementPool.Unlock(nr);
    }

    public async Task PlayAsync()
    {
        await Task.Factory.StartNew(Play);
    }

    private string GetFilePath()
    {
        string fileName = Component.Name + Component.Number + ".wav";

        string path = string.Format(@"Assets\{0}\{1}", Component.ContainingInstrument.Kit, fileName);

        return path;
    }

    private bool IsComponentIdentifierValid()
    {
        return Component != null &&
            Component.ContainingInstrument != null &&
               !string.IsNullOrWhiteSpace(Component.ContainingInstrument.Kit) &&
               !string.IsNullOrWhiteSpace(Component.Name);
    }

    public void PreLoadAudio()
    {
        // todo: change accessing this method directly to Property of type bool and set loading mode there
        if (!IsComponentIdentifierValid())
        {
            return;
        }

        // return prefetched stream
        //var transmitter = new AudioTransmitter();
        try
        {
            // bug: this line causes a crash at backNavigationCommand
            _soundEffect = SoundEffect.FromStream(TitleContainer.OpenStream(GetFilePath()));
            _isAudioPreloaded = true;

            //transmitter.SoundEffect = SoundEffect.FromStream(TitleContainer.OpenStream(path));
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e);
        }
        //return transmitter;
    }

    /// <summary>
    /// Source: http://blogs.msdn.com/b/ashtat/archive/2010/06/03/soundeffect-creation-in-xna-game-studio-4.aspx
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public async Task PreLoadAudioAsync()
    {
        await Task.Factory.StartNew(PreLoadAudio);
    }

    public void Dispose()
    {
        if (_soundEffect != null)
        {
            _soundEffect.Dispose();
            _isAudioPreloaded = false;
        }
    }
}