package com.fellfalla.beatshake.Mathematics;

/**
 * Eine Mathematische operation mit 2 Argumenten. Folgende Operationen sind vorhanden:
 * Summation
 * Subtraktion
 * Multiplikation
 * Division
 * Potenzierung
 * Created by Markus Weber on 28.08.2015.
 */
public class Operation implements IExpression {
    private IExpression IExpression1;
    private IExpression IExpression2;
    private Operand Operation;


    public Operation(IExpression IExpression1, Operand operation, IExpression IExpression2){
        this.IExpression1 = IExpression1;
        this.IExpression2 = IExpression2;
        this.Operation = operation;
    }

    private double Exponentiate(IExpression x, IExpression y){
        return Math.pow(x.GetValue(),y.GetValue());
    }

    private double Multiplication(IExpression x, IExpression y){
        return x.GetValue()*y.GetValue();
    }

    private double Division(IExpression x, IExpression y){
        return x.GetValue()/y.GetValue();
    }

    private double Summation(IExpression x, IExpression y){
        return x.GetValue()+y.GetValue();
    }

    private double Subtraction(IExpression x, IExpression y){
        return x.GetValue()-y.GetValue();
    }

    @Override
    public double GetValue() {
        switch (Operation){
            case SUM:
                return Summation(IExpression1, IExpression2);
            case SUB:
                return Subtraction(IExpression1, IExpression2);
            case MUL:
                return Multiplication(IExpression1, IExpression2);
            case DIV:
                return Division(IExpression1, IExpression2);
            case EXP:
                return Exponentiate(IExpression1, IExpression2);
            default:
                return 0;
        }
    }

}
