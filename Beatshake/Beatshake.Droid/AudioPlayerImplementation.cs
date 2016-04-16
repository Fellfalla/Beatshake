using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Media;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Beatshake.Core;
using Beatshake.UWP;
using Java.IO;


namespace Beatshake.UWP
{

    public class AudioPlayerImplementation : IAudioPlayer
    {
        SoundPool soundPool;
        List<int> soundIDs;
        float actVolume, maxVolume, volume;
        AudioManager audioManager;

        public AudioPlayerImplementation()
        {
            soundPool = new SoundPool(2, Stream.Music, 0);
            audioManager = AudioManager.FromContext(Application.Context);
            actVolume = audioManager.GetStreamVolume(Stream.Music);
            maxVolume = audioManager.GetStreamMaxVolume(Stream.Music);
            volume = actVolume / maxVolume;

        }

        public async Task Play(string audioFile)
        {
            Toast.MakeText(Application.Context, "Not supported yet", ToastLength.Long);

            //if (soundIDs[0] != null)
            //{
            //    setLastPlayed(System.currentTimeMillis());
            //    Random random = new Random();
            //    float minX = .995f;
            //    float maxX = 1.005f;
            //    float randomFloatRate = random.nextFloat() * (maxX - minX) + minX;
            //    int newStreamID = soundPool.play(soundIDs.get(0), volume, volume, 1, 0, randomFloatRate);
            //    if (StreamID != null)
            //    {
            //        soundPool.stop(StreamID);
            //    }
            //    StreamID = newStreamID;
            //}
        }
    }
}
