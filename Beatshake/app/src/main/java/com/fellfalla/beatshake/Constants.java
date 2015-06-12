package com.fellfalla.beatshake;

import android.hardware.SensorManager;

/**
 * Ansammlung von Kosntanten
 * Created by Markus Weber on 12.06.2015.
 */
public class Constants {
    /**
     * Teile durch diese Zahl, um den Timestamp eines SensorEvents in millisekunden darzustellen
     */
    public static long EVENT_TIMESTAMP_TO_TIMESTAMP = (long) 10e5; //diese Zahl konvertiert Alle Timestamps zu millisekunden
    /**
     * Dividiere durch diese Zahl, um System.currentTimeMillis tatsächlich in millis darzustellen
     */
    public static long SYSTEM_TIMESTAMP_TO_TIMESTAMP = EVENT_TIMESTAMP_TO_TIMESTAMP / (long) 10e5;

    public static int SensorDelay = SensorManager.SENSOR_DELAY_GAME;

}
