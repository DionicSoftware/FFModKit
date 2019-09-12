using System.IO;
using UnityEngine;

public class TextureSaver : MonoBehaviour {

    public RenderTexture renderTextureBig;

    enum IconSize {
        Medium,
        Big,
        VeryBig
    }

    void Start () {
        GetComponent<Camera>().targetTexture = renderTextureBig;
    }

    public void SaveTexture(string iconName)
    {
        if (iconName == null || iconName == "") { iconName = "object"; }

        GetComponent<Camera>().targetTexture = renderTextureBig;
        RenderTexture.active = renderTextureBig;
        Texture2D originalIcon = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
        originalIcon.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
        System.IO.File.WriteAllBytes(GetImagePath(iconName, IconSize.VeryBig), originalIcon.EncodeToPNG());

        Texture2D bigIcon = TextureScale.Bilinear(originalIcon, 512, 512);
        System.IO.File.WriteAllBytes(GetImagePath(iconName, IconSize.Big), bigIcon.EncodeToPNG());

        Texture2D mediumIcon = TextureScale.Bilinear(originalIcon, 128, 128);
        System.IO.File.WriteAllBytes(GetImagePath(iconName, IconSize.Medium), mediumIcon.EncodeToPNG());

        print("Saved pic " + iconName + " in " + GetImageDirectory(IconSize.Medium));
    }

    // https://gamedev.stackexchange.com/questions/92285/unity3d-resize-texture-without-corruption
    //public static Texture2D Resize(Texture2D source, int newWidth, int newHeight) {
    //    source.filterMode = FilterMode.Point;
    //    RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
    //    rt.filterMode = FilterMode.Point;
    //    RenderTexture.active = rt;
    //    Graphics.Blit(source, rt);
    //    Texture2D nTex = new Texture2D(newWidth, newHeight);
    //    nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
    //    nTex.Apply();
    //    RenderTexture.active = null;
    //    return nTex;
    //}

    private string GetImagePath(string iconName, IconSize iconSize)
    {
        string suffix = "";
        switch (iconSize) {
            case IconSize.Big: suffix = "Big";break;
            case IconSize.VeryBig: suffix = "VeryBig";break;
        }
        string name = GetImageDirectory(iconSize) + iconName + "Icon" + suffix + ".png";
        return name;
    }

    private string GetImageDirectory(IconSize iconSize) {
        string directory = Helper.GetToolkitDirectory() + "Icons/PhotoStudioIcons" + iconSize.ToString() + "/";

        DirectoryInfo dirInfo = (new FileInfo(directory)).Directory;
        if (!dirInfo.Exists) {
            dirInfo.Create();
        }
        return directory;
    }
}

