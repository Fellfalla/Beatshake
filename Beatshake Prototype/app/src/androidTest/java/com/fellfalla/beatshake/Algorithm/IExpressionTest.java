package com.fellfalla.beatshake.Algorithm;

import com.fellfalla.beatshake.Mathematics.Constant;
import com.fellfalla.beatshake.Mathematics.IExpression;
import com.fellfalla.beatshake.Mathematics.Operand;
import com.fellfalla.beatshake.Mathematics.Operation;
import com.fellfalla.beatshake.Mathematics.Variable;

import junit.framework.TestCase;

/**
 * Created by Markus Weber on 27.08.2015.
 */
public class IExpressionTest extends TestCase {



    public void testGetValue() throws Exception {

        Constant a = new Constant(4);
        Variable X = new Variable("X", a);

        // Baue den ausdruck 4x*4x zusammen
        IExpression IExpression1 = new Operation(X,Operand.MUL,X);

        // 4*4
        X.SetValue(1);
        assertEquals(4d * 4, IExpression1.GetValue());

        // 4*4*4*4
        X.SetValue(4);
        assertEquals(4d * 4 * 4 * 4, IExpression1.GetValue());

        // Baue den Ausdruck (4x*4x)/4 zusammen
        IExpression IExpression2 = new Operation(IExpression1,Operand.DIV,a);

        assertEquals(4d * 4 * 4 * 4 / 4, IExpression2.GetValue());

        // Füge Coeffizient hinzu
        X.SetValue(1);
        X.AddCoefficient(new Constant(1 / 4d)); // -> Die koeffizienten ergeben zusammen wieder 1
        Variable exponent = new Variable("exponen");
        // (x*x)^exponent
        IExpression IExpression3 = new Operation(IExpression1, Operand.EXP, exponent);

        exponent.SetValue(0);
        assertEquals(1d, IExpression3.GetValue());

        exponent.SetValue(3);
        assertEquals(1d, IExpression3.GetValue());

        // (2*2)^3
        X.SetValue(2);
        assertEquals(4*4*4d, IExpression3.GetValue());



    }

}