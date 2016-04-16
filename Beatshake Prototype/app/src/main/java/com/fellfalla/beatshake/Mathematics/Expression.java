package com.fellfalla.beatshake.Mathematics;

import android.app.Notification;

import org.nfunk.jep.JEP;

import java.util.List;

/**
 * Created by Markus Weber on 28.08.2015.
 */
public class Expression implements IExpression {
    private IExpression Expression;

    public Expression(String expressionString){
        this.Expression = Parse(expressionString);
    }

    public Expression(IExpression expression){
        this.Expression = expression;
    }

    /**
     * @param expressionString Der Analytische ausdruck als String
     *                         Beispielsweise: (x(2+a))^2*(y+x)
     *                         Dinge die gemacht werden müssen:
     *                         - Punkt vor Strich
     *                         - Exponenten als Klammerausdruck
     *                         - Exponenten als einzelne Zahl
     *                         - Multiplikation ohne *-Zeichen
     *                         - Erkennen von Keywords wie cos,sin,tan,exp ...
     *
     *                         Rangfolge:
     *                         1. Klammern
     *                         2. Potenzen
     *                         3. Punktrechnung
     *                         4. Strichrechnung
     *                         5. von links nach rechts
     *
     * @return IExpression, welche aus dem übergebenen Asudruck erzeugt wurde
     */
    public static IExpression Parse(String expressionString){
        // Als erstes werden Klammerausdrücke ausgewertet
        JEP myParser = new JEP();
        myParser.addStandardConstants();
        myParser.addStandardFunctions();
        double d = 5d;
        myParser.addVariableAsObject("x", d);

    }


    @Override
    public double GetValue() {
        return Expression.GetValue();
    }

    enum keyWords{
        sin,
        cos,
        exp
    }


}
