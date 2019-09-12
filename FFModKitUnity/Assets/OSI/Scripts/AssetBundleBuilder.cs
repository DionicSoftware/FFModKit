#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class AssetBundleBuilder {

    [MenuItem("Assets/Build Asset Bundle")]
    private static void BuildAssetBundle() {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        int indexOfLastSlash = path.LastIndexOf('/');
        string folderName = path.Substring(indexOfLastSlash, path.Length - indexOfLastSlash);

        List<Object> assets = new List<Object>();
        foreach (string filePath in Helper.GetFilesRecursive(path)) {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            if (go != null) {
                assets.Add(go);
                continue;
            }
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
            if (sprite != null) {
                assets.Add(sprite);
                continue;
            }
        }

        string projectPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')+1);
        if (!Directory.Exists(Helper.GetToolkitDirectory() + "BuiltAssetBundles")) { Directory.CreateDirectory(projectPath + "BuiltAssetBundles"); }

        BuildPipeline.BuildAssetBundle(null, assets.ToArray(), Helper.GetToolkitDirectory() + "BuiltAssetBundles/" + folderName + ".ffasset", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Assets/Build Asset Bundle", true)]
    private static bool ConvertToTextureValidation() {
        if (Selection.activeObject == null) {
            return false;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        return System.IO.Directory.Exists(path);
    }
}
#endif