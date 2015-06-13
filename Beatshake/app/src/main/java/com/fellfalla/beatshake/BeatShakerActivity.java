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
import android.widget.RelativeLayout;
import android.widget.SeekBar;
import android.widget.TextView;

import java.lang.reflect.Array;
import java.sql.Time;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;
import java.util.Timer;
import java.util.TimerTask;


public class BeatShakerActivity extends Activity implements SensorEventListener, SeekBar.OnSeekBarChangeListener, CheckedTextView.OnClickListener {
    float actVolume, maxVolume, volume;
    AudioManager audioManager;
    int counter;
    ArrayList<Integer> mStreamIDs ;
    float sensitivityFaktor; // todo: auslagern in -resource

    //Jukebox jukebox;
    Drums drumKit1;
    SensorManager sensorManager;
    Sensor mAccelerometer;
    Metronome metronome;

    TextView sensorValues;
    TextView seekBarValue;
    SeekBar accuracySeekBar;
    CheckBox gravityEnabler;

    Data data;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_beat_shaker);
        //jukebox = new Jukebox(getApplicationContext());
        drumKit1 = new Drums(getApplicationContext());
        mStreamIDs = new ArrayList<>();
        data = new Data(getResources().getInteger(R.integer.max_data_size));
        metronome = new Metronome(Constants.ACCURACY_TOLERANCE_INITIAL, data);
        seekBarValue = (TextView) findViewById(R.id.seek_bar_value);
        sensorValues = (TextView) findViewById(R.id.sensor_values);
        accuracySeekBar = (SeekBar)findViewById(R.id.seekBar1); // make seekbar object
        gravityEnabler = (CheckBox) findViewById(R.id.gravity_toggle);
        sensitivityFaktor = 0.7f;

        gravityEnabler.setOnClickListener(this);
        accuracySeekBar.setOnSeekBarChangeListener(this);
        accuracySeekBar.setProgress((int)(Constants.ACCURACY_TOLERANCE_INITIAL * getResources().getInteger(R.integer.accuracy_seek_bar_span)));

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
        //loadSamples();
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
        loadAxesCheckboxes(component);
    }


    private void loadPlayButton(String component){
        LinearLayout linear = (LinearLayout) findViewById(R.id.linear);
        LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1.0f);

        // Generate Button and add to ListLayout
        Button btn = new Button(getApplicationContext());
        btn.setText(component);
        btn.setLayoutParams(param);
        btn.setOnClickListener(handleOnClick());
        linear.addView(btn);
    }

    private void loadSensitivitySeekBar(String component){
        LinearLayout linear = (LinearLayout) findViewById(R.id.linear);
        LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1.0f);

        // Generate Button and add to ListLayout
        SeekBar seekBar = new SeekBar(getApplicationContext());
        seekBar.setLayoutParams(param);
        seekBar.setProgress(Constants.COMPONENT_SENSITIVITY_INITIAL);
        seekBar.setTag(component);
        seekBar.setOnSeekBarChangeListener(this);
        linear.addView(seekBar);
    }

    private void loadAxesCheckboxes(String component){
        LinearLayout linear = (LinearLayout) findViewById(R.id.linear);
        LinearLayout.LayoutParams param = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1.0f);

        LinearLayout linearLayout = new LinearLayout(this);
        linearLayout.setOrientation(LinearLayout.HORIZONTAL);

        for (Axes axe : Axes.values()) {
            CheckBox checkBox = new CheckBox(this);
            checkBox.setText(axe.toString());
            checkBox.setActivated(true);
            checkBox.setChecked(true);
            checkBox.setTag(R.string.view_component, component);
            checkBox.setTag(R.string.view_axe, axe);
            this.drumKit1.components.get(component).activatedAxes.add(checkBox);
            linearLayout.addView(checkBox);
        }

        linear.addView(linearLayout);
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



    View.OnClickListener handleOnClick() {
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


//    public void loadSamples(){
//        for (String component : drumKit1.components.keySet()) {
//            if (drumKit1.components.get(component) != null) {
//                jukebox.addSample(component, drumKit1.components.get(component).getSample());
//            }
//        }
//    }

    public void playSound() {
        View v = null;
        playSound(v);
    }
    public void playSound(View v) {
        Random random    = new Random();
        List<String> keys      = new ArrayList<>(drumKit1.components.keySet());
        String       randomKey = keys.get(random.nextInt(keys.size()) );
        String component = drumKit1.components.get(randomKey).getName();
        if (component != null) {
            playSound(component);
        }
    }

    public void playSound(String sampleName) {
        if (drumKit1.components.containsKey(sampleName)){
                drumKit1.components.get(sampleName).Play();
                Log.i(getString(R.string.app_name), "Played " + sampleName);
            }
        }

    public void playSound(String[] sampleNames) {
        List<Integer> sampleIDs = new ArrayList<>();
        for( String sampleName : sampleNames){
            playSound(sampleName);
        }
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (event.sensor.getType()==mAccelerometer.getType()) {
            data.AddData(new MeasurePoint(event));

            Peak newPeak = metronome.LookIfPeak(data.getLastMeasurePoint()); //metronome.calculateNewLastPeak();
            Peak lastPeak = data.getLastPeak();
            if (lastPeak.getTimestamp() + Constants.MINIMAL_METRUM_NANOSECONDS < newPeak.getTimestamp())
            {
                //todo: die letzten peaks müssen den einzelnen instrumenten zugewiesen werden
                data.AddPeak(newPeak); // den ermittelten Peak als Peak verbuchen
                //adjustMetronome();
                for (String component : drumKit1.GetComponents()) {
                    if (drumKit1.GetSensitivity(component) * sensitivityFaktor < newPeak.getStrength()) {
                        if (drumKit1.components.get(component).isAxeActivated(newPeak.getAxe())) {
                            drumKit1.components.get(component).Play();
                            Log.i(getString(R.string.app_name), "Played " + component + "with Peakvalues: " + newPeak);
                        }
                    }
                }
            }
        }
        ShowSensorValues(event);

    }

    public void adjustMetronome(){
            metronome.adjustMetrum();
            final TimerTask timerTask = new TimerTask() {
                @Override
                public void run() {
                    playSound(getString(R.string.instrument_hihat));
                    adjustMetronome();
                }
            };
            long delayTime = (long) (metronome.getMetrum()* Constants.NANOSECONDS_TO_MILLISECONDS);
            metronome.metronome.schedule(timerTask, delayTime);
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
            metronome.setAccelerationAccuracy(value);
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
        if (v.getId() == R.id.gravity_toggle){
            toggleGravity(v);
        }
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
