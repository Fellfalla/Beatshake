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
    protected Map<String, Component> components;

    protected Context context;

    Instrument(Context context){
        this.context = context;
        components = new HashMap<>();
    }

    public void  AddComponent(String componentName){
        Component component = new Component(componentName, Constants.COMPONENT_SENSITIVITY_INITIAL, context);
        components.put(componentName,component);
    }

    public void  AddComponent(String componentName, Integer sampleFile){
        Component component = new Component(componentName, Constants.COMPONENT_SENSITIVITY_INITIAL, context);
        component.setSample(sampleFile);
        components.put(componentName,component);
    }

    public void  AddComponent(String componentName, Integer sampleFile, Float sensitivity){
        Component component = new Component(componentName, sensitivity, context);
        component.setSample(sampleFile);
        components.put(componentName,component);
    }

    /**
     * @param component: Name der gewünschten Komponente
     * @return Liefert die SampleID einer bestimmten Instrumentenkomponente zurück
     */
    public Integer GetSampleIDOfComponent(String component){
        return this.components.get(component).getSample();
    }

    public Float GetSensitivity(String component){
        return this.components.get(component).getSensitivity();
    }

    public void SetSensitivity(String component, Float value){
            components.get(component).setSensitivity(value);
            Log.i("Beatshake",component + " sensitivity changed to " + value );
        }


    public Set<String> GetComponents(){
        return components.keySet();
    }

}
