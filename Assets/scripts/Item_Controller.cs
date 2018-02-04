using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Controller : MonoBehaviour {

    private LineRenderer LR;
    private BoxCollider BC;
    public bool isActivated = false;
    public float BorderSize = 0.1f;


    void Start () {
        LR = GetComponent<LineRenderer>();
        BC = GetComponent<BoxCollider>();

        Vector3 newItemSize = new Vector3(BC.size.x/2+ BorderSize, 0, 0);

        LR.SetPosition(0, newItemSize*-1);
        LR.SetPosition(1, newItemSize);
	}
	
	void FixedUpdate () {
		if (isActivated) {
            LR.enabled = true;
        } else {
            LR.enabled = false;
        }
	}
}
