package com.fellfalla.beatshake;

import android.app.Activity;
import android.content.Context;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.media.SoundPool;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;


public class MainActivity extends Activity {

    private SoundPool soundPool;
    private static MediaPlayer mediaPlayer;
    private int soundID;
    boolean plays = false, loaded = false;
    float actVolume, maxVolume, volume;
    AudioManager audioManager;
    int counter;
    ArrayList<Integer> mStreamIDs ;

    MediaPlayerHandler jukebox;


    /**
     * Called when the activity is first created.
     */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //set the layout of the Activity
        setContentView(R.layout.activity_main);

        // AudioManager audio settings for adjusting the volume
        audioManager = (AudioManager) getSystemService(AUDIO_SERVICE);
        actVolume = (float) audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
        maxVolume = (float) audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
        volume = actVolume / maxVolume;

        //Hardware buttons setting to adjust the media sound
        this.setVolumeControlStream(AudioManager.STREAM_MUSIC);

        // the counter will help us recognize the stream id of the sound played  now
        mStreamIDs = new ArrayList<>();
        counter = 0;

        // Load the sounds
        soundPool = new SoundPool(10, AudioManager.STREAM_MUSIC, 0);
        soundPool.setOnLoadCompleteListener(new SoundPool.OnLoadCompleteListener() {
            @Override
            public void onLoadComplete(SoundPool soundPool, int sampleId, int status) {
                loaded = true;
            }
        });
        soundID = soundPool.load(this, R.raw.beat, 1);

        jukebox = new MediaPlayerHandler(getApplicationContext());
        jukebox.addSample("beat", R.raw.beat);
        loaded = true;
    }

    public void playSound(View v) {
//        int tries=0;
//        while(true) {// Is the sound loaded
//            if(loaded){
//                mStreamIDs.add(soundPool.play(soundID, volume, volume, 1, 0, 1f));
//                Toast.makeText(this, "Played sound", Toast.LENGTH_SHORT).show();
//                plays = true;
//                break;
//            }
//            else {
//                if (tries>500) {
//                    break;
//                }
//                else{
//                    tries++;
//                }
//            }
//        }
        jukebox.playSound("beat");
}

    public void playLoop(View v) {
        // Is the sound loaded does it already play?
        int tries=0;
        while(true) {// Is the sound loaded
            if(loaded){
                // the sound will play for ever if we put the loop parameter -1
                soundPool.play(soundID, volume, volume, 1, -1, 1f);
                counter = counter++;
                Toast.makeText(this, "Plays loop", Toast.LENGTH_SHORT).show();
                plays = true;
                break;
            }
            else {
                if (tries>500) {
                    break;
                }
                else{
                    tries++;
                }
            }

        }
    }

    public void pauseSound(View v) {
        for (int id : mStreamIDs) {
            soundPool.pause(id);
        }
            //soundID = soundPool.load(this, R.raw.beat, counter);
            Toast.makeText(this, "Pause sound", Toast.LENGTH_SHORT).show();
    }

    public void stopSound(View v) {
            for (int id : mStreamIDs) {
                soundPool.stop(id);
            }
            //soundID = soundPool.load(this, R.raw.beat, counter);
            Toast.makeText(this, "Stop sound", Toast.LENGTH_SHORT).show();
        jukebox.stopSound("beat");
        }
    }
