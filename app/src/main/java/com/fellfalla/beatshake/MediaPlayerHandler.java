package com.fellfalla.beatshake;

import android.content.Context;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.media.SoundPool;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by Markus Weber on 01.06.2015.
 */
public class MediaPlayerHandler {
    Map<String,SoundPool> samplePlayers;
    Context context;
    ArrayList<Integer> mStreamIDs ;
    float actVolume, maxVolume, volume;
    AudioManager audioManager;
    int soundID;

    MediaPlayerHandler(Context context){
        this.context = context;
        mStreamIDs = new ArrayList<>();

        samplePlayers = new HashMap<>(); // todo: auswählen zwischen: HashMap LinkedHashMap Hashtable
        // AudioManager audio settings for adjusting the volume
        audioManager = (AudioManager) context.getSystemService(context.AUDIO_SERVICE);
        actVolume = (float) audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
        maxVolume = (float) audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
        volume = actVolume / maxVolume;
    }
    void addSample(String sampleName, int sampleId) {
//        MediaPlayer mp = MediaPlayer.create(this.context, sampleId);
//        mp.setAudioStreamType(AudioManager.STREAM_MUSIC);
//        mp.setOnCompletionListener(new MediaPlayer.OnCompletionListener() {
//            @Override
//            public void onCompletion(MediaPlayer mp) {
//
//            }
//        });
//        samplePlayers.put(sampleName, MediaPlayer.create(this.context ,sampleId));
        SoundPool soundPool = loadSoundPool();
        soundID = soundPool.load(context, sampleId, 1);
        samplePlayers.put(sampleName, soundPool);
    }

    void playSound(String soundName){
        mStreamIDs.add(samplePlayers.get(soundName).play(soundID, volume, volume, 1, 0, 1f));
    }

    void stopSound(String soundName){

    }

    SoundPool loadSoundPool(){
        SoundPool soundPool = new SoundPool(10, AudioManager.STREAM_MUSIC, 0);
        soundPool.setOnLoadCompleteListener(new SoundPool.OnLoadCompleteListener() {
            @Override
            public void onLoadComplete(SoundPool soundPool, int sampleId, int status) {
                //loaded = true;
            }
        });
        return soundPool;
    }

}
