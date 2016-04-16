package com.fellfalla.beatshake.Mathematics;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by Markus Weber on 28.08.2015.
 */
public class Variable implements IExpression {
    private String Name;
    private double Value;
    private List<IExpression> Coefficients = new ArrayList<>();

    public Variable(String name, double coefficient){
        this.Name = name;
        Coefficients.add(new Constant(coefficient));
    }

    public Variable(String name, IExpression coefficient){
        this.Name = name;
        Coefficients.add(coefficient);
    }

    public Variable(String name){
        this.Name = name;
        Coefficients.add(new Constant(1));
    }

    public void SetValue(double x){
        this.Value = x;
    }

    public void AddCoefficient(IExpression IExpression){
        Coefficients.add(IExpression);
    }

    @Override
    public double GetValue() {
        double value = this.Value;
        for (IExpression coefficient: Coefficients) {
            value *= coefficient.GetValue();
        }
        return value;
    }
}
