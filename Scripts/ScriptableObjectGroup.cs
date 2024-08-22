using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnitySpreadsheetSync.Editor;
#endif


namespace UnitySpreadsheetSync.Scripts
{
    public abstract class ScriptableObjectGroup<T> : ScriptableObject, IScriptableObjectGroup where T : ScriptableObject
    {
        [SerializeField] private List<T> _list;

        public List<ScriptableObject> List => _list.Cast<ScriptableObject>().ToList();

        public T Get(int index)
        {
            return _list[index];
        }

#if UNITY_EDITOR
        [Button]
        public void AddSubAssets()
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
        public void Remove()
        {
            foreach (var so in List)
            {
                AssetUtility.RemoveSubAsset(so);
            }

            _list.Clear();
        }
#endif
    }
}