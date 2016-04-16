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

    class InstrumentPlayerImplementation : IInstrumentPlayer
    {


        public async Task Play(IInstrumentalComponentIdentification component)
        {
            var sample = new MediaElement();
            //sample.Source = new Uri(audioFile);

            StorageFolder Folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Folder = await Folder.GetFolderAsync("Assets");
            Folder = await Folder.GetFolderAsync(component.ContainingInstrument.Kit);
            StorageFile sf = await Folder.GetFileAsync( component.Name + component.Number + ".wav");

            sample.SetSource(await sf.OpenAsync(FileAccessMode.Read), sf.ContentType);
            sample.Play();
        }
    }
}
