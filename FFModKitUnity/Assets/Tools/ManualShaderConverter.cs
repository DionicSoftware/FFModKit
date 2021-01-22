#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class ManualShaderConverter {

    [MenuItem("Assets/Convert To Texture")]
    private static void ConvertToTexture() {
        var selected = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(selected); // Assets/Shaders/ManualMaterial.mat
        string assetName = selected.name;
        //MonoBehaviour.print("asset path: " + path);
        if (!(selected is Material)) { return; }
        Material mat = selected as Material;

        List<Color> colors = new List<Color>();
        List<float> smoothness = new List<float>();
        List<float> metallic = new List<float>();
        for (int i = 1; i <= 16; i++) {
            colors.Add(mat.GetColor("_Color" + i));
            smoothness.Add(mat.GetFloat("_Glossiness" + i));
            metallic.Add(mat.GetFloat("_Metallic" + i));
        }

        // create new folder and move
        string parentFolder = path.Substring(0, path.LastIndexOf('/')); // Assets/Shaders

        string[] folders = parentFolder.Split('/');
        string newFolder;
        if (folders[folders.Length - 1] == assetName) {
            newFolder = parentFolder + "/";
        } else {
            newFolder = parentFolder + "/" + assetName + "/"; // Assets/Shaders/ManualMaterial/
            AssetDatabase.CreateFolder(parentFolder, assetName);
            AssetDatabase.MoveAsset(path, newFolder + assetName + ".mat");
        }

        // pathSuffix = Shaders/ManualMaterial/ManualMaterial_
        string pathSuffix = newFolder + assetName + "_";

        string fullPath = Application.dataPath + "/" + pathSuffix.Substring(pathSuffix.IndexOf("/") + 1);

        // Albedo
        Texture2D albedoTex = new Texture2D(4, 4, TextureFormat.ARGB32, false);
        for (int i = 0; i < 16; i++) {
            albedoTex.SetPixel(i / 4, i % 4, colors[i]);
        }
        byte[] albedoPng = albedoTex.EncodeToPNG();

        // Metallic
        Texture2D metallicTex = new Texture2D(4, 4, TextureFormat.ARGB32, false);
        for (int i = 0; i < 16; i++) {
            metallicTex.SetPixel(i / 4, i % 4, new Color(metallic[i], 0, 0, smoothness[i]));
        }
        byte[] metallicPng = metallicTex.EncodeToPNG();

        // Albedo
        string albedoSuffix = "Baked_Albedo.png";
        string albedoPath = fullPath + albedoSuffix;
        File.WriteAllBytes(albedoPath, albedoPng);

        // Metallic
        string metallicSuffix = "Baked_Metallic.png";
        string metallicPath = fullPath + metallicSuffix;
        File.WriteAllBytes(metallicPath, metallicPng);

        AssetDatabase.Refresh();

        // Albedo
        string albedoImporterPath = pathSuffix + albedoSuffix;
        TextureImporter albedoImporter = AssetImporter.GetAtPath(albedoImporterPath) as TextureImporter;
        albedoImporter.filterMode = FilterMode.Point;
        albedoImporter.textureCompression = TextureImporterCompression.Uncompressed;
        albedoImporter.mipmapEnabled = false;
        albedoImporter.alphaIsTransparency = true;
        //AssetDatabase.WriteImportSettingsIfDirty(albedoPath);

        // Metallic
        string metallicImporterPath = pathSuffix + metallicSuffix;
        TextureImporter metallicImporter = AssetImporter.GetAtPath(metallicImporterPath) as TextureImporter;
        metallicImporter.filterMode = FilterMode.Point;
        metallicImporter.textureCompression = TextureImporterCompression.Uncompressed;
        metallicImporter.mipmapEnabled = false;
        metallicImporter.sRGBTexture = false;
        //AssetDatabase.WriteImportSettingsIfDirty(metallicPath);

        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(albedoImporterPath);
        AssetDatabase.ImportAsset(metallicImporterPath);

        string bakedMatPath = newFolder + assetName + "_Baked.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(bakedMatPath) == null) {
            Material bakedMat = new Material(Shader.Find("Custom/Manual/Baked"));
            bakedMat.name = assetName + "_Baked";
            bakedMat.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(albedoImporterPath));
            bakedMat.SetTexture("_Metallic", AssetDatabase.LoadAssetAtPath<Texture2D>(metallicImporterPath));
            bakedMat.enableInstancing = true;
            AssetDatabase.CreateAsset(bakedMat, bakedMatPath);
        }

        MonoBehaviour.print(newFolder + assetName);
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Material>(newFolder + assetName + ".mat"));
    }

    [MenuItem("Assets/Convert To Texture", true)]
    private static bool ConvertToTextureValidation() {
        return Selection.activeObject != null && Selection.activeObject.GetType() == typeof(Material);
    }
}

