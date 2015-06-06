package com.fellfalla.beatshake;

import android.content.Context;

import java.util.HashMap;
import java.util.Map;

/**
 * Created by Markus Weber on 02.06.2015.
 * Schlagzeugabbild
 */
public class Drums extends Instrument{

    Drums(Context context) {
        super(context);
        AddComponent(context.getResources().getString(R.string.instrument_kick));
        AddComponent(context.getResources().getString(R.string.instrument_cowbell));
        AddComponent(context.getResources().getString(R.string.instrument_cym));
        AddComponent(context.getResources().getString(R.string.instrument_hihat));
        AddComponent(context.getResources().getString(R.string.instrument_ride));
        AddComponent(context.getResources().getString(R.string.instrument_snare));
        AddComponent(context.getResources().getString(R.string.instrument_tom));
    }
}
