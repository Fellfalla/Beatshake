package com.fellfalla.beatshake;

import android.app.Activity;
import android.content.Intent;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.media.AudioManager;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CheckedTextView;
import android.widget.LinearLayout;
import android.widget.SeekBar;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;


public class BeatShakerActivity extends Activity implements SensorEventListener, SeekBar.OnSeekBarChangeListener, CheckedTextView.OnClickListener {
    float actVolume, maxVolume, volume;
    AudioManager audioManager;
    int counter;
    ArrayList<Integer> mStreamIDs ;
    float sensitivityFaktor; // todo: auslagern in -resource

    Jukebox jukebox;
    Drums drumKit1;
    SensorManager sensorManager;
    Sensor mAccelerometer;
    Metronome metronome;

    int miliSVerzoegerung = 1000/10;

    float[] axes;

    TextView sensorValues;
    TextView seekBarValue;
    SeekBar accuracySeekBar;
    CheckBox gravityEnabler;

    Data data;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_beat_shaker);
        jukebox = new Jukebox(getApplicationContext());
        drumKit1 = new Drums(getApplicationContext());
        mStreamIDs = new ArrayList<>();
        data = new Data(getResources().getInteger(R.integer.max_data_size));
        metronome = new Metronome(0.00, data);
        seekBarValue = (TextView) findViewById(R.id.seek_bar_value);
        sensorValues = (TextView) findViewById(R.id.sensor_values);
        accuracySeekBar = (SeekBar)findViewById(R.id.seekBar1); // make seekbar object
        gravityEnabler = (CheckBox) findViewById(R.id.gravity_toggle);
        sensitivityFaktor = 0.7f;

        gravityEnabler.setOnClickListener(this);
        accuracySeekBar.setOnSeekBarChangeListener(this);

        gravityEnabler.setChecked(true);

        sensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
        mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        metronome.accelerationSensor = mAccelerometer;

        // AudioManager audio settings for adjusting the volume
        audioManager = (AudioManager) getSystemService(AUDIO_SERVICE);
        actVolume = (float) audioManager.getStreamVolume(AudioManager.STREAM_MUSIC);
        maxVolume = (float) audioManager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);
        volume = actVolume / maxVolume;

        //Hardware buttons setting to adjust the media sound
        this.setVolumeControlStream(AudioManager.STREAM_MUSIC);

        // the counter will help us recognize the stream id of the sound played  now
        counter = 0;

        // Add Components to the first DrumKit
        drumKit1.AddComponent(getString(R.string.instrument_cowbell), R.raw.kit1cowbell);
        drumKit1.AddComponent(getString(R.string.instrument_cym), R.raw.kit1cym1);
        drumKit1.AddComponent(getString(R.string.instrument_kick), R.raw.kit1kick);
        drumKit1.AddComponent(getString(R.string.instrument_snare), R.raw.kit1snare);
        drumKit1.AddComponent(getString(R.string.instrument_hihat), R.raw.kit1hihat);
        drumKit1.AddComponent(getString(R.string.instrument_ride), R.raw.kit1ride1);
        drumKit1.AddComponent(getString(R.string.instrument_tom), R.raw.kit1tom1);

        //Receive Intent Message
        Intent intent = getIntent();
        String message = intent.getStringExtra(MainActivity.KITMESSAGE);
        GenerateUI();
        loadSamples();
        toggleGravity(gravityEnabler);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_beat_shaker, menu);
        return true;
    }

    public void GenerateUI(){
        //ArrayList<Button> btnList = new ArrayList<>();
        for (String component : drumKit1.components.keySet()) {
            GenerateComponentUI(component);
        }
    }

    public void GenerateComponentUI(String component){
        loadPlayButton(component);
        loadSensitivitySeekBar(component);
    }


    private void loadPlayButton(String component){
        LinearLayout linear = (LinearLayout) findViewById(R.id.linear);
        LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1.0f);

        // Generate Button and add to ListLayout
        Button btn = new Button(getApplicationContext());
        btn.setText(component);
        btn.setLayoutParams(param);
        btn.setOnClickListener(handleOnClick(btn));
        linear.addView(btn);
    }

    private void loadSensitivitySeekBar(String component){
        LinearLayout linear = (LinearLayout) findViewById(R.id.linear);
        LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1.0f);

        // Generate Button and add to ListLayout
        SeekBar seekBar = new SeekBar(getApplicationContext());
        seekBar.setLayoutParams(param);
        seekBar.setTag(component);
        seekBar.setOnSeekBarChangeListener(this);
        linear.addView(seekBar);

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
        sensorManager.registerListener(this, mAccelerometer, Constants.SensorDelay);
    }

    protected void onPause() {
        super.onPause();
        sensorManager.unregisterListener(this);
    }


    public void loadSamples(){
        for (String component : drumKit1.components.keySet()) {
            if (drumKit1.components.get(component) != null) {
                jukebox.addSample(component, drumKit1.components.get(component).getSample());
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
                //jukebox.playSound(soundID);
                drumKit1.components.get(sampleName).Play();
                Log.i(getString(R.string.app_name), "Played " + sampleName);
            }
        }}

    public void playSound(String[] sampleNames) {
        List<Integer> sampleIDs = new ArrayList<>();
        for( String sampleName : sampleNames){
            if (jukebox.soundIDs.containsKey(sampleName)){
                Integer soundID = jukebox.soundIDs.get(sampleName);
                if (soundID != null) {
                    sampleIDs.add(soundID);
                    jukebox.playSound(soundID);

                }
            }
        }
        jukebox.playSound(sampleIDs);
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        MeasurePoint measurePoint = new MeasurePoint(event);
        data.AddData(measurePoint);

        MeasurePoint lastPeak= data.getLastPeak();

        if (event.sensor.getType()==mAccelerometer.getType()) {
            long t1 = getTime(event);
            long t2 = getTime();
            if (data.getLastPeak().getTimestamp() + miliSVerzoegerung < getTime()) {
                Peak newPeak = metronome.calculateNewLastPeak();
                if (lastPeak.getTimestamp() + miliSVerzoegerung < newPeak.getTimestamp()) //todo: die letzten peaks müssen den einzelnen instrumenten zugewiesen werden
                    data.AddPeak(newPeak); // den ermittelten Peak als Peak verbuchen
                    for (String component : drumKit1.GetComponents()) {
                        if (drumKit1.GetSensitivity(component) * sensitivityFaktor < newPeak.getStrength()) {
                            drumKit1.components.get(component).Play();
                            Log.i(getString(R.string.app_name), "Played " + component + "with Peakvalues: " + newPeak);
                        }
                    }
            }
        }
        ShowSensorValues(event);

    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

    public void ShowSensorValues(SensorEvent event){
        String text = "\nX:" + Float.toString(event.values[0]) + "\t\tY:" + Float.toString(event.values[1]) + "\t\tZ:" + Float.toString(event.values[2]);
        sensorValues.setText(text);
    }

    @Override
    public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
        if (seekBar.getTag()!= null){ // hier werden alle gettagged bars als sensitivity bars angenommen
            SensitivitySeekBarChanged(seekBar.getTag().toString(),progress);
        }
        else{
            double value = ((double) progress) / getResources().getInteger(R.integer.accuracy_seek_bar_span);
            seekBarValue.setText("Value: " + value);
            metronome.AccelerationAccuracy = value;
        }
    }

    public void SensitivitySeekBarChanged(String component, Integer value)
    {
        value -= 1;
        if (value < 0){
            value = 0;
        }
        drumKit1.SetSensitivity(component,(float) value * sensitivityFaktor);
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

    }


    public static long getTime(SensorEvent event){
        return event.timestamp/ Constants.EVENT_TIMESTAMP_TO_TIMESTAMP;
    }

    public static long getTime(){
        return System.currentTimeMillis() / Constants.SYSTEM_TIMESTAMP_TO_TIMESTAMP;
    }

    public void toggleGravity(View view) {
        gravityEnabler = (CheckBox) findViewById(R.id.gravity_toggle);
        sensorManager.unregisterListener(this);
        if (gravityEnabler.isChecked()){
            mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
            sensorManager.registerListener(this, mAccelerometer, Constants.SensorDelay);
        }
        else {
            mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_LINEAR_ACCELERATION);
            sensorManager.registerListener(this, mAccelerometer, Constants.SensorDelay);
        }
    }
}
