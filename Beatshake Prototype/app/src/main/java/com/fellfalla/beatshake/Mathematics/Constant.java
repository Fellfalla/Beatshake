package com.fellfalla.beatshake.Mathematics;

/**
 * Created by Markus Weber on 28.08.2015.
 */
public class Constant implements IExpression {
    public Constant(double x){
        Value = x;
    }
    private final double Value;

    @Override
    public double GetValue() {
        return Value;
    }
}
