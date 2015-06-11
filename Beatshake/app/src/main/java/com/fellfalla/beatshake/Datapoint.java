package com.fellfalla.beatshake;

import android.app.usage.UsageEvents;
import android.hardware.SensorEvent;

/**
 * Enthält alle informationen die wichtige Datenn eines Messpunktes enthalten
 * Messwerte
 * Zeitpunkt
 * Iist ein Peak
 *
 * Created by Markus on 11.06.2015.
 */
public class Datapoint {

    /**
     * Die Datenwerte des Datenpunktes
     */
    private float[] values = new float [3];

    /**
     * Der Zeitpunkt zu dem dieser Datenpunkt entstanden ist
     */
    private long timestamp;

    private long timestamp;

    private boolean tendency;

    public Datapoint(){
    }

    public Datapoint(SensorEvent event){
        setValues(event);
    }

    public long getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(long time) {
        this.timestamp = time;
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

    public boolean isTendency() {
        return tendency;
    }

    public void setTendency(boolean tendency) {
        this.tendency = tendency;
    }
}
