package com.fellfalla.beatshake;

import android.util.LongSparseArray;

import java.util.ArrayList;

/**
 * Sammlung aller Datenpunkte
 * Created by Markus on 11.06.2015.
 */
public class Data {

    public ArrayList<Datapoint> peaks;
    public ArrayList<Datapoint> data;

    Data(int maxDataSize){
        peaks = new ArrayList<>(maxDataSize);
        data = new ArrayList<>(maxDataSize);
    }

    private void AddPeak(Datapoint datapoint){
        peaks.add(datapoint);
    }

    Datapoint getLastPeak(){
        return peaks.get(peaks.size() - 1);
    }


    public void AddData(Datapoint datapoint){
        data.add(datapoint);
    }



}
