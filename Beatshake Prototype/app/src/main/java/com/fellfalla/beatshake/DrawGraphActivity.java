package com.fellfalla.beatshake;

import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.FragmentActivity;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.CheckBox;
import android.widget.CheckedTextView;
import android.widget.LinearLayout;

import com.jjoe64.graphview.GraphView;
import com.jjoe64.graphview.series.DataPoint;
import com.jjoe64.graphview.series.LineGraphSeries;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;


public class DrawGraphActivity extends FragmentActivity implements SensorEventListener, CheckedTextView.OnClickListener {
    SensorManager sensorManager;
    Sensor mAccelerometer;
    private CheckBox gravityEnabler;
    private Data data;
    private ArrayList<LineGraphSeries<DataPoint>> lineGraphSeriesArrayList;
    private int counter;
    private int dataPoint = 0;
    private int maxPoints = 200;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_graph);
        data = new Data(100);
        lineGraphSeriesArrayList = new ArrayList<>();
        MeasurePoint examplePoint = new MeasurePoint();

        if (savedInstanceState == null) {
            for (float ignored : examplePoint.getValues()){
                LineGraphSeries<DataPoint> lineGraphSeries = new LineGraphSeries<>();
                for (int i = 0; i < maxPoints; i++){
                    lineGraphSeries.appendData(new DataPoint(0f,0f) ,true, maxPoints);
                }
                lineGraphSeriesArrayList.add(lineGraphSeries);
                createGraph(lineGraphSeries);
            }
        }
        gravityEnabler = (CheckBox) findViewById(R.id.gravity_toggle);
        gravityEnabler.setOnClickListener(this);
        gravityEnabler.setChecked(true);

        sensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
        mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);

        toggleGravity(gravityEnabler);
    }

    public void toggleGravity(View view) {
        gravityEnabler = (CheckBox) findViewById(R.id.gravity_toggle);
        sensorManager.unregisterListener(this);
        if (gravityEnabler.isChecked()){
            mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
            sensorManager.registerListener(this, mAccelerometer, SensorManager.SENSOR_DELAY_FASTEST);
        }
        else {
            mAccelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_LINEAR_ACCELERATION);
            sensorManager.registerListener(this, mAccelerometer, SensorManager.SENSOR_DELAY_FASTEST);
        }
    }

    /**
     * Nimmt eine Liste von Werten und fügt diese in eine LineGraphSerie ein
     * @param values Hier ist noch eine Definition der Schnittstelle notwendig, momentan weren als X-Werte nur Datenpukte durchnummeriert
     * @return LineGraphSeries mit dem Inhalt der List
     */
    private LineGraphSeries<DataPoint> createSeries (List<Float> values){

        DataPoint[] pointArray = new DataPoint[values.size()];

        for (int i = 0; i<values.size(); i++){
            DataPoint point = new DataPoint(i, values.get(i));
            pointArray[i] = point;
        }
        return new LineGraphSeries<>(pointArray);

    }


    /**
     * Generiert einen GraphView und fügt ihn zum Layout hinzu
     * Alternative: http://androidplot.com/download/
     * Styling options: http://www.android-graphview.org/documentation/styling-options
     * Legende: http://www.android-graphview.org/documentation/legend
     * @param series Die zu plotenden Daten
     */
    public void createGraph(LineGraphSeries<DataPoint> series){
        LinearLayout linear = (LinearLayout) findViewById(R.id.fragment_container);
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.WRAP_CONTENT, 1.0f);


        //GraphView graph = new GraphView(getApplicationContext());
        GraphView graph = (GraphView) getLayoutInflater().inflate(R.layout.graph_fragment_layout, (ViewGroup) this.findViewById(R.id.fragment_container), false);

        // Daten hinzufügen
        graph.addSeries(series);

        // Scroll und skalierbarer inhalt
        graph.getViewport().setScalable(true);
        graph.getViewport().setScrollable(true);

        // Beschriftungen
        graph.setTitle("Test Graph");
        graph.getGridLabelRenderer().setHorizontalAxisTitle("Time");
        graph.getGridLabelRenderer().setVerticalAxisTitle("Speed");

        // Design
        graph.getGridLabelRenderer().setPadding(0);
        graph.setTitleColor(getResources().getColor(R.color.graph_title));
        graph.getViewport().setBackgroundColor(getResources().getColor(R.color.graph_background));
        series.setColor(getResources().getColor(R.color.graph));
        linear.addView(graph);
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

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (event.sensor.getType() == mAccelerometer.getType()) {
            data.AddData(new MeasurePoint(event));
            for (int axe = 0; axe < lineGraphSeriesArrayList.size(); axe++) {
                DataPoint dataPoint = new DataPoint(this.dataPoint, event.values[axe]);
                lineGraphSeriesArrayList.get(axe).appendData(dataPoint, true, maxPoints);
                this.dataPoint++;
            }

        }
    }


    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

    @Override
    public void onClick(View v) {

    }
}
