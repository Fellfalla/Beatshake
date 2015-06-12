package com.fellfalla.beatshake;

import java.util.ArrayList;

/**
 * Sammlung aller Datenpunkte
 * Created by Markus on 11.06.2015.
 */
public class Data {

    public ArrayList<DataPoint> peaks;
    public ArrayList<DataPoint> data;

    Data(int maxDataSize){
        peaks = new ArrayList<>(maxDataSize);
        data = new ArrayList<>(maxDataSize);
    }

    private void AddPeak(DataPoint dataPoint){
        peaks.add(dataPoint);
    }

    DataPoint getLastPeak(){
        return peaks.get(peaks.size() - 1);
    }


    public void AddData(DataPoint dataPoint){
        data.add(dataPoint);
    }



}
