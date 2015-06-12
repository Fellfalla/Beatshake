package com.fellfalla.beatshake;

import android.hardware.Sensor;
import android.util.LongSparseArray;

import java.lang.Long;

/**
 * Created by Markus Weber on 03.06.2015.
 *
 */
public class Metronome {
    LongSparseArray<Boolean> peakTendency;
    public LongSparseArray<Float> peaksDelta;
    public float latestDelta = 0f;
    //ArrayList[] Data = new ArrayList[] {DataX, DataY, DataZ};

    public double AccelerationAccuracy;
    public Sensor accelerationSensor;

    boolean rising = true;
    boolean falling= false;

    Data data;

    Metronome(double accelerationAccuracy,Data data){
        AccelerationAccuracy = accelerationAccuracy;
        this.data = data;
        peaksDelta = new LongSparseArray<>();
        Long initPoint = System.currentTimeMillis();
        peaksDelta.append(initPoint, 0f);
    }


    static void GetMetrum(){
    }

    /**
     * @return Gibt den Zeitpunkt des nächsten zu erwartenden Taktschlages zurück
     */
    Long GetNextPeakExpectation(){
        Long nextPeakTime = data.getLastPeak().getTimestamp();
        if (data.peaks.size() > 1) {
            long sum = 0;
            for (int i = 1; i < data.peaks.size(); i++) {
                sum += data.peaks.get(i).getTimestamp() - data.peaks.get(i - 1).getTimestamp();
            }
            nextPeakTime += sum / (data.peaks.size() - 1);
        }
        return nextPeakTime;
    }




    /**
     * @return Berechnet und gibt den neuesten Peak zurück
     */
    DataPoint calculateNewLastPeak(){
        // Ermittelt den letzten Peakwert
        DataPoint latestPeak = data.getLastPeak();
        for (DataPoint dataPoint : data.data)
        {
            if (dataPoint.getTimestamp() < latestPeak.getTimestamp()){
                break;
            }
            else{
                LookIfPeak(dataPoint);
            }
        }
//        for(int i = data.data.size()-1; i >0; i--) // >0 da jeder datenpunkt auf den vorherigen schauen muss
//        {
//            if (Data.data.keyAt(i) < latestPeak){
//                break;
//            }
//            else{
//                long timepoint = Data.keyAt(i);
//                LookForPeak(i);
//            }
//        }
        return latestPeak;
    }



    /**
     * Schaut ob der Messpunkt, der sich hinter dem Übergebenen Datenzeitpunkt verbirgt ein Peak ist.
     * Wenn er einer ist, wird dieser Peak in der Klasse als Peak vermerkt
     * @param dataPoint
     */
    private void LookIfPeak(DataPoint dataPoint){
        DataPoint previousDataPoint = data.data.get(data.data.indexOf(dataPoint)-1);

        boolean lastPeakType = previousDataPoint.isTendency();
        Long peak;

        for (int j =0; j < dataPoint.length; j++){
            // Überprüft ob der letzte Peaktyp gegensetzlich war
            if (!lastPeakType && dataPoint[j] > previousDataPoint[j] + accelerationSensor.getResolution()){
                //Der Sensorwert steigt gerade, allso muss er irgendwo gefallen sein
                peak = Data.keyAt(datakey);
                latestDelta = Math.abs(dataPoint[j] - previousDataPoint[j]);
                AddPeak(peak,rising);
                // todo: die Werte überspringen die in einem Vorherigen durchgang schon negiert wurden
                return peak;
                break;
            }
            else if (lastPeakType && dataPoint[j] < previousDataPoint[j] - accelerationSensor.getResolution()){
                //Der Sensorwert steigt gerade, allso muss er irgendwo gefallen sein
                peak = Data.keyAt(datakey);
                latestDelta = Math.abs(dataPoint[j] - previousDataPoint[j]);
                AddPeak(peak, falling);
                return peak;
                break;
            }
        }
        return peak;
    }
}

