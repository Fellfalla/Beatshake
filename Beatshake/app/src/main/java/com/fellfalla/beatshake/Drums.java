package com.fellfalla.beatshake;

import android.content.Context;

import java.util.HashMap;
import java.util.Map;

/**
 * Created by Markus Weber on 02.06.2015.
 * Schlagzeugabbild
 */
public class Drums {

    public Map<String, Integer> components;

    Drums(Context context){
        components = new HashMap<>();
        components.put(context.getResources().getString(R.string.instrument_kick),null);
        components.put(context.getResources().getString(R.string.instrument_cowbell),null);
        components.put(context.getResources().getString(R.string.instrument_cym),null);
        components.put(context.getResources().getString(R.string.instrument_hihat),null);
        components.put(context.getResources().getString(R.string.instrument_ride),null);
        components.put(context.getResources().getString(R.string.instrument_snare),null);
        components.put(context.getResources().getString(R.string.instrument_tom),null);
    }
}
