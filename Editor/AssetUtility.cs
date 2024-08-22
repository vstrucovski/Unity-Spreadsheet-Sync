using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnitySpreadsheetSync.Editor
{
    public static class AssetUtility
    {
#if UNITY_EDITOR
        public static ScriptableObject AddSubAsset(ScriptableObject subAsset, ScriptableObject parentAsset)
        {
            if (subAsset == null || parentAsset == null)
            {
                Debug.LogWarning("Sub-asset or parent asset is null.");
                return null;
            }

            var currentParentPath = AssetDatabase.GetAssetPath(subAsset);
            var parentAssetPath = AssetDatabase.GetAssetPath(parentAsset);

            ScriptableObject newSubAsset = subAsset;
            if (!string.IsNullOrEmpty(currentParentPath) && currentParentPath != parentAssetPath)
            {
                newSubAsset = Object.Instantiate(subAsset);
                newSubAsset.name = subAsset.name;
            }

            if (!AssetDatabase.Contains(newSubAsset))
            {
                AssetDatabase.AddObjectToAsset(newSubAsset, parentAsset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (!string.IsNullOrEmpty(currentParentPath) && currentParentPath != parentAssetPath)
            {
                AssetDatabase.DeleteAsset(currentParentPath);
                Debug.Log($"Deleted original asset file: {currentParentPath}");
            }

            return newSubAsset;
        }

        public static void RemoveSubAsset(ScriptableObject subAsset)
        {
            if (subAsset == null)
            {
                Debug.LogWarning("Sub-asset is null.");
                return;
            }

            var currentParentPath = AssetDatabase.GetAssetPath(subAsset);

            if (!string.IsNullOrEmpty(currentParentPath))
            {
                AssetDatabase.RemoveObjectFromAsset(subAsset);
                var parentDirectory = Path.GetDirectoryName(currentParentPath);
                var newAssetPath = Path.Combine(parentDirectory, $"{subAsset.name}.asset");
                newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);
                AssetDatabase.CreateAsset(subAsset, newAssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Sub-asset does not belong to any asset.");
            }
        }
#endif
    }
}