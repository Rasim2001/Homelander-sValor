using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ClearProgressTool
    {
        [MenuItem("Tools/Clear Prefs")]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        [MenuItem("TexturePacker/Delete All Materials")]
        public static void DeleteMaterialFromTexturePacker()
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in allAssets)
            {
                if (assetPath.EndsWith("_NRM.mat", System.StringComparison.OrdinalIgnoreCase))
                    AssetDatabase.DeleteAsset(assetPath);
            }

            Debug.Log("Clear");
        }
    }
}