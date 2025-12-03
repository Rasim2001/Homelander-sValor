using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoogleImporter
{
    public class ScriptableObjectUtilityCreator
    {
        public static T CreateAsset<T>(string directoryPath, string assetName) where T : ScriptableObject
        {
            if (!Directory.Exists(directoryPath))
                return null;

            T asset = ScriptableObject.CreateInstance<T>();

#if UNITY_EDITOR
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directoryPath, $"{assetName}.asset"));

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
#endif

            return asset;
        }
    }
}