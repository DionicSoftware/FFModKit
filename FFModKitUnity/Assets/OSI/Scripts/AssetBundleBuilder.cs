#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

public class AssetBundleBuilder {

    [MenuItem("Assets/Build Asset Bundle")]
    private static void BuildAssetBundle() {
        int totalGameObjects;
        List<ManualShaderChecker.ShaderUsage> manualShaderUsages = ManualShaderChecker.GetShaderUsages(out totalGameObjects);

        if (manualShaderUsages.Any()) {
            throw new System.Exception("Manual shader is used! This is forbidden because it crashes the game on some graphics cards. Please convert the shader to a texture using the \"Convert to Texture\" tool or use a different shader for the asset, like the ManualSmallShader. Check for Manual Shader usage yourself using the \"Tools\" menu at the top.");
        }

        string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        int indexOfLastSlash = path.LastIndexOf('/');
        string folderName = path.Substring(indexOfLastSlash, path.Length - indexOfLastSlash);

        List<Object> assets = new List<Object>();
        foreach (string filePath in ModKitHelper.GetFilesRecursive(path)) {
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
        if (!Directory.Exists(ModKitHelper.GetToolkitDirectory() + "BuiltAssetBundles")) { Directory.CreateDirectory(projectPath + "BuiltAssetBundles"); }

        BuildPipeline.BuildAssetBundle(null, assets.ToArray(), ModKitHelper.GetToolkitDirectory() + "BuiltAssetBundles/" + folderName + ".ffasset", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows64);
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