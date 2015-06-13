package com.fellfalla.beatshake;

import android.app.usage.UsageEvents;
import android.hardware.SensorEvent;
import android.hardware.SensorManager;

/**
 * Ansammlung von Kosntanten
 * Created by Markus Weber on 12.06.2015.
 */
public class Constants {

    public static long getTime(){
        return System.nanoTime();
    }

    public static long getTime(SensorEvent sensorEvent){
        return sensorEvent.timestamp;
    }

    public static int SensorDelay = SensorManager.SENSOR_DELAY_GAME;

    public static double NANOSECONDS_TO_MILLISECONDS =  10E-5;

    public static double MILLISECONDS_TO_NANOSECONDS = 10E5;

    public static int MINIMAL_METRUM_MILLISECONDS = 100;

    public static int MINIMAL_METRUM_NANOSECONDS = (int) (MINIMAL_METRUM_MILLISECONDS*MILLISECONDS_TO_NANOSECONDS);

    public static int COMPONENT_SENSITIVITY_INITIAL = 99;

    public static float ACCURACY_TOLERANCE_INITIAL = 1.5f;

}
