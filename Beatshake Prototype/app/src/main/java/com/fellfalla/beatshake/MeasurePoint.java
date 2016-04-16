package com.fellfalla.beatshake;

import android.hardware.SensorEvent;

/**
 * Enthält alle informationen die wichtige Datenn eines Messpunktes enthalten
 * Messwerte
 * Zeitpunkt
 * Iist ein Peak
 *
 * Created by Markus on 11.06.2015.
 */
public class MeasurePoint implements IMeasurePoint{

    /**
     * Die Datenwerte des Datenpunktes
     */
    private float[] values = new float [3];

    /**
     * Der Zeitpunkt zu dem dieser Datenpunkt entstanden ist
     */
    private long timestamp;

    private MeasurePoint previousMeasurePoint;

    public MeasurePoint(){
        setTimestamp(Constants.getTime());
    }

    public MeasurePoint(SensorEvent event){
        setValues(event);
        setTimestamp(event);
    }

    public long getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(long time) {
        this.timestamp = time;
    }

    public void setTimestamp(SensorEvent sensorEvent) {
        this.timestamp = Constants.getTime(sensorEvent);
    }

    public void setTimestamp(){
        this.timestamp = Constants.getTime();
    }

    public float[] getValues() {
        return values;
    }

    public void setValues(float[] values) {
        this.values[0] = values[0];
        this.values[1] = values[1];
        this.values[2] = values[2];
    }

    public void setValues(float[] values, long timestamp) {
        this.values[0] = values[0];
        this.values[1]= values[1];
        this.values[2] = values[2];
        this.timestamp = timestamp;
    }

    public void setValues(SensorEvent event) {
        this.values[0] = event.values[0];
        this.values[1]= event.values[1];
        this.values[2] = event.values[2];
        this.timestamp = event.timestamp;
    }

    public MeasurePoint getPreviousMeasurePoint() {
        return previousMeasurePoint;
    }

    public void setPreviousMeasurePoint(MeasurePoint previousMeasurePoint) {
        this.previousMeasurePoint = previousMeasurePoint;
    }

    @Override
    public String toString(){
        String string = "";
        string += "\nTimestamp: " + getTimestamp();
        for (int i =0; i<values.length; i++){
            string += "value" + i + ":\t" + values[i];
        }

        return string;
    }
}
