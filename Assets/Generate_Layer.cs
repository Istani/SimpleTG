using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate_Layer : MonoBehaviour {

    public Layers data = new Layers();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        data.position = new Vector2(transform.position.x, transform.position.z);
        data.rotation = transform.localRotation.z;
        data.scale = new Vector2 (transform.localScale.x, transform.localScale.y);
	}
}
