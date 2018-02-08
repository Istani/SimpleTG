using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global_Data : MonoBehaviour {
    public static Global_Data Instance;

    public Recents picture_recent = new Recents();
    private bool isLoaded = false;

	void Awake () {
	    if (Instance==null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
	}
	
	void Start () { 
        if (Instance!=this) { return; }

        // TODO: Coroutine?
        picture_recent = Recents.Load();
        
        isLoaded = true;
    }

    void Update() {
        if (isLoaded == false) { return; }

        if (Application.loadedLevel==0) {
            Application.LoadLevel(1);   // Switch to Loaded Scene
        }

        // TODO: Open Project Selection
    }
}
