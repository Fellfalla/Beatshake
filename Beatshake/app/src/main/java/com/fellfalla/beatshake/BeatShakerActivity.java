package com.fellfalla.beatshake;

import android.content.Intent;
import android.content.res.Resources;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.media.AudioManager;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CheckedTextView;
import android.widget.LinearLayout;
import android.widget.SeekBar;
import android.widget.TextView;
import android.widget.Toast;

import org.w3c.dom.Text;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;


public class BeatShakerActivity extends ActionBarActivity implements SensorEventListener, SeekBar.OnSeekBarChangeListener, CheckedTextView.OnClickListener {
    boolean plays = false, loaded = false;
    float actVolume, maxVolume, volume;
    AudioManager audioManager;
    int counter;
    ArrayList<Integer> mStreamIDs ;

    Jukebox jukebox;
    Drums drumKit1;
    SensorManager sensorManager;
    Sensor mAccelerometer;
    Metronome metronome;

    int miliSVerzoegerung = 1000/8;
    float[] axes;

    TextView sensorValues;
    TextView seekBarValue;
    SeekBar accuracySeekBar;
    CheckBox gravityEnabler;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_beat_shaker);
        jukebox = new Jukebox(getApplicationContext());
        drumKit1 = new Drums(getApplicationContext());
        mStreamIDs = new ArrayList<>();
        metronome = new Metronome(0.00, getResources().getInteger(R.integer.max_acceleration_values));
        seekBarValue = (TextView) findViewById(R.id.seek_bar_value);
        sensorValues = (TextView) findViewById(R.id.sensor_values);
        accuracySeekBar = (SeekBar)findViewById(R.id.seekBar1); // make seekbar object
        gravityEnabler = (CheckBox) findViewById(R.id.gravity_bool);

        gravityEnabler.setOnClickListener(this);
        accuracySeekBar.setOnSeekBarChangeListener(this);

        gravityEnabler.setChecked(true);

        sensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
        mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);




        // AudioManager audio settings for adjusting the volume
        audioManager = (AudioManager) getSystemService(AUDIO_SERVICE);
        actVolume = (float) audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
        maxVolume = (float) audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
        volume = actVolume / maxVolume;

        //Hardware buttons setting to adjust the media sound
        this.setVolumeControlStream(AudioManager.STREAM_MUSIC);

        // the counter will help us recognize the stream id of the sound played  now
        counter = 0;

        axes = new float[3];
        axes[0] = (float) 2.5;
        axes[1] = (float) 4.5;
        axes[2] = (float) 11;


        loaded = true;

        // Add Components to the first DrumKit
        drumKit1.components.put(getString(R.string.instrument_cowbell), R.raw.kit1cowbell);
        drumKit1.components.put(getString(R.string.instrument_cym), R.raw.kit1cym1);
        drumKit1.components.put(getString(R.string.instrument_kick), R.raw.kit1kick);
        drumKit1.components.put(getString(R.string.instrument_snare), R.raw.kit1snare);
        drumKit1.components.put(getString(R.string.instrument_hihat), R.raw.kit1hihat);
        drumKit1.components.put(getString(R.string.instrument_ride), R.raw.kit1ride1);
        drumKit1.components.put(getString(R.string.instrument_tom), R.raw.kit1tom1);

        //Receive Intent Message
        Intent intent = getIntent();
        String message = intent.getStringExtra(MainActivity.KITMESSAGE);

        loadButtons();
        loadSamples();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_beat_shaker, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
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
        sensorManager.registerListener(this, mAccelerometer, SensorManager.SENSOR_DELAY_NORMAL);
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
        Random random    = new Random();
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

    @Override
    public void onSensorChanged(SensorEvent event) {
        metronome.AddData(event.values);
        long latestpeak= metronome.getLastPeak();
        int bereich1 = 2;
        int bereich2 = 6;
        int bereich3 = 10;
        int bereich4 = 15;

        if (event.sensor.getType()==mAccelerometer.getType()){
            if (latestpeak + miliSVerzoegerung < metronome.calculateNewLastPeak())
                for (float value : axes){
                    axes[0] = event.values[0];
                    axes[1]= event.values[1];
                    axes[2] = event.values[2];
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
            }
        /*
        switch (event.sensor.getType()) {
            case type:
                axes[0] = event.values[0];
                axes[1]= event.values[1];
                axes[2] = event.values[2];
                if (latestpeak + miliSVerzoegerung < metronome.calculateNewLastPeak())
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
                axes[0] = event.values[0];
                axes[1]= event.values[1];
                axes[2] = event.values[2];
                if (latestpeak + miliSVerzoegerung < metronome.calculateNewLastPeak())
                    for (float value : axes) {
                        if (isInRange(value, bereich4, 200)) {
                            String[] samples = new String[2];
                            samples[0] = getString(R.string.instrument_kick);
                            samples[1] = getString(R.string.instrument_ride);
                            playSound(samples);
                        } else if (isInRange(value, bereich3, bereich4)) {
                            String[] samples = new String[2];
                            samples[0] = getString(R.string.instrument_kick);
                            samples[1] = getString(R.string.instrument_hihat);
                            playSound(samples);

                        } else if (isInRange(value, bereich2, bereich3)) {
                            String[] samples = new String[2];
                            samples[0] = getString(R.string.instrument_hihat);
                            samples[1] = getString(R.string.instrument_snare);
                            playSound(samples);
                        } else if (isInRange(value, bereich1, bereich2)) {
                            playSound(getString(R.string.instrument_hihat));
                        }
                    }
                break;

            default:
                break;
        }
        */
        ShowSensorValues();

    }

    public boolean isInRange(float value, int untergrenze, int obergrenze){
        return Math.abs(value) < obergrenze && Math.abs(value) > untergrenze;
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

    public void ShowSensorValues(){
        String text = "\nX:" + Float.toString(this.axes[0]) + "\t\tY:" + Float.toString(this.axes[1]) + "\t\tZ:" + Float.toString(this.axes[2]);
        sensorValues.setText(text);
    }


    @Override
    public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
        double value = ((double) progress) / getResources().getInteger(R.integer.accuracy_seek_bar_span);
        seekBarValue.setText("Value: " + value);
        metronome.AccelerationAccuracy = value;
    }

    @Override
    public void onStartTrackingTouch(SeekBar seekBar) {

    }

    @Override
    public void onStopTrackingTouch(SeekBar seekBar) {

    }

    @Override
    public void onClick(View v) {
        CheckBox checkbox = (CheckBox) v;
        sensorManager.unregisterListener(this,mAccelerometer);
        if (checkbox.isChecked()){
            mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
            sensorManager.registerListener(this, mAccelerometer, SensorManager.SENSOR_DELAY_UI);
        }
        else {
            mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_LINEAR_ACCELERATION);
            sensorManager.registerListener(this, mAccelerometer, SensorManager.SENSOR_DELAY_GAME);
        }


    }
}
