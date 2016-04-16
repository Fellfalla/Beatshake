package com.fellfalla.beatshake;
import android.hardware.SensorEvent;

import java.util.ArrayList;
import java.util.List;

/**
 * Die Peak-Klasse repräsentiert einen besonders wichtigen Messpunkt, welcher sich von den
 * anderen Punkten hervorhebt
 * Created by Markus Weber on 12.06.2015.
 */
public class Peak extends MeasurePoint implements IMeasurePoint {

    private float Strength;

    /**
     * Die Tendenz der Beschleunigung um doppelte interpretation naheliegender Datenpunkte als Peak zu vermeiden
     */
    private Tendency tendency;

    private List<Axes> axes = new ArrayList<>(Axes.values().length);

    public Peak(){
        super();
    }

    public Peak(SensorEvent event){
        super(event);
    }

    public Peak(MeasurePoint measurePoint){
        super();
        setValues(measurePoint.getValues());
        setPreviousMeasurePoint(measurePoint.getPreviousMeasurePoint());
        setTimestamp(measurePoint.getTimestamp());
    }




    /**
     * Relative Stärke des ausschlags
     * @return Gibt einen Float-Wert zurück der die relative Stärke des Peaks repräsentiert
     */
    public float getStrength() {
        return Strength;
    }

    public void setStrength(float strength) {
        Strength = strength;
    }

    public Tendency getTendency() {
        return tendency;
    }

    public void setTendency(Tendency tendency) {
        this.tendency = tendency;
    }

    @Override
    public String toString(){
        String string = "";
        string += "\nTimestamp:\t" + getTimestamp();
        string += "\nStärke:\t" + getStrength();
        string += "\nTendenz:\t" + getTendency();
        for (int i =0; i < getValues().length; i++){
            string += "value" + i + ":\t" + getValues()[i];
        }
        return string;
    }

    public List<Axes> getAxes() {
        return axes;
    }

    public void addAxe(Axes axe) {
        this.axes.add(axe);
    }
}
