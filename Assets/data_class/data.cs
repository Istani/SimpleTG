using UnityEngine;

using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class Global { // #define klappt irgendwie nicht... dann halt so^^
    public const int MAX_RECENT     = 7;
    public const int MAX_LAYER      = MAX_RECENT;
}

public class Pictures {
    public string path = "";

    [XmlIgnore]
    public Texture2D texture = new Texture2D(2, 2);

    public Pictures() {
        // Für den XML Kram braucht er einen publc _constructor_ ohne parameter...
    }

    public Pictures (string load_from) {
        path = load_from;
        ReLoad_Texture();
        Debug.Log("Picture Load: " + path);
    }
    public void ReLoad_Texture() {
        (new WWW(path)).LoadImageIntoTexture(texture);
    }
}

public class Recents {
    public Pictures[] pic_array = new Pictures[Global.MAX_RECENT];  

    public void Add_Picture(Pictures pic) {
        int foundPosition = -1;
        for (int count_pics=0;count_pics<pic_array.Length;count_pics++) {
            if (pic_array[count_pics] == null) {
                continue;
            }
            if (pic_array[count_pics].path==pic.path) {
                foundPosition = count_pics;
            }
        }
        if (foundPosition>=0) {
            for (int count_pics_down = foundPosition; count_pics_down > 0; count_pics_down--) {
                pic_array[count_pics_down] = pic_array[count_pics_down - 1];
            }
        } else { 
            for (int count_pics_down=pic_array.Length-1;count_pics_down>0;count_pics_down--) {
                pic_array[count_pics_down] = pic_array[count_pics_down - 1];
            }
            pic_array[0] = pic;
        }
        Save();
    }

    public void Save() {
        string file_path = Path.Combine(Application.persistentDataPath, "recent.xml");
        var serializer = new XmlSerializer(typeof(Recents));
        using (var stream = new FileStream(file_path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static Recents Load() {
        string file_path = Path.Combine(Application.persistentDataPath, "recent.xml");
        Debug.Log(file_path);
        if (File.Exists(file_path)==false) {
            return new Recents();
        }
        var serializer = new XmlSerializer(typeof(Recents));
        using (var stream = new FileStream(file_path, FileMode.Open))
        {
            Recents ReturnValue=serializer.Deserialize(stream) as Recents;
            for (int Image_to_Load=0;Image_to_Load<ReturnValue.pic_array.Length;Image_to_Load++) {
                if (ReturnValue.pic_array[Image_to_Load]==null) {
                    continue;
                }
                ReturnValue.pic_array[Image_to_Load].ReLoad_Texture();
            }
            return ReturnValue;
        }
    }
}

public class Layers {
    public Pictures pic;
    public Vector2 position;
    public Vector2 scale;
    public float rotation;
}

public class Project {
    public string name;
    public Layers[] layer_array = new Layers[Global.MAX_RECENT];

    public void Add_Layer(Layers layer) { // NOTE: Ich glaube so ist das keine vernünftige idee...
        layer_array[layer_array.Length] = layer;
    }
}