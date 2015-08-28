package com.fellfalla.beatshake.Algorithm;

import junit.framework.TestCase;

/**
 * Created by Markus Weber on 27.08.2015.
 */
public class FunctionTest extends TestCase {



    public void testGetValue() throws Exception {
        String expression1 = "x";
        assertEquals(Function.GetValue(5, expression1),5);
        assertEquals(Function.GetValue(-5, expression1),-5);
        assertEquals(Function.GetValue(1.123, expression1),1.123);
        assertEquals(Function.GetValue(-5.123, expression1),-5.123);
        assertEquals(Function.GetValue(0, expression1),0);

        String expression2 = "x^2";
        assertEquals(Function.GetValue(5, expression2),25);
        assertEquals(Function.GetValue(-5, expression2),25);
        assertEquals(Function.GetValue(1.123, expression2),1.123*1.123);
        assertEquals(Function.GetValue(-5.123, expression2),-5.123*-5.123);
        assertEquals(Function.GetValue(0, expression2),0);


        String expression3 = "x^3";
        String expression4 = "x^17";
        String expression5 = "x+x";
        String expression6 = "x+25x";
        String expression7 = "x+";
        String expression7 = "x+";
        String expression7 = "x+";
        String expression7 = "x+";
        String expression7 = "x+";
        String expression7 = "x+";
    }
}