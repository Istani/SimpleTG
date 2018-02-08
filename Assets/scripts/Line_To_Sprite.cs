using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Line_To_Sprite : MonoBehaviour {

	// Use this for initialization
    
	void Start () {
        Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        gameObject.GetComponent<BoxCollider>().size = S;
        gameObject.GetComponent<BoxCollider>().center = new Vector2(0, 0);

        LineRenderer LR = gameObject.GetComponent<LineRenderer>();
        LR.SetPosition(0, new Vector3((S.x/2)+0.1f, 0, 0));
        LR.SetPosition(1, new Vector3((S.x/2)*-1 - 0.1f, 0, 0));
        LR.startWidth=S.y + 0.1f;
        LR.endWidth=S.y + 0.1f;
    }

    // Update is called once per frame
    void Update () {
        //Start();
    }
}
