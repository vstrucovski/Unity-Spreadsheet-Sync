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
    public abstract class ScriptableObjectGroup<T> : ScriptableObject, IScriptableObjectGroup where T : ScriptableObject
    {
        [SerializeField, Expandable] private List<T> _list;

        public List<ScriptableObject> List => _list.Cast<ScriptableObject>().ToList();

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
        public void UnlinkSubAssets()
        {
            foreach (var so in List)
            {
                AssetUtility.RemoveSubAsset(so);
            }

            _list.Clear();
        }

        [Button]
        public void CreateNewSub()
        {
            string baseName = "Level_";
            int highestIndex = 0;

            // Search for the highest index in existing sub-assets
            foreach (var so in List)
            {
                if (so != null && so.name.StartsWith(baseName))
                {
                    var suffix = so.name.Substring(baseName.Length);
                    if (int.TryParse(suffix, out int index))
                    {
                        if (index > highestIndex)
                        {
                            highestIndex = index;
                        }
                    }
                }
            }

            string newName = baseName + (highestIndex + 1).ToString("D2");
            T newAsset = CreateInstance<T>();
            newAsset.name = newName;
            AssetDatabase.AddObjectToAsset(newAsset, this);
            _list.Add(newAsset);

            // Save the changes
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}