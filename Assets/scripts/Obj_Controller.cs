using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obj_Controller : MonoBehaviour {

    enum ObjModes {Move,Scale,Rotate,ScaleFix};

    private List<GameObject> Obj_List = new List<GameObject>();
    public GameObject Layer_GUI;
    public GameObject Layer_Pref_Buttons;

    private GameObject SelectedObj = null;
    private ObjModes SelectedMode = 0;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    Vector3 Last_HitPoint = Vector3.zero;
	void Update () {
        // Check Mous Positon
        if (SelectedObj == null) return;

        if (Input.GetMouseButton(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                Vector3 New_HitPoint=hit.point;
                New_HitPoint.z = New_HitPoint.y;
                New_HitPoint.y = SelectedObj.transform.localPosition.y;

                DoWithObj(New_HitPoint, Last_HitPoint);
                Last_HitPoint = New_HitPoint;
            }
        } else {
            Last_HitPoint = Vector3.zero;
        }
	}

    public GameObject ObjectToCreate;
    public void GenerateObj(string name) {
        GameObject temp_object = (GameObject) Instantiate(ObjectToCreate, this.transform);
        Debug.Log("Generate OBJ " + temp_object.GetComponent<SpriteRenderer>().sprite.name);
        temp_object.transform.localPosition = new Vector3(ObjectToCreate.transform.localPosition.x, Obj_List.Count + 1, ObjectToCreate.transform.localPosition.z);
        Obj_List.Add(temp_object);

        GameObject temp_button = (GameObject)Instantiate(Layer_Pref_Buttons, Layer_GUI.transform);
        temp_button.transform.localPosition = new Vector3(Layer_Pref_Buttons.transform.localPosition.x+90, ((Obj_List.Count-1) * 100+60)*-1, Layer_Pref_Buttons.transform.localPosition.z);
        Button btn = temp_button.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        int ObjToClick = Obj_List.Count - 1;
        btn.onClick.AddListener(() => { SelectObj(ObjToClick); });
        SelectObj(ObjToClick);

    }

    public void SelectObj(int Element) {
        //Debug.Log("Select Element " + Element);
        if (Element<Obj_List.Count) {
            SelectedObj = null;
            for (int count_elements=0;count_elements<Obj_List.Count;count_elements++)
            {
                Obj_List[count_elements].GetComponent<Item_Controller>().isActivated = false;
            }
            SelectedObj = Obj_List[Element];
            SelectedObj.GetComponent<Item_Controller>().isActivated = true;
        }
    }

    public void SelectMode(/*ObjModes ChangeTo*/ int ChangeTo) {
        SelectedMode = (ObjModes)ChangeTo;
    }

    public void DoWithObj(Vector3 NewPoss, Vector3 OldPos) {
        Vector3 Difference = NewPoss - OldPos;
        Vector3 Difference_directions = Difference;

        if (Difference.x<0) { Difference.x *= -1; }
        if (Difference.y < 0) { Difference.y *= -1; }
        if (Difference.z < 0) { Difference.z *= -1; }

        float Distance = Vector3.Distance(NewPoss, OldPos);

        float Distance_Old = Vector3.Distance(OldPos, SelectedObj.transform.localPosition);
        float Distance_New = Vector3.Distance(NewPoss, SelectedObj.transform.localPosition);
        
        float Scale_Faktor = 0.1f;

        switch (SelectedMode) {
            case ObjModes.Move:
                SelectedObj.transform.localPosition = NewPoss;
                break;
            case ObjModes.ScaleFix:
            case ObjModes.Scale:
                if (OldPos == Vector3.zero) return;
                if (Distance == 0) return;
                Debug.Log(SelectedObj.transform.localScale + " * " + Distance);

                Vector3 CurrentScale = SelectedObj.transform.localScale;
                SelectedObj.transform.localScale *= Distance;
                
                if (Distance_Old<Distance_New) { 
                    if (SelectedMode==ObjModes.ScaleFix) { 
                        SelectedObj.transform.localScale = new Vector3(CurrentScale.x + Scale_Faktor, CurrentScale.y + Scale_Faktor, CurrentScale.z + Scale_Faktor);
                    } else {
                        SelectedObj.transform.localScale = new Vector3(CurrentScale.x + Scale_Faktor* Difference.x, CurrentScale.y + Scale_Faktor * Difference.z, CurrentScale.z + Scale_Faktor * Difference.y);
                    }
                } else {
                    if (CurrentScale.x < Scale_Faktor) return;
                    if (CurrentScale.y < Scale_Faktor) return;
                    if (CurrentScale.z < Scale_Faktor) return;
                    if (SelectedMode == ObjModes.ScaleFix) {
                        SelectedObj.transform.localScale = new Vector3(CurrentScale.x - Scale_Faktor, CurrentScale.y - Scale_Faktor, CurrentScale.z - Scale_Faktor);
                    } else {
                        SelectedObj.transform.localScale = new Vector3(CurrentScale.x - Scale_Faktor * Difference.x, CurrentScale.y - Scale_Faktor * Difference.z, CurrentScale.z - Scale_Faktor * Difference.y);
                    }
                 }
                break;
            case ObjModes.Rotate:
                SelectedObj.transform.LookAt(NewPoss);
                Vector3 RotationEuler = SelectedObj.transform.localRotation.eulerAngles;
                RotationEuler.x = 90;
                RotationEuler.y = 0;
                RotationEuler.z *= -1;
                SelectedObj.transform.localRotation = Quaternion.Euler(RotationEuler.x, RotationEuler.y, RotationEuler.z);
                break;
        }
    }
}
