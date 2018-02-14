using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obj_Controller : MonoBehaviour {

    enum ObjModes {Move,Scale,Rotate,ScaleFix};
    private List<GameObject> Button_List_Recent = new List<GameObject>();

    public GameObject Layer_GUI;
    public GameObject Layer_Pref_Buttons;

    public GameObject Recent_GUI;
    public GameObject Recent_Pref_Buttons;

    private GameObject SelectedObj = null;
    private ObjModes SelectedMode = 0;

    public Camera Camera_for_Screenshot;
    public RenderTexture Camera_Texture;

    private bool isFocus = false;

    private List<GameObject> Obj_List = new List<GameObject>();
    Vector3 Last_HitPoint = Vector3.zero;
    public GameObject ObjectToCreate;   // Prefab

    // Use this for initialization
    void Start () {
        GenerateRecent();
    }

    // Update is called once per frame
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

    
    public void GenerateObj(int Pic_ID, Pictures pic = null) {
        Texture2D t;
        Pictures[] current_recent = Global_Data.Instance.picture_recent.pic_array;
        if (pic==null) { 
            t = (Texture2D)current_recent[Pic_ID].texture;
        } else {
            t = (Texture2D)pic.texture;
        }
        GameObject temp_object = (GameObject) Instantiate(ObjectToCreate, this.transform);
        SpriteRenderer sr = temp_object.GetComponent<SpriteRenderer>();
        Generate_Layer GL= temp_object.GetComponent<Generate_Layer>();
        if (pic==null) {
            GL.data.pic = current_recent[Pic_ID];
        } else  {
            GL.data.pic = pic;
        }
        
        sr.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        temp_object.transform.localPosition = new Vector3(ObjectToCreate.transform.localPosition.x, Obj_List.Count + 1, ObjectToCreate.transform.localPosition.z);
        Obj_List.Add(temp_object);
        Debug.Log("Generate OBJ " + sr.sprite.name);

        GameObject temp_button = (GameObject)Instantiate(Layer_Pref_Buttons, Layer_GUI.transform);
        GameObject temp_children = temp_button.transform.GetChild(0).gameObject;
        Button btn = temp_button.GetComponent<Button>();
        Image img = temp_children.GetComponent<Image>();

        temp_button.transform.localPosition = new Vector3(Layer_Pref_Buttons.transform.localPosition.x+90, ((Obj_List.Count-1) * 100+60)*-1, Layer_Pref_Buttons.transform.localPosition.z);
        btn.onClick.RemoveAllListeners();
        img.sprite = sr.sprite;
        btn.image = img;
        int ObjToClick = Obj_List.Count - 1;
        btn.onClick.AddListener(() => { SelectObj(ObjToClick); });
        SelectObj(ObjToClick);

        Global_Data.Instance.picture_recent.Add_Picture(current_recent[Pic_ID]);
    }

    public void GenerateObj(Layers layer) {
        GenerateObj(0, layer.pic);
        GameObject GO = Obj_List[Obj_List.Count - 1];
        GO.transform.position = new Vector3(layer.position.x, GO.transform.position.y, layer.position.y);
        GO.transform.localScale = new Vector3(layer.scale.x, layer.scale.y, 1);
        // data.rotation = transform.localRotation.eulerAngles.z;
        GO.transform.localRotation = new Quaternion(0, 0, layer.rotation, 0);

    }

    public void GenerateRecent(int objectToGenerate = -1) {
        Debug.Log("Trying to Regenerate Recent Buttons");
        Clean_Buttons(Button_List_Recent);
        
        Pictures[] current_recent = Global_Data.Instance.picture_recent.pic_array;
        for (int count_recent=0;count_recent<current_recent.Length;count_recent++) {
            if (current_recent[count_recent]==null) {
                Debug.LogWarning("No Recent");
                continue;
            }
            if (current_recent[count_recent].path == null) {
                Debug.LogWarning("No Path for Recent Image");
                continue;
            }
            if (current_recent[count_recent].texture == null) {
                Debug.LogWarning("No Texture for Recent Image");
                continue;
            }
            
            GameObject temp_button = (GameObject)Instantiate(Recent_Pref_Buttons, Recent_GUI.transform);
            GameObject temp_children = temp_button.transform.GetChild(0).gameObject;
            Button btn = temp_button.GetComponent<Button>();
            Image img = temp_children.GetComponent<Image>();
            Texture2D t = (Texture2D)current_recent[count_recent].texture;

            temp_button.transform.localPosition = new Vector3(Recent_Pref_Buttons.transform.localPosition.x + 90, ((count_recent) * 100 + 60) * -1, Recent_Pref_Buttons.transform.localPosition.z);
            
            btn.onClick.RemoveAllListeners();
            int Button_No = count_recent;   // Die Variable selbst darf man ja nicht benutzen beim Button -.- (Bzw wohl eher bei Dynamischen Funktionen?
            btn.onClick.AddListener(() => { GenerateObj(Button_No); });
            
            img.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            btn.image = img;    // ? NOTE: Ist das nicht eigentlich falsch?

            Button_List_Recent.Add(temp_button);
        }
        if (objectToGenerate>=0) {
            GenerateObj(objectToGenerate);
        }
    }

    public void Clean_Buttons(List<GameObject> Button_List) {
        foreach (GameObject g in Button_List) {
            Destroy(g);
        }
    }

    public void SelectObj(int Element) {
        //Debug.Log("Select Element " + Element);
        if (Element<Obj_List.Count) {
            SelectedObj = null;
            for (int count_elements=0;count_elements<Obj_List.Count;count_elements++)
            {
                Obj_List[count_elements].GetComponent<Item_Controller>().isActivated = false;
            }
            if (Element>=0) { 
                SelectedObj = Obj_List[Element];
                SelectedObj.GetComponent<Item_Controller>().isActivated = true;
            }
        }
    }

    public void SelectMode(/*ObjModes ChangeTo*/ int ChangeTo) {
        SelectedMode = (ObjModes)ChangeTo;
    }

    public void DoWithObj(Vector3 NewPoss, Vector3 OldPos) {
        Vector3 Difference = NewPoss - OldPos;

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

    void sleep(float seconds) {
        float start = Time.realtimeSinceStartup;
        while (start + seconds > Time.realtimeSinceStartup)
        {
            new WaitForSecondsRealtime(seconds);
        }
    }

    public void ExportProject() {
        Global_Data.Instance.current_project.Clean_Layer();
        int layer_count = 0;
        foreach (GameObject GO in Obj_List) {
            Generate_Layer GL = GO.GetComponent<Generate_Layer>();
            Layers temp_layer = GL.data;
            Global_Data.Instance.current_project.Add_Layer(temp_layer, layer_count);
            layer_count++;
        }
        Global_Data.Instance.current_project.Save();

        SelectObj(-1);
        sleep(1f);
        string destination = System.IO.Path.Combine(Application.persistentDataPath, "screenshot.png");
        Texture2D screenShot = new Texture2D(Camera_Texture.width, Camera_Texture.height, TextureFormat.RGB24, false);
        RenderTexture.active = Camera_Texture;
        sleep(0.5f);
        Camera_for_Screenshot.Render();
        sleep(0.5f);
        screenShot.ReadPixels(new Rect(0, 0, Camera_Texture.width, Camera_Texture.height), 0, 0);
        RenderTexture.active = null;
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(destination, bytes);
        sleep(0.5f);

        if (!Application.isEditor) {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),uriObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),"My new Thumbnail");
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",intentObject, "Share your new Thumbnail");
            currentActivity.Call("startActivity", chooser);
            new WaitForSecondsRealtime(1);
        }

        new WaitUntil(() => isFocus);
    }
    private void OnApplicationFocus(bool focus) {
        isFocus = focus;
    }
}
