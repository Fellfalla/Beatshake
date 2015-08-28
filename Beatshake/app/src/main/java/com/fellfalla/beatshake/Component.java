package com.fellfalla.beatshake;

import android.content.Context;
import android.media.AudioManager;
import android.media.SoundPool;
import android.media.ToneGenerator;
import android.util.Log;
import android.widget.CheckBox;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;

/**
 * Diese Klasse spiegelt eine Komponente eines Instrumentes wie z.b. eine Snare wieder
 * Created by Markus on 11.06.2015.
 */
public class Component {
    Integer StreamID ;
    SoundPool soundPool;
    ArrayList<Integer> soundIDs ;
    float actVolume, maxVolume, volume;
    AudioManager audioManager;
    private Context context;
    public ArrayList<CheckBox> activatedAxes;

    public Component(String name, float sensitivity, Context context){
        ComponentName = name;
        Sensitivity = sensitivity;
        this.context = context;
        activatedAxes = new ArrayList<>();

        soundIDs = new ArrayList<>();
        soundPool = new SoundPool(2, AudioManager.STREAM_MUSIC, 0);

        // AudioManager audio settings for adjusting the volume
        audioManager = (AudioManager) context.getSystemService(Context.AUDIO_SERVICE);
        actVolume = (float) audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
        maxVolume = (float) audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
        volume = actVolume / maxVolume;
    }
    private int Sample;
    private float Sensitivity;
    private long LastPlayed;
    private String ComponentName;

    public float getSensitivity() {
        return Sensitivity;
    }

    public void setSensitivity(float sensitivity) {
        Sensitivity = sensitivity;
    }

    public long getLastPlayed() {
        return LastPlayed;
    }

    public void setLastPlayed(long lastPlayed) {
        LastPlayed = lastPlayed;
    }

    public String getName() {
        return ComponentName;
    }

    public void setName(String name) {
        ComponentName = name;
    }

    public int getSample() {
        return Sample;
    }

    public void setSample(int sample) {
        soundIDs.add(soundPool.load(context, sample, 1));
        Sample = sample;
    }

    /**
     * Spielt die aktuelle Komponente
     */
    public void Play(){
        if (soundIDs.get(0) != null){
            setLastPlayed(System.currentTimeMillis());
            Random random = new Random();
            float minX = .995f;
            float maxX = 1.005f;
            float randomFloatRate = random.nextFloat() * (maxX - minX) + minX;
            int newStreamID = soundPool.play(soundIDs.get(0), volume, volume, 1, 0, randomFloatRate);
            if (StreamID != null) {
                soundPool.stop(StreamID);
            }
            StreamID = newStreamID;
        }
    }

    public boolean isAxeActivated(List<Axes> axes) { // Der Name der Methode passt nicht wirklich
        for (CheckBox activatedAxe : activatedAxes){
            for (Axes axe : axes){
                if (axe == activatedAxe.getTag(R.string.view_axe))
                {
                    if (activatedAxe.isChecked()){  // schaue ob die Axe im Kontropllkästchen aktiviert ist
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
