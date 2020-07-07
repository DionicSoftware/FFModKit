#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

public class AssetBundleBuilder
{

    [MenuItem("Assets/Build Asset Bundle")]
    private static void BuildAssetBundle()
    {
        int totalGameObjects;
        List<ManualShaderChecker.ShaderUsage> manualShaderUsages = ManualShaderChecker.GetShaderUsages(out totalGameObjects);

        if (manualShaderUsages.Any())
        {
            throw new System.Exception("Manual shader is used! This is forbidden because it crashes the game on some graphics cards. Please convert the shader to a texture using the \"Convert to Texture\" tool or use a different shader for the asset, like the ManualSmallShader. Check for Manual Shader usage yourself using the \"Tools\" menu at the top.");
        }

        string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        int indexOfLastSlash = path.LastIndexOf('/');
        string folderName = path.Substring(indexOfLastSlash, path.Length - indexOfLastSlash);

        List<Object> assets = new List<Object>();
        foreach (string filePath in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
        {
            if (Path.GetExtension(path) == ".meta") continue;

            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            if (go != null)
            {
                assets.Add(go);
                continue;
            }
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
            if (sprite != null)
            {
                assets.Add(sprite);
                continue;
            }
        }

        DirectoryInfo assetDir = new DirectoryInfo(Application.dataPath); //returns the "Assets" directory
        DirectoryInfo modKitRootDir = assetDir.Parent.Parent; //returns the "FFModKit-master" directory

        string projectPath = modKitRootDir.FullName + "\\BuiltAssetBundles";

        if (!Directory.Exists(projectPath)) Directory.CreateDirectory(projectPath);

        BuildPipeline.BuildAssetBundle(null,
            assets.ToArray(),
            $"{projectPath}\\{folderName}.ffasset",
            BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows64);

    }

    [MenuItem("Assets/Build Asset Bundle", true)]
    private static bool ConvertToTextureValidation()
    {
        if (Selection.activeObject == null)
        {
            return false;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        return System.IO.Directory.Exists(path);
    }
}
#endif