package com.fellfalla.beatshake;

import android.hardware.Sensor;
import android.hardware.SensorManager;
import android.util.LongSparseArray;

import java.util.ArrayList;
import java.lang.Long;

/**
 * Created by Markus Weber on 03.06.2015.
 *
 */
public class Metronome {
    LongSparseArray<float[]> Data;
    LongSparseArray<Boolean> peakTendency;
    public LongSparseArray<Float> peaksDelta;
    ArrayList<Long> peaks;
    public float latestDelta = 0f;
    //ArrayList[] Data = new ArrayList[] {DataX, DataY, DataZ};
    float[] datapoint = new float[3];
    float[] previousDatapoint = new float[3];
    public double AccelerationAccuracy;
    public Sensor accelerationSensor;
    // todo enum mit tendenz einfüge
    boolean rising = true;
    boolean falling= false;

    Metronome(double accelerationAccuracy, int maxDataSize){
        AccelerationAccuracy = accelerationAccuracy;
        Data = new LongSparseArray<>(maxDataSize);
        peaks = new ArrayList<>(maxDataSize);
        peaksDelta = new LongSparseArray<>(maxDataSize);
        Long initPoint = System.currentTimeMillis();
        peaks.add(initPoint);
        peaksDelta.append(initPoint, 0f);
    }

    public void AddData(float data[]){
        long now = System.currentTimeMillis();
        datapoint[0]=data[0];
        datapoint[1]=data[1];
        datapoint[2]=data[2];

        Data.append(now, datapoint.clone());

//        DataY.add(new LongSparseArray<Double>() {{
//            append(now,y);}});
//        DataZ.add(new LongSparseArray<Double>() {{
//            append(now,z);}});

    }

    static void GetMetrum(){
    }

    /**
     * @return Gibt den Zeitpunkt des nächsten zu erwartenden Taktschlages zurück
     */
    Long GetNextPeakExpectation(){
        Long nextPeak = System.currentTimeMillis();
        if (peaks.size() > 1) {
            long sum = 0;
            for (int i = 1; i < peaks.size(); i++) {
                sum += peaks.get(i) - peaks.get(i - 1);
            }
            nextPeak += sum / (peaks.size() - 1);
        }
        return nextPeak;
    }

    Long getLastPeak(){
        return peaks.get(peaks.size() - 1);
    }


    /**
     * @return Berechnet und gibt den neuesten Peak zurück
     */
    Long calculateNewLastPeak(){
        // Ermittelt den letzten Peakwert
        Long latestPeak = getLastPeak();
        for(int i = Data.size()-1; i >0; i--) // >0 da jeder datenpunkt auf den vorherigen schauen muss
        {
            if (Data.keyAt(i) < latestPeak){
                break;
            }
            else{
                long timepoint = Data.keyAt(i);
                LookForPeak(i);
            }
        }
        return latestPeak;
    }

    /**
     * @param peak der Datenzeitpunkt der hinzugefügt werden soll
     * @param flank Bool ob der peak eine erhöhung (rising) oder verminderung (falling) war
     */
    private void AddPeak(Long peak, boolean flank){
        peaks.add(peak);
        peakTendency.append(peak, flank);
    }

    /**
     * Schaut ob der Messpunkt, der sich hinter dem Übergebenen Datenzeitpunkt verbirgt ein Peak ist.
     * Wenn er einer ist, wird dieser Peak in der Klasse als Peak vermerkt
     * @param datakey
     */
    private void LookForPeak(int datakey){
        datapoint = Data.get(datakey);
        previousDatapoint = Data.get(Data.keyAt(datakey-1));
        boolean lastPeakType = peakTendency.get(getLastPeak());
        Long peak;

        for (int j =0; j < datapoint.length; j++){
            // Überprüft ob der letzte Peaktyp gegensetzlich war
            if (!lastPeakType && datapoint[j] > previousDatapoint[j] + accelerationSensor.getResolution()){
                //Der Sensorwert steigt gerade, allso muss er irgendwo gefallen sein
                peak = Data.keyAt(datakey);
                latestDelta = Math.abs(datapoint[j] - previousDatapoint[j]);
                AddPeak(peak,rising);
                // todo: die Werte überspringen die in einem Vorherigen durchgang schon negiert wurden
                break;
            }
            else if (lastPeakType && datapoint[j] < previousDatapoint[j] - accelerationSensor.getResolution()){
                //Der Sensorwert steigt gerade, allso muss er irgendwo gefallen sein
                peak = Data.keyAt(datakey);
                latestDelta = Math.abs(datapoint[j] - previousDatapoint[j]);
                AddPeak(peak, falling);
                break;
            }

        }
    }
}

