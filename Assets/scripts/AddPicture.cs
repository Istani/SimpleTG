using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LukeWaffel.AndroidGallery;

public class AddPicture : MonoBehaviour {
    public Obj_Controller OBJ;
    
    public void Import_New_Picture() {
#if UNITY_EDITOR
        Global_Data.Instance.picture_recent.Add_Picture(new Pictures("C:/Dropbox/SimpleSoftwareStudioShare/SimpleTG/Assets/images/CreatorsHub.png"));
        OBJ.GenerateRecent(0);
#elif UNITY_ANDROID
        AndroidGallery.Instance.OpenGallery(Import_Android_Callback);
#endif


    }

    void Import_Android_Callback() {
        Global_Data.Instance.picture_recent.Add_Picture(new Pictures(AndroidGallery.Instance.GetPath()));
        OBJ.GenerateRecent(0);

        AndroidGallery.Instance.ResetOutput();
    }
}