public class ManualShaderChecker {
    [MenuItem("Tools/Check for Manual Shader Usage")]
    public static void Check() {
        ModKitHelper.ClearUnityConsole();

        int totalGameObjects;
        List<ShaderUsage> usages = GetShaderUsages(out totalGameObjects);

        foreach (ShaderUsage usage in usages) {
            MonoBehaviour.print("Invalid Shader used by " + usage.assetPath + ": " + usage.shaderName + " for material " + usage.materialName);
        }

        MonoBehaviour.print(usages.Count + " out of " + totalGameObjects + " GameObjects use the manual shader.");
    }

    public struct ShaderUsage {
        public string assetPath, shaderName, materialName;
        public ShaderUsage(string assetPath, string shaderName, string materialName) { this.assetPath = assetPath; this.shaderName = shaderName; this.materialName = materialName; }
    }

    public static List<ShaderUsage> GetShaderUsages(out int totalGameObjects) {
        totalGameObjects = 0;
        List<ShaderUsage> usages = new List<ShaderUsage>();

        string[] guids = AssetDatabase.FindAssets("t: GameObject");
        for (int i = 0; i < guids.Length; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null || !(importer is ModelImporter)) { continue; }
            ModelImporter modelImporter = importer as ModelImporter;
            foreach (var kvp in modelImporter.GetExternalObjectMap()) {
                if (kvp.Value is Material) {
                    Material mat = (kvp.Value as Material);
                    string shaderName = mat.shader.name;
                    if (shaderName.Contains("Manual") && !shaderName.EndsWith("Baked") && !shaderName.Contains("ManualSmall")) {
                        usages.Add(new ShaderUsage(assetPath, shaderName, mat.name));
                    }
                }
            }
            totalGameObjects++;
        }
        return usages;
    }

    [MenuItem("Tools/Remove Manual Shader Usage")]
    public static void Remove() {
        ModKitHelper.ClearUnityConsole();

        int count = 0;
        int total = 0;

        string[] guids = AssetDatabase.FindAssets("t: GameObject");
        for (int i = 0; i < guids.Length; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null || !(importer is ModelImporter)) { continue; }
            ModelImporter modelImporter = importer as ModelImporter;
            List<AssetImporter.SourceAssetIdentifier> remapsToRemove = new List<AssetImporter.SourceAssetIdentifier>();
            foreach (var kvp in modelImporter.GetExternalObjectMap()) {
                if (kvp.Value is Material) {
                    Material mat = (kvp.Value as Material);
                    string shaderName = mat.shader.name;
                    if (shaderName.Contains("Manual") && !shaderName.EndsWith("Baked") && !shaderName.Contains("ManualSmall")) {
                        MonoBehaviour.print("Invalid Shader used by " + assetPath + ": " + shaderName + " for material " + mat.name);
                        remapsToRemove.Add(kvp.Key);
                        count++;
                    }
                }
            }
            foreach (var remap in remapsToRemove) {
                modelImporter.RemoveRemap(remap);
                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            total++;
        }

        MonoBehaviour.print(count + " out of " + total + " GameObjects used the manual shader.");
    }
}

#endif
