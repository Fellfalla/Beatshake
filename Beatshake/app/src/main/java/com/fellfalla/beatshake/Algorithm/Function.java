package com.fellfalla.beatshake.Algorithm;

/**
 * Diese Klasse repräsentiert eine analytische Funktion
 * Created by Markus Weber on 27.08.2015.
 */
public class Function {
    public String Expression;

    /**
     * @param x
     * @return
     * Gibt den Wert des y-Abschnitts zu einem Bestimmten X-Wert zurück
     */
    public static double GetValue(double x, String expression){
        return x;
    }

    private double Euler(double x){
        return Math.exp(x);
    }

    private double Potentation(double x, double y){
        return Math.pow(x,y);
    }

    private double Multiplication(double x, double y){
        return x*y;
    }

    private double Division(double x, double y){
        return x/y;
    }

    private double Summation(double x, double y){
        return x+y;
    }

    private double Substraction(double x, double y){
        return x-y;
    }





}
