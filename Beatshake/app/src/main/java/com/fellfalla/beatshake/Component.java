package com.fellfalla.beatshake;

/**
 * Created by Markus on 11.06.2015.
 */
public class Component {
    private float Sensitivity;

    public float getSensitivity() {
        return Sensitivity;
    }

    public void setSensitivity(float sensitivity) {
        Sensitivity = sensitivity;
    }

    private long LastPlayed;

    public long getLastPlayed() {
        return LastPlayed;
    }

    public void setLastPlayed(long lastPlayed) {
        LastPlayed = lastPlayed;
    }

    private String ComponentName;

    public String getName() {
        return ComponentName;
    }

    public void setName(String name) {
        ComponentName = name;
    }
}
