package com.fellfalla.beatshake;

import android.content.Context;
import android.util.Log;

import java.util.HashMap;
import java.util.Hashtable;
import java.util.Map;
import java.util.Set;

/**
 * Die Instrumentenklasse beschreibt ein Instrument und alle Klangkomponenten und zugehörige Sensibilität
 * die dieses Instrument besitzt
 * Created by Markus on 04.06.2015.
 */
public class Instrument {

    /**
     * Die Komponenten des Instrumentes
     */
    protected Map<String, Integer> components;
    /**
     * Die Sensibilität der einzelnen Komponenten
     */
    protected Map<String, Float> sensitivity;

    protected Context context;

    Instrument(Context context){
        this.context = context;
        components = new HashMap<>();
        sensitivity = new HashMap<>();
    }

    public void  AddComponent(String newComponent){
        components.put(newComponent,null);
        sensitivity.put(newComponent, 99f);
    }

    public void  AddComponent(String newComponent, Integer sampleFile){
        components.put(newComponent,sampleFile);
        sensitivity.put(newComponent, 99f);
    }

    public void  AddComponent(String newComponent, Integer sampleFile, Float sensitivity){
        components.put(newComponent,sampleFile);
        this.sensitivity.put(newComponent, sensitivity);
    }

    /**
     * @param component: Name der gewünschten Komponente
     * @return Liefert die SampleID einer bestimmten Instrumentenkomponente zurück
     */
    public Integer GetSampleIDOfComponent(String component){
        return this.components.get(component);
    }

    public Float GetSensitivity(String component){
        return this.sensitivity.get(component);
    }

    public void SetSensitivity(String component, Float value){
        if (sensitivity.get(component) != null){
            sensitivity.put(component, value);
            Log.i("Beatshake",component + " sensitivity changed to " + value );
        }
    }

    public Set<String> GetComponents(){
        return components.keySet();
    }

}
