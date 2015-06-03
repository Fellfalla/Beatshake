package com.fellfalla.beatshake;

import android.annotation.TargetApi;
import android.app.Activity;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.media.SoundPool;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;


public class MainActivity extends Activity implements SensorEventListener {

    boolean plays = false, loaded = false;
    float actVolume, maxVolume, volume;
    AudioManager audioManager;
    int counter;
    ArrayList<Integer> mStreamIDs ;

    MediaPlayerHandler jukebox;
    Drums drumKit1;
    SensorManager sensorManager;
    Sensor mAccelerometer;

    float outputX;
    float outputY;
    float outputZ;
    float[] axes;

    TextView sensorValues;
    /**
     * Called when the activity is first created.
     */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //Initialisierung der Variablen
        jukebox = new MediaPlayerHandler(getApplicationContext());
        drumKit1 = new Drums(getApplicationContext());
        mStreamIDs = new ArrayList<>();
        sensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
        mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_LINEAR_ACCELERATION);
        sensorValues = (TextView) findViewById(R.id.sensor_values);

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
        counter = 0;
        outputX = (float) 2.5;
        outputY = (float) 4.5;
        outputZ = (float) 11;

        axes = new float[3];
        axes[0] = outputX;
        axes[1] = outputY;
        axes[2] = outputZ;


        loaded = true;

        // Add Components to the first DrumKit
        drumKit1.components.put(getString(R.string.instrument_cowbell), R.raw.kit1cowbell);
        drumKit1.components.put(getString(R.string.instrument_cym), R.raw.kit1cym1);
        drumKit1.components.put(getString(R.string.instrument_kick), R.raw.kit1kick);
        drumKit1.components.put(getString(R.string.instrument_snare), R.raw.kit1snare);
        drumKit1.components.put(getString(R.string.instrument_hihat), R.raw.kit1hihat);
        drumKit1.components.put(getString(R.string.instrument_ride), R.raw.kit1ride1);
        drumKit1.components.put(getString(R.string.instrument_tom), R.raw.kit1tom1);

        loadButtons();
        loadSamples();
    }

    public void loadButtons(){
        LinearLayout linear = (LinearLayout) findViewById(R.id.linear);

        LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(
               LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1.0f);

        ArrayList<Button> btnList = new ArrayList<>();
        for (String sampleName : drumKit1.components.keySet()) {
            Button btn = new Button(getApplicationContext());
            btn.setText(sampleName);
            btn.setLayoutParams(param);
            btn.setOnClickListener(handleOnClick(btn));

            btnList.add(btn);
            linear.addView(btn);
        }
    }

    View.OnClickListener handleOnClick(final Button button) {
        return new View.OnClickListener() {
            public void onClick(View v) {
                Button b = (Button)v;
                String buttonText = b.getText().toString();
                playSound(buttonText);
            }
        };
    }

    protected void onResume() {
        super.onResume();
        sensorManager.registerListener(this, mAccelerometer, SensorManager.SENSOR_DELAY_GAME);
    }

    protected void onPause() {
        super.onPause();
        sensorManager.unregisterListener(this);
    }


    public void loadSamples(){
        for (String component : drumKit1.components.keySet()) {
            if (drumKit1.components.get(component) != null) {
                jukebox.addSample(component, drumKit1.components.get(component));
            }
        }
    }
    public void playSound() {
        View v = null;
        playSound(v);
    }
    public void playSound(View v) {
        Random       random    = new Random();
        List<String> keys      = new ArrayList<>(drumKit1.components.keySet());
        String       randomKey = keys.get(random.nextInt(keys.size()) );
        Integer soundID = jukebox.soundIDs.get(randomKey);
        if (soundID != null) {
            jukebox.playSound(soundID);
        }
}

    public void playSound(String sampleName) {
        if (jukebox.soundIDs.containsKey(sampleName)){
            Integer soundID = jukebox.soundIDs.get(sampleName);
            if (soundID != null) {
                jukebox.playSound(soundID);
            }
        }}

    public void playSound(String[] sampleNames) {
        List<Integer> sampleIDs = new ArrayList<>();
        for( String sampleName : sampleNames){
        if (jukebox.soundIDs.containsKey(sampleName)){
            Integer soundID = jukebox.soundIDs.get(sampleName);
            if (soundID != null) {
                sampleIDs.add(soundID);
                //jukebox.playSound(soundID);
                }
            }
        }
        jukebox.playSound(sampleIDs);
}

    public void playLoop(View v) {
        // Is the sound loaded does it already play?
        int tries=0;
        while(true) {// Is the sound loaded
            if(loaded){
                // the sound will play for ever if we put the loop parameter -1
                //soundPool.play(soundID, volume, volume, 1, -1, 1f);
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
           // soundPool.pause(id);
        }
            //soundID = soundPool.load(this, R.raw.beat, counter);
            Toast.makeText(this, "Pause sound", Toast.LENGTH_SHORT).show();
    }

    public void stopSound(View v) {
            for (int id : mStreamIDs) {
             //  soundPool.stop(id);
            }
            //soundID = soundPool.load(this, R.raw.beat, counter);
            Toast.makeText(this, "Stop sound", Toast.LENGTH_SHORT).show();
        jukebox.stopSound("beat");
        }

    @Override
    public void onSensorChanged(SensorEvent event) {
        int bereich1 = 2;
        int bereich2 = 6;
        int bereich3 = 10;
        int bereich4 = 15;
        switch (event.sensor.getType()) {
            case Sensor.TYPE_LINEAR_ACCELERATION:
                axes[0] = event.values[0];
                axes[1]= event.values[1];
                axes[2] = event.values[2];
                for (float value : axes){
                    if (isInRange(value,bereich4,200)){
                        String[] samples = new String[2];
                        samples[0] = getString(R.string.instrument_kick);
                        samples[1] = getString(R.string.instrument_ride);
                        playSound(samples);
                    }
                    else if (isInRange(value,bereich3,bereich4)){
                        String[] samples = new String[2];
                        samples[0] = getString(R.string.instrument_kick);
                        samples[1] = getString(R.string.instrument_hihat);
                        playSound(samples);

                    }
                    else if (isInRange(value,bereich2,bereich3)){
                        String[] samples = new String[2];
                        samples[0] = getString(R.string.instrument_hihat);
                        samples[1] = getString(R.string.instrument_snare);
                        playSound(samples);
                    }
                    else if (isInRange(value,bereich1,bereich2)){
                        playSound(getString(R.string.instrument_hihat));
                    }

                }

                break;
            case Sensor.TYPE_ACCELEROMETER:
                this.outputX = event.values[0];
                this.outputY = event.values[1];
                this.outputZ = event.values[2];
                if (this.outputX > 1){
                    playSound();
                }
                if (this.outputY > 1){
                    playSound();
                }
                if (this.outputZ > 1){
                    playSound();
                }
                break;
            default:
                break;
        }

        ShowSensorValues();
    }

    public boolean isInRange(float value, int untergrenze, int obergrenze){
        return Math.abs(value) < obergrenze && Math.abs(value) > untergrenze;
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

    public void ShowSensorValues(){
        String text = "\nX:" + Float.toString(this.outputX) + "\nY:" + Float.toString(this.outputY) + "\nZ:" + Float.toString(this.outputZ);
        //sensorValues.setText(text);
    }
}
