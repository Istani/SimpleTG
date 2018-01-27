using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj_Controller : MonoBehaviour {

    private List<GameObject> Obj_List = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject Temp;
    public void GenerateObj(string name) {
        
        GameObject temp = (GameObject) Instantiate(Temp, this.transform);
        Debug.Log("Generate OBJ " + temp.GetComponent<SpriteRenderer>().sprite.name);
        temp.transform.position = new Vector3(temp.transform.position.x, Obj_List.Count + 1, temp.transform.position.z);
        Obj_List.Add(temp);

    }
}
