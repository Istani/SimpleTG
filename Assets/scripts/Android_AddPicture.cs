using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LukeWaffel.AndroidGallery;

public class Android_AddPicture : MonoBehaviour {
    int Recent_Limit = 10;

    public Obj_Controller OBJ;
    public static Android_AddPicture Instance=null;
	// Use this for initialization
	void Start () {
        if (Instance!=null) { 
            Instance = this;
            SetImage();
        }
    }

    /* Lukewaffel*/
    public void Import_New_Picture() {
        AndroidGallery.Instance.OpenGallery(Import_Callback);
    }
    void Import_Callback() {
        Texture myCustomImage = AndroidGallery.Instance.GetTexture();
        OBJ.Recent_List.Add(myCustomImage);
        OBJ.GenerateRecent();
        AndroidGallery.Instance.ResetOutput();
    }

    /* Forum */
    private List<string> GetAllGalleryImagePaths()
    {
        List<string> results = new List<string>();
        HashSet<string> allowedExtesions = new HashSet<string>() { ".png", ".jpg", ".jpeg" };
        try {
            AndroidJavaClass mediaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media");
            const string dataTag = "_data";
            string[] projection = new string[] { dataTag };
            AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = player.GetStatic<AndroidJavaObject>("currentActivity");
            string[] urisToSearch = new string[] { "EXTERNAL_CONTENT_URI", "INTERNAL_CONTENT_URI" };
            foreach (string uriToSearch in urisToSearch) {
                AndroidJavaObject externalUri = mediaClass.GetStatic<AndroidJavaObject>(uriToSearch);
                AndroidJavaObject finder = currentActivity.Call<AndroidJavaObject>("managedQuery", externalUri, projection, null, null, null);
                bool foundOne = finder.Call<bool>("moveToFirst");
                while (foundOne) {
                    int dataIndex = finder.Call<int>("getColumnIndex", dataTag);
                    string data = finder.Call<string>("getString", dataIndex);
                    if (allowedExtesions.Contains(System.IO.Path.GetExtension(data).ToLower())) {
                        string path = @"file:///" + data;
                        results.Add(path);
                    } 
                    foundOne = finder.Call<bool>("moveToNext");
                }
            }
        }
        catch (System.Exception e)
        {
            // do something with error...
            Debug.Log(e);
        }

        return results;
    }
    
    public void SetImage()
    {
        List<string> galleryImages = GetAllGalleryImagePaths();
        Texture2D t = new Texture2D(2, 2);
        (new WWW(galleryImages[0])).LoadImageIntoTexture(t);
        // Hier irgendwas machen bis geladen ist?
        OBJ.Recent_List.Add((Texture)t);
        OBJ.GenerateRecent();
        //m_image.texture = t;
    }
}
