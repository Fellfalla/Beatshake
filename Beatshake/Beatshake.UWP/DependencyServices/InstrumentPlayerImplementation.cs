using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Beatshake.Core;
using Beatshake.UWP;


namespace Beatshake.UWP
{

    class InstrumentPlayerImplementation : IInstrumentPlayer
    {
        public async Task Play(object audioData)
        {
            var soundElement = new MediaElement();
            var transmitter = audioData as AudioTransmitter;
            
            if (transmitter != null)
            {
                soundElement.SetSource(transmitter.Stream, transmitter.StorageFile.ContentType);
                soundElement.Play();
            }
            //var soundElement = (MediaElement) audioData;
            //if (soundElement != null) await soundElement.Dispatcher.RunAsync(CoreDispatcherPriority.High, soundElement.Play);
            else
            {
                throw new ArgumentNullException("Sound media has not been set");
            }
        }

        public async Task<object> PreLoadAudio(IInstrumentalComponentIdentification component)
        {
            if (component.ContainingInstrument == null ||
                string.IsNullOrWhiteSpace(component.ContainingInstrument.Kit) ||
                string.IsNullOrWhiteSpace(component.Name))
            {
               
                return null;
            }

            var soundElement = new MediaElement();
            //sample.Source = new Uri(audioFile);
            StorageFolder Folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Folder = await Folder.GetFolderAsync("Assets");
            Folder = await Folder.GetFolderAsync(component.ContainingInstrument.Kit);
            StorageFile sf = await Folder.GetFileAsync(component.Name + component.Number + ".wav");
            var stream = (await sf.OpenAsync(FileAccessMode.Read)).CloneStream();
            //soundElement.SetSource( await sf.OpenAsync(FileAccessMode.Read), sf.ContentType);
            
            return new AudioTransmitter(stream, sf);
        }

    }

    class AudioTransmitter
    {
        public AudioTransmitter(IRandomAccessStream stream, StorageFile storageFile)
        {
            Stream = stream;
            StorageFile = storageFile;
        }

        public IRandomAccessStream Stream;

        public StorageFile StorageFile;
    }
}
