package com.fellfalla.beatshake;

import android.hardware.SensorEvent;

/**
 * Created by Markus Weber on 12.06.2015.
 */
public interface IMeasurePoint {

    long getTimestamp();

    void setTimestamp(long time);

    float[] getValues();

    void setValues(float[] values);

    void setValues(float[] values, long timestamp);

    void setValues(SensorEvent event);

    MeasurePoint getPreviousMeasurePoint();

    void setPreviousMeasurePoint(MeasurePoint previousMeasurePoint);
}
