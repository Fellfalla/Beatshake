using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation.Diagnostics;
using Windows.Media.Effects;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Buffer = Windows.Storage.Streams.Buffer;


class InstrumentPlayerImplementation : IInstrumentPlayer
{
    //private readonly InstancePool<SoundEffect> _mediaElementPool = new InstancePool<SoundEffect>(200);
    private Random random = new Random();
    public async Task Play(object audioData)
    {
        
        int nr;
        //var soundElement = _mediaElementPool.GetInstance(out nr);

        var transmitter = audioData as AudioTransmitter;

        if (transmitter != null)
        {
            //transmitter.Stream.Seek(0);
            
            //var soundElement = SoundEffect.FromStream(transmitter.Stream.AsStreamForRead());
            
            transmitter.SoundEffect.Play(1f, 
                (float) random.NextDouble(-BeatshakeSettings.RandomPitchRange, BeatshakeSettings.RandomPitchRange), 
                (float)random.NextDouble(-BeatshakeSettings.RandomPan, BeatshakeSettings.RandomPan));
            //soundElement.SetSource(transmitter.Stream, transmitter.StorageFile.ContentType);
            //soundElement.Play();
        }
        //var soundElement = (MediaElement) audioData;
        //if (soundElement != null) await soundElement.Dispatcher.RunAsync(CoreDispatcherPriority.High, soundElement.Play);
        else
        {
            throw new ArgumentNullException("Sound media has not been set");
        }
        //_mediaElementPool.Unlock(nr);
    }

    /// <summary>
    /// Source: http://blogs.msdn.com/b/ashtat/archive/2010/06/03/soundeffect-creation-in-xna-game-studio-4.aspx
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public async Task<object> PreLoadAudio(IInstrumentalComponentIdentification component)
    {
        if (component.ContainingInstrument == null ||
            string.IsNullOrWhiteSpace(component.ContainingInstrument.Kit) ||
            string.IsNullOrWhiteSpace(component.Name))
        {
            return null;
        }

        string fileName = component.Name + component.Number + ".wav";

        // Load file into stream
        StorageFolder Folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        Folder = await Folder.GetFolderAsync("Assets");
        Folder = await Folder.GetFolderAsync(component.ContainingInstrument.Kit);
        StorageFile sf = await Folder.GetFileAsync(fileName);
        var stream = await sf.OpenStreamForReadAsync();

        // prefetch stream into memory
        //IBuffer buffer = new Buffer((uint) stream.Size);
        //await stream.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.ReadAhead);
        //var memoryStream = new InMemoryRandomAccessStream();
        //await memoryStream.WriteAsync(buffer);

        string path = string.Format(@"Assets\{0}\{1}", component.ContainingInstrument.Kit, fileName);
        // return prefetched stream
        var transmitter = new AudioTransmitter();
        transmitter.SoundEffect = SoundEffect.FromStream(TitleContainer.OpenStream(path));
        //if (transmitter.SoundEffect == null)
        //{
        //    var buffer = new byte[stream.Length];
        //    stream.Read(buffer, 0, buffer.Length);
        //    transmitter.SoundEffect = new SoundEffect(buffer, 44000, AudioChannels.Stereo);
        //}
        return transmitter;
    }
}

class AudioTransmitter
{
    public AudioTransmitter()
    {

    }

    public IRandomAccessStream Stream;

    public IBuffer Buffer;

    public SoundEffect SoundEffect;

    public StorageFile StorageFile;
}