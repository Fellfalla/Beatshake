package com.fellfalla.beatshake;

import android.content.Context;
import android.media.AudioManager;
import android.media.SoundPool;
import android.os.Handler;
import android.os.Handler.Callback;
import android.os.Message;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Date;

/**
 * Created by Markus Weber on 01.06.2015.
 * Die Jukebox
 */
public class MediaPlayerHandler {
    Map<String,Integer> soundIDs;
    Context context;
    ArrayList<Integer> mStreamIDs ;
    float actVolume, maxVolume, volume;
    AudioManager audioManager;
    SoundPool soundPool;
    long starttime = 0;

    MediaPlayerHandler(Context context){
        this.context = context;
        mStreamIDs = new ArrayList<>();
        soundIDs = new HashMap<>(); // todo: auswählen zwischen: HashMap LinkedHashMap Hashtable

        soundPool = new SoundPool(50, AudioManager.STREAM_MUSIC, 0 );

        // AudioManager audio settings for adjusting the volume
        audioManager = (AudioManager) context.getSystemService(Context.AUDIO_SERVICE);
        actVolume = (float) audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
        maxVolume = (float) audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
        volume = actVolume / maxVolume;
    }

    void addSample(String sampleName, int sampleId) {
        int soundID = soundPool.load(context, sampleId, 1);
        soundIDs.put(sampleName, soundID);
    }

    void playSound(int soundID){
        //todo : priorität mit übergeben
        int miliSVerzoegerung = 1000/8;
        if (starttime + miliSVerzoegerung < System.currentTimeMillis() ) {
            starttime = System.currentTimeMillis();
            mStreamIDs.add(soundPool.play(soundID, volume, volume, 1, 0, 1f));
        }
    }
    void playSound(List<Integer> soundIDs){
        //todo : priorität mit übergeben
        //todo: jedes instrument braucht eigenen timer, oder play sound fügt samples zu liste hinzu, die dann von einerm Timerevent ausgeführt wird
        int miliSVerzoegerung = 1000/8;
        if (starttime + miliSVerzoegerung < System.currentTimeMillis() ) {
            for(int soundID : soundIDs) {
                starttime = System.currentTimeMillis();
                mStreamIDs.add(soundPool.play(soundID, volume, volume, 1, 0, 1f));
            }
        }
    }

    void stopSound(String soundName){

    }

    SoundPool loadSoundPool(){

        soundPool.setOnLoadCompleteListener(new SoundPool.OnLoadCompleteListener() {
            @Override
            public void onLoadComplete(SoundPool soundPool, int sampleId, int status) {
                //loaded = true;
            }
        });
        return soundPool;
    }

}
