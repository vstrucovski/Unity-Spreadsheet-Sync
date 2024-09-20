using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnitySpreadsheetSync.Editor;
#endif


namespace UnitySpreadsheetSync.Scripts
{
    public abstract class ScriptableObjectGroup<T> : ScriptableObject, IScriptableObjectGroup
        where T : ScriptableObject, IDataBindable
    {
        [SerializeField, Expandable, ReorderableList]
        private List<T> _list;

        public List<ScriptableObject> List => _list.Cast<ScriptableObject>().ToList();
        [SerializeField] private string subAssetName = "Level";
        [SerializeField] private string subAssetIds;

        public T Get(int index)
        {
            return _list[Mathf.Clamp(index, 0, _list.Count - 1)];
        }

        public void Add(T value)
        {
            _list.Add(value);
        }

#if UNITY_EDITOR
        [Button]
        public void ConvertToSubAssets()
        {
            var newList = new List<T>();
            foreach (var so in List)
            {
                var newSo = AssetUtility.AddSubAsset(so, this);
                newList.Add((T) newSo);
            }

            _list.Clear();
            _list.AddRange(newList); //TODO not adding
        }

        [Button]
        public void SetMassSubAssetIds()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var item = _list[i];
                item.id = subAssetIds + "_" + (i + 1).ToString("D1");
            }
        }

        [Button]
        public void UnlinkSubAssets()
        {
            foreach (var so in List)
            {
                AssetUtility.RemoveSubAsset(so);
            }

            _list.Clear();
        }

        [Button]
        public void CreateNewSubAsset()
        {
            int highestIndex = MaxIndex(subAssetName);

            string newName = subAssetName + "_" + (highestIndex + 1).ToString("D2");
            T newAsset = CreateInstance<T>();
            newAsset.name = newName;
            AssetDatabase.AddObjectToAsset(newAsset, this);
            _list.Add(newAsset);

            // Save the changes
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
        public void RemoveLastSubAsset()
        {
            if (_list.Count > 0)
            {
                T lastSubAsset = _list[^1];
                _list.RemoveAt(_list.Count - 1);
                string assetPath = AssetDatabase.GetAssetPath(this);
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
            else
            {
                Debug.LogWarning("No sub-assets to remove.");
            }
        }
#endif
    }
}