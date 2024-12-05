using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnitySpreadsheetSync.Scripts.Data
{
    public class ScriptableObjectTypedGroup<T> : ScriptableObject, IScriptableObjectTypedGroup
        where T : ScriptableObject, IDataBindable
    {
        [SerializeField, Expandable, ReorderableList]
        private List<T> _list;

        public List<ScriptableObject> List => _list.Cast<ScriptableObject>().ToList();

        [SerializeField] private string subAssetName = "Level";

#if UNITY_EDITOR
        [Button]
        public ScriptableObject CreateSubAsset()
        {
            int highestIndex = MaxIndex(subAssetName + "_");

            string newName = subAssetName + "_" + (highestIndex + 1).ToString("D2");
            T newAsset = CreateInstance<T>();
            newAsset.name = newName;
            AssetDatabase.AddObjectToAsset(newAsset, this);
            _list.Add(newAsset);

            // Save the changes
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return newAsset;
        }


        private int MaxIndex(string baseName)
        {
            var highestIndex = 0;
            foreach (var so in List)
            {
                if (so != null && so.name.StartsWith(baseName))
                {
                    var suffix = so.name.Substring(baseName.Length);
                    if (int.TryParse(suffix, out var index))
                    {
                        if (index > highestIndex)
                        {
                            highestIndex = index;
                        }
                    }
                }
            }

            return highestIndex;
        }

        [Button]
        public void RemoveSubAssets()
        {
            while (_list.Count > 0)
            {
                var lastSubAsset = _list[^1];
                _list.RemoveAt(_list.Count - 1);
                var assetPath = AssetDatabase.GetAssetPath(this);
                AssetDatabase.RemoveObjectFromAsset(lastSubAsset);
                DestroyImmediate(lastSubAsset, true);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                string subAssetPath = AssetDatabase.GetAssetPath(lastSubAsset);
                if (!string.IsNullOrEmpty(subAssetPath) && subAssetPath != assetPath)
                {
                    AssetDatabase.DeleteAsset(subAssetPath);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
#endif
    }
}