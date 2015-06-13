package com.fellfalla.beatshake;

import android.hardware.Sensor;
import android.util.Log;

import java.lang.Long;
import java.util.Timer;

/**
 * Created by Markus Weber on 03.06.2015.
 *
 */
public class Metronome {
    private double AccelerationAccuracy;
    public Sensor accelerationSensor;
    public Timer metronome = new Timer();
    private long metrum ;

    Data data;

    Metronome(double accelerationAccuracy,Data data){
        AccelerationAccuracy = accelerationAccuracy;
        this.data = data;
    }

    /**
     * @return Gibt den Zeitpunkt des nächsten zu erwartenden Taktschlages zurück
     */
    Long GetNextPeakExpectation(){
        long nextPeakTime = data.getLastPeak().getTimestamp();
        int size = data.getPeaks().size()-1;
        if (size>4){
            size = 4;
        }
        if (size > 0) {
            long sum = 0;
            for (int i = 0; i < size; i++) {
                sum += data.getPeaks().get(i).getTimestamp() - data.getPeaks().get(i + 1).getTimestamp();
            }
            setMetrum(sum / size);
            nextPeakTime += getMetrum();
        }
        return nextPeakTime;
    }

    public void adjustMetrum(){
        int size = data.getPeaks().size()-1;
        if (size>4){
            size = 4;
        }
        if (size > 0) {
            long sum = 0;
            for (int i = 0; i < size; i++) {
                sum += data.getPeaks().get(i).getTimestamp() - data.getPeaks().get(i + 1).getTimestamp();
            }
            setMetrum(sum / size);
        }
    }

    /**
     * @return Berechnet und gibt den neuesten Peak zurück
     */
    Peak calculateNewLastPeak(){
        // Ermittelt den letzten Peakwert
        Peak latestPeak = data.getLastPeak();
        for (MeasurePoint measurePoint : data.getMeasurePoints())
        {   // todo: die for-schleife läuft in falscher richtung durch
            if (measurePoint.getTimestamp() < latestPeak.getTimestamp()){
                break;
            }
            else{
                latestPeak = LookIfPeak(measurePoint);
            }
        }

        return latestPeak;
    }



    /**
     * Schaut ob der Messpunkt, der sich hinter dem Übergebenen Datenzeitpunkt verbirgt ein Peak ist.
     * Wenn er einer ist, wird dieser als Peak zurückgegeben, falls er keiner ist, wird der letzte
     * Peak zurückgegeben
     * @param measurePoint Der Messpunkt der überprüft werden soll, ob er ein Peak ist und falls ja, als Peak gecastet zurückgegeben werden soll
     */
    public Peak LookIfPeak(MeasurePoint measurePoint){
        MeasurePoint previousMeasurePoint = measurePoint.getPreviousMeasurePoint(); //data.measurePoints.get(data.measurePoints.lastIndexOf(measurePoint) - 1);
        Peak lastPeak = data.getLastPeak();
        Peak peak = lastPeak;

        for (int j =0; j < measurePoint.getValues().length; j++){
            if (previousMeasurePoint == null){
                break; // todo: das muss auch ohne if-Abfrage gehen, um die performance zu verbessern
            }
            // Überprüft ob der letzte Peaktyp gegensetzlich war
            if (/*lastPeak.getTendency() != Tendency.rising &&*/ measurePoint.getValues()[j] > previousMeasurePoint.getValues()[j] + getAccelerationAccuracy()){
                //Der Sensorwert steigt gerade, allso muss er irgendwo gefallen sein
                peak = new Peak(measurePoint);
                peak.setTendency(Tendency.rising);
                peak.setStrength(Math.abs(measurePoint.getValues()[j] - previousMeasurePoint.getValues()[j]));
                peak.setAxe(Axes.values()[j]); // setzt den enum der entsprechenden Achse in die achse ein
                // todo: die Werte überspringen die in einem Vorherigen durchgang schon negiert wurden
                break;
            }
            else if (/*lastPeak.getTendency() != Tendency.falling &&*/ measurePoint.getValues()[j] < previousMeasurePoint.getValues()[j] - getAccelerationAccuracy()){ //accelerationSensor.getResolution()){
                //Der Sensorwert steigt gerade, allso muss er irgendwo gefallen sein
                peak = new Peak(measurePoint);
                peak.setTendency(Tendency.falling);
                peak.setStrength(Math.abs(measurePoint.getValues()[j] - previousMeasurePoint.getValues()[j]));
                peak.setAxe(Axes.values()[j]);
                break;
            }
        }
        return peak;
    }

    public long getMetrum() {
        return metrum;
    }

    public void setMetrum(long metrum) {
        this.metrum = metrum;
    }

    public double getAccelerationAccuracy() {
        return AccelerationAccuracy;
    }

    public void setAccelerationAccuracy(double accelerationAccuracy) {
        AccelerationAccuracy = accelerationAccuracy;
        Log.i("Beatshake","Acceleration Accuracy changed to " + accelerationAccuracy );
    }
}

