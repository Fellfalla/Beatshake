using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Beatshake.Core;
using Beatshake.UWP;


namespace Beatshake.UWP
{

    class AudioPlayerImplementation : IAudioPlayer
    {

        public async Task Play(string audioFile)
        {
            var sample = new MediaElement();
            //sample.Source = new Uri(audioFile);

            StorageFolder Folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Folder = await Folder.GetFolderAsync("MyFolder");
            StorageFile sf = await Folder.GetFileAsync("MyFile.mp3");

            sample.SetSource(await sf.OpenAsync(FileAccessMode.Read), sf.ContentType);
            sample.Play();
        }
    }
}
