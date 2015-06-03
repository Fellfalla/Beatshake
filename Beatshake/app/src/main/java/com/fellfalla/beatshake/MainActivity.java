package com.fellfalla.beatshake;

import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.View;


public class MainActivity extends ActionBarActivity {

    public static final String KITMESSAGE = "Soundkit";
    public String Soundkit;

    /**
     * Called when the activity is first created.
     */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Soundkit = "drumkit1png";
        //Make Fullscreen


        //Initialisierung der Variablen
        setContentView(R.layout.activity_main);

    }

    public void startBeatShaker(View view) {
        Intent intent;
        intent = new Intent(this, BeatShakerActivity.class);
        intent.putExtra(KITMESSAGE,Soundkit);
        startActivity(intent);
    }

}
