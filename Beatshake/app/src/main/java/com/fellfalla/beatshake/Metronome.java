package com.fellfalla.beatshake;

import android.content.res.Resources;
import android.util.LongSparseArray;

import java.util.ArrayList;
import java.lang.Long;

/**
 * Created by Markus Weber on 03.06.2015.
 *
 */
public class Metronome {
    LongSparseArray<float[]> Data;
    ArrayList<Long> peaks;

    //ArrayList[] Data = new ArrayList[] {DataX, DataY, DataZ};
    float[] datapoint = new float[3];
    float[] previousDatapoint = new float[3];
    public double AccelerationAccuracy;

    Metronome(double accelerationAccuracy, int maxDataSize){
        AccelerationAccuracy = accelerationAccuracy;
        Data = new LongSparseArray<>(maxDataSize);
        peaks = new ArrayList<>(maxDataSize);
        Long initPoint = System.currentTimeMillis();

        peaks.add(initPoint);
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

    Long getLastPeak(){
        return peaks.get(peaks.size() - 1);
    }

    Long calculateNewLastPeak(){
        // Ermittelt den letzten Peakwert
        Long latestPeak = peaks.get(peaks.size() - 1);
        for(int i = Data.size()-1; i >0; i--) // >0 da jeder datenpunkt auf den vorherigen schauen muss
        {
            if (Data.keyAt(i) < latestPeak){
                break;
            }
            else{
                datapoint = Data.get(Data.keyAt(i));
                previousDatapoint = Data.get(Data.keyAt(i-1));
                for (int j =0; j < datapoint.length; j++){
                    if (datapoint[j] > previousDatapoint[j] + AccelerationAccuracy || datapoint[j] < previousDatapoint[j] - AccelerationAccuracy){
                        latestPeak = Data.keyAt(i);
                        // todo: die Werte überspringen die in einem Vorherigen durchgang schon negiert wurden
                        peaks.add(latestPeak);
                        break;
                    }

                }

            }
        }
        return latestPeak;


}
}

