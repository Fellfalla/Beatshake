package com.fellfalla.beatshake;

import java.util.ArrayList;

/**
 * Sammlung aller Datenpunkte
 * Created by Markus on 11.06.2015.
 */
public class Data {

    private ArrayList<Peak> peaks;
    private ArrayList<MeasurePoint> measurePoints;

    Data(final int maxDataSize){
        peaks = new ArrayList<Peak>(maxDataSize){
            {for (int i =0; i<maxDataSize; i++){
                add(i,new Peak());}
            }
        };
        measurePoints = new ArrayList<MeasurePoint>(maxDataSize){
            {for (int i =0; i<maxDataSize; i++){
                add(i,new MeasurePoint());}
            }
        };
    }

    public void AddPeak(Peak peak){
        peaks.add(0,peak);
    }

    public ArrayList<Peak> getPeaks(){
        return peaks;
    }

    public ArrayList<MeasurePoint> getMeasurePoints(){
        return measurePoints;
    }

    Peak getLastPeak(){
        return peaks.get(0);
    }

    MeasurePoint getLastMeasurePoint(){
        return measurePoints.get(0); //measurePoints.get(measurePoints.size() - 1);
    }

    public void AddData(MeasurePoint measurePoint) {
        measurePoint.setPreviousMeasurePoint(getLastMeasurePoint());
        measurePoints.add(0,measurePoint);
    }



}
