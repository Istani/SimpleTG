using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global_Data : MonoBehaviour {
    public static Global_Data Instance;

    public List<Project> all_projects = new List<Project>();
    public Recents picture_recent = new Recents();
    public Project current_project = new Project();
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
        all_projects = Project.Load_all();
        
        isLoaded = true;
    }

    void Update() {
        if (isLoaded == false) { return; }
        Scene scene = SceneManager.GetActiveScene();
        if (scene.buildIndex==0) {
            SceneManager.LoadScene(1);
        }
    }
}
